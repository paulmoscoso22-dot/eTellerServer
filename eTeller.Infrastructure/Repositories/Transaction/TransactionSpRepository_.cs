using eTeller.Application.Contracts;
using eTeller.Application.DTOs;
using eTeller.Infrastructure.Context;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Dapper;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

//namespace eTeller.Infrastructure.Repositories.StoreProcedures.Transaction
//{
//    public class TransactionSpRepository : BaseSimpleRepository<Domain.Models.Transaction>, ITransactionRepository
//    {
//        private readonly ILogger<TransactionSpRepository> _logger;
//        private readonly IHostCommunicationService _hostCommunicationService;
//        private readonly IErrorCodeRepository _errorCodeRepository;

//        static TransactionSpRepository()
//        {
//            SqlMapper.SetTypeMap(typeof(Domain.Models.Transaction), new CustomPropertyTypeMap(
//                typeof(Domain.Models.Transaction),
//                (type, columnName) =>
//                {
//                    var properties = type.GetProperties();
//                    var property = properties.FirstOrDefault(p =>
//                    {
//                        var columnAttr = p.GetCustomAttribute<ColumnAttribute>();
//                        return columnAttr?.Name?.Equals(columnName, StringComparison.OrdinalIgnoreCase) == true;
//                    });
//                    if (property == null)
//                    {
//                        property = properties.FirstOrDefault(p => 
//                            p.Name.Equals(columnName, StringComparison.OrdinalIgnoreCase));
//                    }
//                    return property!;
//                }));
//        }

//        public TransactionSpRepository(
//            eTellerDbContext dbContext,
//            ILogger<TransactionSpRepository> logger,
//            IHostCommunicationService hostCommunicationService,
//            IErrorCodeRepository errorCodeRepository) : base(dbContext)
//        {
//            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
//            _hostCommunicationService = hostCommunicationService ?? throw new ArgumentNullException(nameof(hostCommunicationService));
//            _errorCodeRepository = errorCodeRepository ?? throw new ArgumentNullException(nameof(errorCodeRepository));
//        }

//        public async Task<IEnumerable<Domain.Models.Transaction>> GetSpTransactionWaitingForBef(string trxCassa, DateTime trxDataDal, DateTime trxDataAl, int trxStatus, string trxBraId)
//        {
//            return await ExecuteStoredProcedure("dbo.sp_Transaction_SelectWaitingForBEF", 
//                trxCassa, trxDataDal, trxDataAl, trxStatus, trxBraId);
//        }

//        public async Task<IEnumerable<Domain.Models.Transaction>> GetSpTransactionWithFiltersForGiornale(string trxCassa, DateTime trxDataDal, DateTime trxDataAl, int trxStatus, string trxBraId)
//        {
//            return await ExecuteStoredProcedure("dbo.sp_Transaction_SelectWithFiltersForGiornale",
//                trxCassa, trxDataDal, trxDataAl, trxStatus, trxBraId);
//        }

//        public async Task<IEnumerable<Domain.Models.Transaction>> GetSpTransactionWithFilters(string trxCassa, DateTime trxDataDal, DateTime trxDataAl, int trxStatus, string trxBraId)
//        {
//            return await ExecuteStoredProcedure("dbo.sp_Transaction_SelectWithFilters", 
//                trxCassa, trxDataDal, trxDataAl, trxStatus, trxBraId);
//        }

//        private async Task<IEnumerable<Domain.Models.Transaction>> ExecuteStoredProcedure(string procedureName, 
//            string trxCassa, DateTime trxDataDal, DateTime trxDataAl, int trxStatus, string trxBraId)
//        {
//            using (var connection = new SqlConnection(_context.Database.GetConnectionString()))
//            {
//                await connection.OpenAsync();
//                var parameters = new
//                {
//                    TRX_CASSA = string.IsNullOrEmpty(trxCassa) ? null : trxCassa,
//                    TRX_DATADAL = trxDataDal,
//                    TRX_DATAAL = trxDataAl,
//                    TRX_STATUS = trxStatus,
//                    TRX_BRA_ID = string.IsNullOrEmpty(trxBraId) ? null : trxBraId
//                };
//                var result = await connection.QueryAsync<Domain.Models.Transaction>(
//                    procedureName, parameters,
//                    commandType: System.Data.CommandType.StoredProcedure);
//                return result;
//            }
//        }

