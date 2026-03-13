using eTeller.Domain.Models;
using static eTeller.Application.Contracts.Commons.IBaseSimpleRepository;

namespace eTeller.Application.Contracts
{
    public interface IClientRepository : IBaseSimpleRepository<Client>
    {
        Task<Client?> WhoIsLogged(string ip);
        
        /// <summary>
        /// Ottiene il prossimo numero di sequenza giornaliera per una cassa.
        /// Utilizza la procedura memorizzata sys_CLIENT_getNextCountID.
        /// Se è il primo giorno, restituisce "1", altrimenti restituisce il contatore corrente.
        /// </summary>
        /// <param name="clientId">Codice della cassa (CLI_ID, max 3 caratteri)</param>
        /// <param name="cancellationToken">Token di cancellazione</param>
        /// <returns>Numero sequenziale giornaliero come stringa</returns>
        Task<string> GetNextCounterAsync(string clientId, CancellationToken cancellationToken = default);
    }
}
