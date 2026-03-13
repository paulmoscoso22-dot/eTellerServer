using eTeller.Application.Contracts;
using eTeller.Application.Contracts.StoreProcedures;
using eTeller.Application.DTOs;
using eTeller.Domain.Models;
using eTeller.Infrastructure.Context;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Dapper;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace eTeller.Infrastructure.Repositories.Archivi.Report.StoreProcedure
{
    public class TransactionSpRepository : BaseSimpleRepository<Transaction>, ITransactionSpRepository
    {
        private readonly ILogger<TransactionSpRepository> _logger;
        private readonly IHostCommunicationService _hostCommunicationService;
        private readonly IErrorCodeRepository _errorCodeRepository;

        static TransactionSpRepository()
        {
            // Configura Dapper per il mapping usando gli attributi [Column()]
            SqlMapper.SetTypeMap(typeof(Transaction), new CustomPropertyTypeMap(
                typeof(Transaction),
                (type, columnName) =>
                {
                    var properties = type.GetProperties();
                    
                    // Primo: cerca mappando gli attributi [Column()] al nome della colonna SQL
                    var property = properties.FirstOrDefault(p =>
                    {
                        var columnAttr = p.GetCustomAttribute<ColumnAttribute>();
                        return columnAttr?.Name?.Equals(columnName, StringComparison.OrdinalIgnoreCase) == true;
                    });
                    
                    // Se non trovato per attributo, prova il match case-insensitive sul nome della proprietà
                    if (property == null)
                    {
                        property = properties.FirstOrDefault(p => 
                            p.Name.Equals(columnName, StringComparison.OrdinalIgnoreCase));
                    }
                    
                    return property!;  // Non-null forgiving operator (property mapping fallback)
                }));
        }

        public TransactionSpRepository(
            eTellerDbContext dbContext,
            ILogger<TransactionSpRepository> logger,
            IHostCommunicationService hostCommunicationService,
            IErrorCodeRepository errorCodeRepository) : base(dbContext)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _hostCommunicationService = hostCommunicationService ?? throw new ArgumentNullException(nameof(hostCommunicationService));
            _errorCodeRepository = errorCodeRepository ?? throw new ArgumentNullException(nameof(errorCodeRepository));
        }

        public async Task<IEnumerable<Transaction>> GetSpTransactionWaitingForBef(string trxCassa, DateTime trxDataDal, DateTime trxDataAl, int trxStatus, string trxBraId)
        {
            return await ExecuteStoredProcedure("dbo.sp_Transaction_SelectWaitingForBEF", 
                trxCassa, trxDataDal, trxDataAl, trxStatus, trxBraId);
        }

        public async Task<IEnumerable<Transaction>> GetSpTransactionWithFiltersForGiornale(string trxCassa, DateTime trxDataDal, DateTime trxDataAl, int trxStatus, string trxBraId)
        {
            return await ExecuteStoredProcedure("dbo.sp_Transaction_SelectWithFiltersForGiornale",
                trxCassa, trxDataDal, trxDataAl, trxStatus, trxBraId);
        }

        public async Task<IEnumerable<Transaction>> GetSpTransactionWithFilters(string trxCassa, DateTime trxDataDal, DateTime trxDataAl, int trxStatus, string trxBraId)
        {
            return await ExecuteStoredProcedure("dbo.sp_Transaction_SelectWaitingForBEF", 
                trxCassa, trxDataDal, trxDataAl, trxStatus, trxBraId);
        }

        private async Task<IEnumerable<Transaction>> ExecuteStoredProcedure(string procedureName, 
            string trxCassa, DateTime trxDataDal, DateTime trxDataAl, int trxStatus, string trxBraId)
        {
            using (var connection = new SqlConnection(_context.Database.GetConnectionString()))
            {
                await connection.OpenAsync();

                var parameters = new
                {
                    TRX_CASSA = string.IsNullOrEmpty(trxCassa) ? null : trxCassa,
                    TRX_DATADAL = trxDataDal,
                    TRX_DATAAL = trxDataAl,
                    TRX_STATUS = trxStatus,
                    TRX_BRA_ID = string.IsNullOrEmpty(trxBraId) ? null : trxBraId
                };

                var result = await connection.QueryAsync<Transaction>(
                    procedureName,
                    parameters,
                    commandType: System.Data.CommandType.StoredProcedure);

                return result;
            }
        }

        /// <summary>
        /// Verifica la validità della transazione presso l'host.
        /// Equivalente a: oe.Verifica(trxId, tipoOperazione, ..., out MsgError, out BenefondoRichiesto)
        /// 
        /// Flusso:
        /// 1. Cambia stato a 10 (in verifica)
        /// 2. Legge transazione e movimenti
        /// 3. Controlla se sistema è online
        /// 4. Invia messaggio di verifica all'host (TipoElaborazione.Verify)
        /// 5. Gestisce errori del socket
        /// 6. Controlla se beneficiario è obbligatorio (solo prelievi)
        /// 7. Cambia stato a 20 se OK
        /// </summary>
        public async Task<VerificaTransazioneResult> VerificaTransazioneAsync(
            int trxId, 
            string tipoOperazione,
            CancellationToken ct = default)
        {
            try
            {
                _logger.LogDebug("VerificaTransazione iniciata per TRX_ID={trxId}, TipoOp={tipoOperazione}", 
                    trxId, tipoOperazione);

                // STEP 1 — Cambia stato a 10 (in verifica)
                await ChangeStatusAsync(trxId, 10, ct);
                _logger.LogDebug("Stato transazione {trxId} cambiato a 10 (in verifica)", trxId);

                // STEP 2 — Legge la transazione + movimenti
                var trx = await _context.Transaction
                    .AsNoTracking()
                    .FirstOrDefaultAsync(t => t.TrxId == trxId, ct)
                    ?? throw new InvalidOperationException($"Transazione {trxId} non trovata");

                var movimenti = await _context.TransactionMov
                    .AsNoTracking()
                    .Where(m => m.TrmTrxId == trxId)
                    .ToListAsync(ct);

                _logger.LogDebug("Transazione {trxId} caricata: {importo}, movimenti: {count}", 
                    trxId, trx.TrxImpope, movimenti.Count);

                // STEP 3 — Controlla se sistema è online
                var isOnline = await IsOnlineAsync(ct);
                if (!isOnline)
                {
                    _logger.LogInformation("Sistema OFFLINE - verifica completata in modalità offline per TRX_ID={trxId}", trxId);
                    // Offline: stato rimane 10, nessun errore bloccante
                    return new VerificaTransazioneResult(
                        Successo: true,
                        BenefondoRichiesto: false,
                        MessaggioErrore: null);
                }

                // STEP 4 — Costruisce e invia messaggio host (TipoElaborazione.Verify)
                _logger.LogDebug("Invio messaggio di verifica all'host per TRX_ID={trxId}", trxId);
                var hostResult = await _hostCommunicationService
                    .SendVerifyAsync(trxId, tipoOperazione, trx, movimenti, ct);

                if (!hostResult.SocketOk)
                {
                    // Errore socket → legge da ErrorCodeDB
                    var errMsg = !string.IsNullOrEmpty(hostResult.ErrorCode)
                        ? await _errorCodeRepository
                            .GetDescriptionAsync(hostResult.ErrorCode, "IT", ct)
                        : null;

                    errMsg ??= $"Errore di comunicazione con l'host";

                    _logger.LogWarning(
                        "Errore socket nella verifica TRX_ID={trxId}: ErrorCode={errorCode}, Message={message}",
                        trxId, hostResult.ErrorCode ?? "N/A", errMsg);

                    return new VerificaTransazioneResult(
                        Successo: false,
                        BenefondoRichiesto: false,
                        MessaggioErrore: errMsg);
                }

                // STEP 5 — Legge ReturnArea (campi ArrMsg[6..10])
                if (hostResult.HasErrors) // Tipo E, S o C
                {
                    _logger.LogWarning(
                        "Host ha ritornato errori per TRX_ID={trxId}: {errorText}",
                        trxId, hostResult.ErrorText);

                    await ChangeStatusAsync(trxId, 10, ct);
                    return new VerificaTransazioneResult(
                        Successo: false,
                        BenefondoRichiesto: false,
                        MessaggioErrore: hostResult.ErrorText);
                }

                // STEP 6 — Controlla benefondo (solo per prelievi WITH)
                if (tipoOperazione == "WITH" && hostResult.HasBenefondo)
                {
                    _logger.LogInformation(
                        "Host richiede beneficiario per prelievo TRX_ID={trxId}",
                        trxId);

                    // stato rimane 10
                    await ChangeStatusAsync(trxId, 10, ct);
                    return new VerificaTransazioneResult(
                        Successo: true,
                        BenefondoRichiesto: true,
                        MessaggioErrore: null);
                }

                // STEP 7 — OK → stato 20
                _logger.LogInformation("Verifica completata con successo per TRX_ID={trxId}", trxId);
                await ChangeStatusAsync(trxId, 20, ct);
                return new VerificaTransazioneResult(
                    Successo: true,
                    BenefondoRichiesto: false,
                    MessaggioErrore: null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante VerificaTransazione per TRX_ID={trxId}", trxId);
                throw;
            }
        }

        /// <summary>
        /// Cambia lo stato della transazione tramite stored procedure.
        /// SP: dbo.sp_Transaction_ChangeStatus
        /// </summary>
        private async Task ChangeStatusAsync(int trxId, int status, CancellationToken ct)
        {
            await _context.Database.ExecuteSqlRawAsync(
                "EXEC dbo.sp_Transaction_ChangeStatus @TRX_ID = {0}, @TRX_STATUS = {1}",
                new object[] { trxId, status },
                ct);

            _logger.LogDebug("Status transazione {trxId} aggiornato a {status}", trxId, status);
        }

        /// <summary>
        /// Controlla se il sistema è connesso e online (può contattare l'host).
        /// </summary>
        private async Task<bool> IsOnlineAsync(CancellationToken ct)
        {
            try
            {
                return await _hostCommunicationService.IsOnlineAsync(ct);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Errore durante il controllo dello stato online");
                return false;
            }
        }
    }
}