//        public async Task<VerificaTransazioneResult> VerificaTransazioneAsync(int trxId, string tipoOperazione, CancellationToken ct = default)
//        {
//            try
//            {
//                _logger.LogDebug("VerificaTransazione iniciata per TRX_ID={trxId}, TipoOp={tipoOperazione}", trxId, tipoOperazione);
//                await ChangeStatusAsync(trxId, 10, ct);
//                _logger.LogDebug("Stato transazione {trxId} cambiato a 10 (in verifica)", trxId);

//                var trx = await _context.Transaction
//                    .AsNoTracking()
//                    .FirstOrDefaultAsync(t => t.TrxId == trxId, ct)
//                    ?? throw new InvalidOperationException($"Transazione {trxId} non trovata");

//                var movimenti = await _context.TransactionMov
//                    .AsNoTracking()
//                    .Where(m => m.TrmTrxId == trxId)
//                    .ToListAsync(ct);

//                _logger.LogDebug("Transazione {trxId} caricata: {importo}, movimenti: {count}", trxId, trx.TrxImpope, movimenti.Count);

//                var isOnline = await IsOnlineAsync(ct);
//                if (!isOnline)
//                {
//                    _logger.LogInformation("Sistema OFFLINE - verifica completata in modalità offline per TRX_ID={trxId}", trxId);
//                    return new VerificaTransazioneResult(Successo: true, BenefondoRichiesto: false, MessaggioErrore: null);
//                }

//                _logger.LogDebug("Invio messaggio di verifica all'host per TRX_ID={trxId}", trxId);
//                var hostResult = await _hostCommunicationService.SendVerifyAsync(trxId, tipoOperazione, trx, movimenti, ct);

//                if (!hostResult.SocketOk)
//                {
//                    var errMsg = !string.IsNullOrEmpty(hostResult.ErrorCode)
//                        ? await _errorCodeRepository.GetDescriptionAsync(hostResult.ErrorCode, "IT", ct)
//                        : null;
//                    errMsg ??= $"Errore di comunicazione con l'host";
//                    _logger.LogWarning("Errore socket nella verifica TRX_ID={trxId}: ErrorCode={errorCode}, Message={message}", trxId, hostResult.ErrorCode ?? "N/A", errMsg);
//                    return new VerificaTransazioneResult(Successo: false, BenefondoRichiesto: false, MessaggioErrore: errMsg);
//                }

//                if (hostResult.HasErrors)
//                {
//                    _logger.LogWarning("Host ha ritornato errori per TRX_ID={trxId}: {errorText}", trxId, hostResult.ErrorText);
//                    await ChangeStatusAsync(trxId, 10, ct);
//                    return new VerificaTransazioneResult(Successo: false, BenefondoRichiesto: false, MessaggioErrore: hostResult.ErrorText);
//                }

//                if (tipoOperazione == "WITH" && hostResult.HasBenefondo)
//                {
//                    _logger.LogInformation("Host richiede beneficiario per prelievo TRX_ID={trxId}", trxId);
//                    await ChangeStatusAsync(trxId, 10, ct);
//                    return new VerificaTransazioneResult(Successo: true, BenefondoRichiesto: true, MessaggioErrore: null);
//                }

//                _logger.LogInformation("Verifica completata con successo per TRX_ID={trxId}", trxId);
//                await ChangeStatusAsync(trxId, 20, ct);
//                return new VerificaTransazioneResult(Successo: true, BenefondoRichiesto: false, MessaggioErrore: null);
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Errore durante VerificaTransazione per TRX_ID={trxId}", trxId);
//                throw;
//            }
//        }

//        private async Task ChangeStatusAsync(int trxId, int status, CancellationToken ct)
//        {
//            await _context.Database.ExecuteSqlRawAsync(
//                "EXEC dbo.sp_Transaction_ChangeStatus @TRX_ID = {0}, @TRX_STATUS = {1}",
//                new object[] { trxId, status }, ct);
//            _logger.LogDebug("Status transazione {trxId} aggiornato a {status}", trxId, status);
//        }

//        private async Task<bool> IsOnlineAsync(CancellationToken ct)
//        {
//            try { return await _hostCommunicationService.IsOnlineAsync(ct); }
//            catch (Exception ex) { _logger.LogWarning(ex, "Errore durante il controllo dello stato online"); return false; }
//        }
//    }
//}