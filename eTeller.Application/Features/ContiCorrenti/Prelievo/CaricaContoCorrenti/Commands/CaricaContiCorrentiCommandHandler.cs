using AutoMapper;
using eTeller.Application.Contracts;
using eTeller.Application.Contracts.StoreProcedures;
using eTeller.Application.Contracts.StoreProcedures.Vigilanza;
using eTeller.Application.Mappings.Account;
using eTeller.Domain.Models;
using eTeller.Domain.Services;
using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;

namespace eTeller.Application.Features.ContiCorrenti.Commands.Carica;

/// <summary>
/// Handler per il comando CaricaContiCorrenti.
/// Orchestra le fasi: validazione → calcolo cambi → verifica IS107 →
/// verifica antiriciclaggio → salvataggio transazione → generazione movimenti.
/// </summary>
public sealed class CaricaContiCorrentiCommandHandler
    : IRequestHandler<CaricaContiCorrentiCommand, Result<CaricaContiCorrentiResult>>
{
    private const string TipoBigliettiBank = "BB";
    private const string DivisaBase        = "CHF";

    private readonly IUnitOfWork              _unitOfWork;
    private readonly IValidator<CaricaContiCorrentiCommand> _validator;
    private readonly IForexDomainService      _forexService;
    private readonly IIS107DomainService      _is107Service;
    private readonly IAccountSpRepository     _accountRepository;
    private readonly IVigilanzaSpRepository   _vigilanzaRepository;
    private readonly IMapper                  _mapper;
    private readonly ILogger<CaricaContiCorrentiCommandHandler> _logger;

    public CaricaContiCorrentiCommandHandler(
        IUnitOfWork unitOfWork,
        IValidator<CaricaContiCorrentiCommand> validator,
        IForexDomainService forexService,
        IIS107DomainService is107Service,
        IAccountSpRepository accountRepository,
        IVigilanzaSpRepository vigilanzaRepository,
        IMapper mapper,
        ILogger<CaricaContiCorrentiCommandHandler> logger)
    {
        _unitOfWork          = unitOfWork;
        _validator           = validator;
        _forexService        = forexService;
        _is107Service        = is107Service;
        _accountRepository   = accountRepository;
        _vigilanzaRepository = vigilanzaRepository;
        _mapper              = mapper;
        _logger              = logger;
    }

    public async Task<Result<CaricaContiCorrentiResult>> Handle(
        CaricaContiCorrentiCommand command,
        CancellationToken cancellationToken)
    {
        // ── FASE 2: Validazione campi ─────────────────────────────────────────
        var validationResult = await _validator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors
                .Select(e => new Error(e.ErrorMessage).WithMetadata("ErrorCode", e.ErrorCode))
                .ToList();
            return Result.Fail<CaricaContiCorrentiResult>(errors);
        }

        // ── FASE 4: Ricerca dati conto ────────────────────────────────────────
        var contoResult = await CaricaDatiContoAsync(command, cancellationToken);
        if (contoResult.IsFailed) return contoResult.ToResult<CaricaContiCorrentiResult>();

        var datoConto = contoResult.Value;

        // ── FASE 5-6: Calcolo cambi e validazione tolleranza ──────────────────
        var cambioResult = await CalcolaCambioEValidaAsync(command, datoConto, cancellationToken);
        if (cambioResult.IsFailed) return cambioResult.ToResult<CaricaContiCorrentiResult>();

        var cambioCalcolato = cambioResult.Value;

        // ── FASE 8: Calcolo importi finali ────────────────────────────────────
        var importiResult = await CalcolaImportiFinaliAsync(command, cambioCalcolato, cancellationToken);
        if (importiResult.IsFailed) return importiResult.ToResult<CaricaContiCorrentiResult>();

        var importiCalcolati = importiResult.Value;

        // ── FASE 9: Controllo IS107 ───────────────────────────────────────────
        var is107Result = await VerificaIS107Async(command, datoConto, importiCalcolati, cancellationToken);
        if (is107Result.IsFailed) return is107Result.ToResult<CaricaContiCorrentiResult>();

        // ── FASE 10: Validazione antiriciclaggio ──────────────────────────────
        var vigilanzaResult = await VerificaVigilanzaAsync(command, datoConto, importiCalcolati, cancellationToken);
        if (vigilanzaResult.IsFailed) return vigilanzaResult.ToResult<CaricaContiCorrentiResult>();

        // ── FASE 11-12: Salvataggio transazione e movimenti ───────────────────
        return await SalvaTransazioneAsync(
            command,
            datoConto,
            cambioCalcolato,
            importiCalcolati,
            vigilanzaResult.Value,
            cancellationToken);
    }

    // ─────────────────────────────────────────────────────────────────────────
    // FASE 4: Carica dati conto
    // ─────────────────────────────────────────────────────────────────────────

    private async Task<Result<DatiConto>> CaricaDatiContoAsync(
        CaricaContiCorrentiCommand command,
        CancellationToken ct)
    {
        var contoEntity = await _accountRepository.GetAccountInfoAsync(command.NumeroConto);

        if (contoEntity is null)
            return Result.Fail<DatiConto>(
                new Error($"Conto {command.NumeroConto} non trovato.")
                    .WithMetadata("ErrorCode", "1305"));

        var conto = _mapper.Map<CustomerAccountVm>(contoEntity);

        return Result.Ok(new DatiConto(
            Account:      conto.AccId,
            Titolare:     conto.AccRubrica,
            Divisa:       conto.AccDivisa,
            Saldo:        conto.AccSaldo,
            TipoConto:    conto.AccType,
            Categoria:    conto.AccCategoria,
            IsDipendente: false,
            IS107:        string.Empty,
            NumeroRel:    conto.AccNea,
            Patrimonio:   0m));
    }

    // ─────────────────────────────────────────────────────────────────────────
    // FASE 5-6: Calcolo cambio + tolleranza
    // ─────────────────────────────────────────────────────────────────────────

    private async Task<Result<CambioCalcolatoDto>> CalcolaCambioEValidaAsync(
        CaricaContiCorrentiCommand command,
        DatiConto datoConto,
        CancellationToken ct)
    {
        var diviseConto = NormalizzaDivisa(command.DivisaConto);
        var diviseBb    = NormalizzaDivisa(command.DivisaBanconote);

        // Determina se applicare lo spread (solo se divise diverse e categoria lo prevede)
        var applicaSpread = diviseConto != diviseBb
            && await _accountRepository.UsaSpreadAsync(command.NumeroConto, datoConto.Categoria, ct);

        var cambio = await _forexService.CalcolaCambioAsync(
            diviseConto,
            diviseBb,
            command.TipoOperazione,
            applicaSpread,
            datoConto.IsDipendente,
            ct);

        // Validazione tolleranza se l'operatore ha inserito un cambio manuale
        if (command.TassoCambio.HasValue && diviseConto != diviseBb)
        {
            var divisaDaControllare = diviseConto == DivisaBase ? diviseBb : diviseConto;

            var isFuoriTolleranza = await _forexService.IsFuoriTolleranzaAsync(
                divisaDaControllare,
                command.TassoCambio.Value,
                cambio.Prezzo,
                command.TipoOperazione,
                command.ForzaCambio,
                ct);

            if (isFuoriTolleranza)
                return Result.Fail<CambioCalcolatoDto>(
                    new Error("Cambio fuori tolleranza rispetto al cambio di sistema.")
                        .WithMetadata("ErrorCode", "2018"));
        }

        return Result.Ok(cambio);
    }

    // ─────────────────────────────────────────────────────────────────────────
    // FASE 8: Calcolo importi finali
    // ─────────────────────────────────────────────────────────────────────────

    private async Task<Result<ImportiCalcolati>> CalcolaImportiFinaliAsync(
        CaricaContiCorrentiCommand command,
        CambioCalcolatoDto cambio,
        CancellationToken ct)
    {
        var diviseConto = NormalizzaDivisa(command.DivisaConto);
        var diviseBb    = NormalizzaDivisa(command.DivisaBanconote);

        var tassoEffettivo = command.TassoCambio ?? cambio.Prezzo;
        var tassoCtvEffettivo = command.TassoCambioControvalore ?? cambio.PrezzoDivisa2;

        decimal importoConto, importoBanconote;

        // Se solo importo conto → calcola banconote; se solo banconote → usa direttamente
        if (command.ImportoConto.HasValue && !command.ImportoBanconote.HasValue)
        {
            importoBanconote = CalcolaControparte(
                command.ImportoConto.Value,
                tassoEffettivo,
                cambio.ScalaDivisa1,
                cambio.ScalaDivisa2,
                cambio.Direzione);

            importoBanconote = await _forexService.ArrotondaAlTaglioMinimoAsync(
                diviseBb, importoBanconote, command.ArrotondaTaglioMinimo, ct);

            importoConto = RicalcolaImportoConto(
                importoBanconote,
                tassoEffettivo,
                cambio.ScalaDivisa1,
                cambio.ScalaDivisa2,
                cambio.Direzione);

            importoConto = await _forexService.ArrotondaAlTaglioMinimoAsync(
                diviseConto, importoConto, false, ct);
        }
        else
        {
            importoBanconote = command.ImportoBanconote!.Value;
            importoConto     = command.ImportoConto ?? CalcolaControparte(
                importoBanconote, tassoEffettivo,
                cambio.ScalaDivisa2, cambio.ScalaDivisa1, !cambio.Direzione);
        }

        // Calcolo aggio
        var (aggioPer, aggioImporto, aggioCtv) = CalcolaAggio(
            command, importoBanconote, tassoCtvEffettivo, cambio.ScalaDivisa2);

        // Calcolo importo finale sul conto (con aggio incluso)
        var importoContoFinale = CalcolaImportoContoConAggio(
            command.TipoOperazione, importoConto, aggioImporto,
            tassoEffettivo, cambio.ScalaDivisa1, cambio.ScalaDivisa2, cambio.Direzione,
            diviseConto);

        // Controvalore CHF banconote
        decimal controValoreBanconote = diviseBb == DivisaBase
            ? importoBanconote
            : importoBanconote * tassoCtvEffettivo / cambio.ScalaDivisa2;

        controValoreBanconote = Math.Round(controValoreBanconote, 2);

        // Controvalore CHF totale (incluso aggio)
        var totaleCtv = command.TipoOperazione == "WITH"
            ? controValoreBanconote + aggioCtv
            : controValoreBanconote - aggioCtv;

        return Result.Ok(new ImportiCalcolati(
            ImportoConto:           importoContoFinale,
            ImportoBanconote:       importoBanconote,
            ControvaloreChfBanc:    controValoreBanconote,
            ControvaloreChfTotale:  totaleCtv,
            AggioPercent:           aggioPer,
            AggioImporto:           aggioImporto,
            AggioImportoCtv:        aggioCtv,
            TassoEffettivo:         tassoEffettivo,
            TassoCtvEffettivo:      tassoCtvEffettivo));
    }

    // ─────────────────────────────────────────────────────────────────────────
    // FASE 9: Verifica IS107
    // ─────────────────────────────────────────────────────────────────────────

    private async Task<Result> VerificaIS107Async(
        CaricaContiCorrentiCommand command,
        DatiConto datoConto,
        ImportiCalcolati importi,
        CancellationToken ct)
    {
        var verificaRequest = new IS107VerificaRequest
        {
            TransactionId          = command.TransactionId ?? 0,
            NumeroRelazione        = datoConto.NumeroRel,
            FlagIS107              = datoConto.IS107,
            TipoOperazione         = command.TipoOperazione,
            ImportoControvaloreChf = importi.ControvaloreChfBanc,
            Patrimonio             = datoConto.Patrimonio,
            CommentoInterno        = command.CommentoInterno,
            ImportoPrecedente      = null
        };

        var risultato = await _is107Service.VerificaLimitiAsync(verificaRequest, ct);

        return risultato.Esito switch
        {
            IS107EsitoVerifica.Ok             => Result.Ok(),
            IS107EsitoVerifica.Block          => Result.Fail(
                new Error(risultato.Messaggio ?? "Limite IS107 superato. Operazione bloccata.")
                    .WithMetadata("ErrorCode", "IS107_BLOCK")),
            IS107EsitoVerifica.Warning        => Result.Fail(
                new Error(risultato.Messaggio ?? "Limite IS107 superato. Inserire commento per procedere.")
                    .WithMetadata("ErrorCode", "IS107_WARNING")
                    .WithMetadata("IS107_SommaTotale", risultato.SommaTotale)
                    .WithMetadata("IS107_Limite", risultato.LimiteConfigurato)),
            IS107EsitoVerifica.ImportoModificato => Result.Fail(
                new Error(risultato.Messaggio ?? "Importo modificato. Selezionare nuovamente Carica.")
                    .WithMetadata("ErrorCode", "IS107_AMOUNT_CHANGED")),
            _ => Result.Ok()
        };
    }

    // ─────────────────────────────────────────────────────────────────────────
    // FASE 10: Validazione vigilanza antiriciclaggio
    // ─────────────────────────────────────────────────────────────────────────

    private async Task<Result<DatiVigilanza?>> VerificaVigilanzaAsync(
        CaricaContiCorrentiCommand command,
        DatiConto datoConto,
        ImportiCalcolati importi,
        CancellationToken ct)
    {
        if (command.DatiAntiRiciclaggio is null)
            return Result.Ok<DatiVigilanza?>(null);

        var isValida = await _vigilanzaRepository.IsVigilanzaValidaAsync(
            command.TipoOperazione,
            "DV",
            importi.ControvaloreChfBanc,
            command.NumeroConto,
            datoConto.Categoria,
            command.DataOperazione.ToString("dd.MM.yyyy"),
            command.DatiAntiRiciclaggio.NomeComparente,
            ct);

        if (!isValida)
            return Result.Fail<DatiVigilanza?>(
                new Error("Dati vigilanza antiriciclaggio non validi o mancanti.")
                    .WithMetadata("ErrorCode", "9024"));

        return Result.Ok<DatiVigilanza?>(new DatiVigilanza(
            NomeComparente:  command.DatiAntiRiciclaggio.NomeComparente,
            DocumentoTipo:   command.DatiAntiRiciclaggio.DocumentoTipo,
            DocumentoNumero: command.DatiAntiRiciclaggio.DocumentoNumero,
            Forzato:         command.DatiAntiRiciclaggio.Forzato));
    }

    // ─────────────────────────────────────────────────────────────────────────
    // FASI 11-12: Preparazione e salvataggio transazione
    // ─────────────────────────────────────────────────────────────────────────

    private async Task<Result<CaricaContiCorrentiResult>> SalvaTransazioneAsync(
        CaricaContiCorrentiCommand command,
        DatiConto datoConto,
        CambioCalcolatoDto cambio,
        ImportiCalcolati importi,
        DatiVigilanza? datiVigilanza,
        CancellationToken ct)
    {
        var isNuovaTransazione = !command.TransactionId.HasValue || command.TransactionId == 0;
        var warnings = new List<string>();
        var benefondoRichiesto = false;

        try
        {
            var sequenzaGiornaliera = await _unitOfWork.ClientRepository
                .GetNextCounterAsync(command.CodiceCassa, ct);

            var transaction = CostruisciTransazione(
                command, datoConto, cambio, importi, sequenzaGiornaliera, isNuovaTransazione);

            // Insert o Update
            if (isNuovaTransazione)
            {
                await _unitOfWork.Repository<Transaction>().AddAsync(transaction, ct);
            }
            else
            {
                transaction.TrxId = command.TransactionId!.Value;
                await _unitOfWork.Repository<Transaction>().UpdateAsync(transaction, ct);
            }

            await _unitOfWork.Complete();

            // Salva dati antiriciclaggio
            if (datiVigilanza is not null)
            {
                await _unitOfWork.VigilanzaSpRepository.SalvaAntiRecyclingAsync(
                    command.UserId,
                    command.CodiceCassa,
                    transaction.TrxId,
                    datiVigilanza.NomeComparente,
                    datiVigilanza.DocumentoTipo,
                    datiVigilanza.DocumentoNumero,
                    ct);
            }

            // Cancella e rigenera movimenti contabili
            await _unitOfWork.VigilanzaSpRepository.DeleteByTrxIdAsync(transaction.TrxId, ct);
            await GeneraMovimentiContabiliAsync(command, transaction, importi, ct);

            // Verifica finale (saldo sufficiente, etc.)
            var verificaFinale = await _unitOfWork.TransactionSpRepository
                .VerificaTransazioneAsync(transaction.TrxId, command.TipoOperazione, ct);

            if (!verificaFinale.Successo)
            {
                // Equivalente a: throw new Exception(pnlOsservazioni.Errore) → rollback
                return Result.Fail(verificaFinale.MessaggioErrore ?? "Errore verifica transazione");
            }
            await _unitOfWork.Complete();

            _logger.LogInformation(
                "Transazione {TrxId} caricata con successo da utente {UserId} su cassa {Cassa}",
                transaction.TrxId, command.UserId, command.CodiceCassa);

            return Result.Ok(CaricaContiCorrentiResult.Success(
                transaction, warnings, benefondoRichiesto));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Errore durante il salvataggio transazione ContiCorrenti per conto {Conto}",
                command.NumeroConto);

            await _unitOfWork.Rollback();
            return Result.Fail<CaricaContiCorrentiResult>(
                new ExceptionalError("Errore interno durante il salvataggio.", ex));
        }
    }

    // ─────────────────────────────────────────────────────────────────────────
    // Generazione movimenti contabili
    // ─────────────────────────────────────────────────────────────────────────

    private async Task GeneraMovimentiContabiliAsync(
        CaricaContiCorrentiCommand command,
        Transaction transaction,
        ImportiCalcolati importi,
        CancellationToken ct)
    {
        var segno = command.TipoOperazione == "DEP" ? 1 : -1;

        // Movimento 1: sul conto del cliente (CTP)
        await _unitOfWork.TransactionMovSpRepository.InsertMovimentoAsync(
            transactionId:  transaction.TrxId,
            tipoMovimento:  "CTP",
            filiale:        transaction.TrxBraId,
            tipoAccount:    transaction.TrxCtoopetp,
            account:        transaction.TrxCtoope,
            divisa:         transaction.TrxDivope,
            importo:        transaction.TrxImpope!.Value * segno,
            importoCtv:     transaction.TrxImpctv!.Value * segno,
            dataValuta:     transaction.TrxDatval!.Value,
            text1:          transaction.TrxText1,
            text2:          transaction.TrxText2,
            codiceCausale:  "TRX",
            hostCod:        string.Empty,
            updatePosition: false,
            ct: ct);

        // Movimento 2: sul conto cassa banconote (CASSA)
        var contoCassa = await _unitOfWork.AccountSpRepository
            .GetContoCassaAsync("CASSA", transaction.TrxBraId, transaction.TrxCassa, transaction.TrxDivctp, TipoBigliettiBank, ct);

        if (contoCassa is null)
            throw new InvalidOperationException(
                $"Nessun conto CASSA trovato per sede {transaction.TrxBraId}, divisa {transaction.TrxDivctp}");

        await _unitOfWork.TransactionMovSpRepository.InsertMovimentoAsync(
            transactionId:  transaction.TrxId,
            tipoMovimento:  "CASSA",
            filiale:        transaction.TrxBraId,
            tipoAccount:    contoCassa.IacCutId,
            account:        contoCassa.IacAccId,
            divisa:         transaction.TrxDivctp,
            importo:        transaction.TrxImpctp!.Value * (segno * -1),
            importoCtv:     transaction.TrxCtpctv!.Value * (segno * -1),
            dataValuta:     transaction.TrxDatval!.Value,
            text1:          transaction.TrxText1,
            text2:          transaction.TrxText2,
            codiceCausale:  "TRX",
            hostCod:        string.Empty,
            updatePosition: false,
            ct: ct);

        // Movimento 3: aggio (solo se presente)
        if (importi.AggioImportoCtv != 0)
        {
            var contoAggio = await _unitOfWork.AccountSpRepository
                .GetContoCassaAsync("AGIO", transaction.TrxBraId, transaction.TrxCassa, DivisaBase, TipoBigliettiBank, ct);

            if (contoAggio is null)
                throw new InvalidOperationException(
                    $"Nessun conto AGIO trovato per sede {transaction.TrxBraId}");

            var segnoAggio = command.TipoOperazione == "WITH" ? segno * -1 : segno;

            await _unitOfWork.TransactionMovSpRepository.InsertMovimentoAsync(
                transactionId:  transaction.TrxId,
                tipoMovimento:  "AGIO",
                filiale:        transaction.TrxBraId,
                tipoAccount:    contoAggio.IacCutId,
                account:        contoAggio.IacAccId,
                divisa:         DivisaBase,
                importo:        importi.AggioImportoCtv * segnoAggio,
                importoCtv:     importi.AggioImportoCtv * segnoAggio,
                dataValuta:     transaction.TrxDatval!.Value,
                text1:          transaction.TrxText1,
                text2:          transaction.TrxText2,
                codiceCausale:  "TRX",
                hostCod:        string.Empty,
                updatePosition: false,
                ct: ct);
        }
    }

    // ─────────────────────────────────────────────────────────────────────────
    // Factory: costruisce l'entità Transaction da salvare
    // ─────────────────────────────────────────────────────────────────────────

    private static Transaction CostruisciTransazione(
        CaricaContiCorrentiCommand cmd,
        DatiConto conto,
        CambioCalcolatoDto cambio,
        ImportiCalcolati importi,
        string sequenzaGiornaliera,
        bool isNuova) => new()
    {
        TrxBraId            = cmd.CodiceFiliale,
        TrxCassa            = cmd.CodiceCassa,
        TrxDatope           = DateTime.Now,
        TrxDailySequence    = sequenzaGiornaliera,
        TrxAptId            = "ACC",
        TrxOptId            = cmd.TipoOperazione,
        TrxCutId            = TipoBigliettiBank,
        TrxCash             = true,
        TrxUsrId            = cmd.UserId,
        TrxReverse          = false,
        TrxRevtrxId         = 0,
        TrxOnline           = true,
        TrxExcrat           = importi.TassoEffettivo,
        TrxExropebas        = 1m,
        TrxExrctpbas        = importi.TassoCtvEffettivo,
        TrxDivope           = NormalizzaDivisa(cmd.DivisaConto),
        TrxImpope           = importi.ImportoConto,
        TrxImpctv           = importi.ControvaloreChfTotale,
        TrxDivctp           = NormalizzaDivisa(cmd.DivisaBanconote),
        TrxImpctp           = importi.ImportoBanconote,
        TrxCtpctv           = importi.ControvaloreChfBanc,
        TrxImpagio          = importi.AggioImporto,
        TrxImpagioctv       = importi.AggioImportoCtv,
        TrxImpcom           = 0,
        TrxImpcomctv        = 0,
        TrxImpspe           = 0,
        TrxImpspectv        = 0,
        TrxImpiva           = 0,
        TrxImpivactv        = 0,
        TrxImpresto         = 0,
        TrxImprestoctv      = 0,
        TrxDatval           = cmd.DataValuta.ToDateTime(TimeOnly.MinValue),
        TrxFlgStampaAvviso  = cmd.StampaAvviso,
        TrxFlgStampaSaldo   = cmd.StampaSaldo,
        TrxFlgStampaIndirizzo = false,
        TrxFlgEmail         = false,
        TrxFinezzaId        = 0,
        TrxText1            = conto.Titolare,
        TrxText2            = string.Empty,
        TrxText3            = conto.Titolare,
        TrxText4            = string.Empty,
        TrxMsgSent          = string.Empty,
        TrxStatus           = 10,  // Stato: caricato, in attesa di conferma
        TrxCtoope           = conto.Account,
        TrxCtoopetp         = conto.TipoConto,
        TrxCtoctp           = string.Empty,
        TrxCtoctptip        = string.Empty,
        TrxMetnet           = 0,
        TrxPrcmetope        = 0,
        TrxPrcmetctp        = 0,
        TrxIvaper           = importi.AggioPercent,
        TrxExcratPrt        = importi.TassoEffettivo,
        TrxBefhost          = string.Empty,
        TrxBefstatus        = 0,
        TrxSaldo            = cmd.TipoOperazione == "DEP"
                                ? conto.Saldo + importi.ImportoConto
                                : conto.Saldo - importi.ImportoConto,
        TrxRubrica          = conto.Titolare,
        TrxAssegno          = string.Empty,
        TrxCencos           = string.Empty,
        TrxFlgForced        = cmd.ForzaCambio,
        TrxIntComment       = cmd.CommentoInterno ?? string.Empty,
        TrxFlgPrinted       = false,
        TrxFlgIs107Customer = false,
        TrxFlgIs107Overlimit = false,
        TrxNumrel           = string.Empty
    };

    // ─────────────────────────────────────────────────────────────────────────
    // Helpers matematici puri (stateless, testabili)
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Calcola l'importo controparte (banconote) dall'importo conto,
    /// applicando il cambio nella direzione corretta.
    /// </summary>
    private static decimal CalcolaControparte(
        decimal importo, decimal tasso,
        decimal scala1, decimal scala2, bool direzione)
    {
        var fattore = tasso / (scala1 / scala2);
        return direzione ? importo * fattore : importo / fattore;
    }

    /// <summary>Ricalcola l'importo conto dall'importo banconote arrotondato.</summary>
    private static decimal RicalcolaImportoConto(
        decimal importoBanconote, decimal tasso,
        decimal scala1, decimal scala2, bool direzione)
    {
        var fattore = tasso / (scala1 / scala2);
        return direzione ? importoBanconote / fattore : importoBanconote * fattore;
    }

    /// <summary>
    /// Calcola aggio/disaggio e relativo controvalore CHF.
    /// Restituisce (percentuale, importo in divisa banconote, importo in CHF).
    /// </summary>
    private static (decimal AggioPer, decimal AggioImporto, decimal AggioCtv) CalcolaAggio(
        CaricaContiCorrentiCommand cmd,
        decimal importoBanconote,
        decimal tassoCtvEffettivo,
        decimal scalaDivisa2)
    {
        if (!cmd.Aggio.HasValue || cmd.Aggio == 0)
            return (0m, 0m, 0m);

        decimal aggioPer = cmd.TipoAggioIsPercentuale
            ? cmd.Aggio.Value
            : cmd.Aggio.Value / importoBanconote * 100m;

        aggioPer *= cmd.SegnoAggio;

        var aggioImporto = importoBanconote * aggioPer / 100m;
        var aggioCtv     = Math.Round(aggioImporto * tassoCtvEffettivo / scalaDivisa2, 2);

        return (aggioPer, aggioImporto, aggioCtv);
    }

    /// <summary>
    /// Calcola l'importo finale da addebitare/accreditare sul conto,
    /// convertendo dai banconote al cambio e aggiungendo/sottraendo l'aggio.
    /// </summary>
    private static decimal CalcolaImportoContoConAggio(
        string tipoOperazione, decimal importoConto, decimal aggioImporto,
        decimal tasso, decimal scala1, decimal scala2, bool direzione,
        string divisaConto)
    {
        var totale = tipoOperazione == "WITH"
            ? importoConto + aggioImporto
            : importoConto - aggioImporto;

        // Riconversione in divisa conto se cross
        if (divisaConto != DivisaBase)
        {
            var fattore = tasso / (scala1 / scala2);
            totale = direzione ? totale / fattore : totale * fattore;
        }

        return Math.Round(totale, 2);
    }

    private static string NormalizzaDivisa(string divisa) =>
        divisa.Trim()[..Math.Min(3, divisa.Trim().Length)].ToUpperInvariant();
}

// ─────────────────────────────────────────────────────────────────────────────
// Record interni per passaggio dati tra i passi dell'handler (private)
// ─────────────────────────────────────────────────────────────────────────────

internal sealed record DatiConto(
    string Account,
    string Titolare,
    string Divisa,
    decimal Saldo,
    string TipoConto,
    string Categoria,
    bool IsDipendente,
    string IS107,
    string NumeroRel,
    decimal Patrimonio);

internal sealed record ImportiCalcolati(
    decimal ImportoConto,
    decimal ImportoBanconote,
    decimal ControvaloreChfBanc,
    decimal ControvaloreChfTotale,
    decimal AggioPercent,
    decimal AggioImporto,
    decimal AggioImportoCtv,
    decimal TassoEffettivo,
    decimal TassoCtvEffettivo);

internal sealed record DatiVigilanza(
    string NomeComparente,
    string DocumentoTipo,
    string DocumentoNumero,
    bool Forzato);
