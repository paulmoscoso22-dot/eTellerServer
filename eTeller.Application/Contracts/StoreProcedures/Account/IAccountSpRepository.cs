using eTeller.Domain.Models;
using eTeller.Domain.Models.View;
using static eTeller.Application.Contracts.Commons.IBaseSimpleRepository;
using CustomerAccountModel = eTeller.Domain.Models.CustomerAccount;

namespace eTeller.Application.Contracts.StoreProcedures
{
    public interface IAccountSpRepository : IBaseSimpleRepository<Account>
    {
        Task<IEnumerable<Account>> GetAccountAsync();

        Task<Account?> GetContoCassaAsync(string accType, string branch, string cliId, string currency, string currencyType, CancellationToken cancellationToken = default);

        Task<IEnumerable<Account>> GetAccountByIacId(int iacId);

        Task<IEnumerable<Account>> GetAccountByPara(string iacAccId, string iacCutId, string iacCurId, string iacDes, string iacActId, string iacCliCassa, string iacBraId);

        Task<IEnumerable<Account>> GetAccountForBalance(string clientId);

        Task<int> GetAccountForCheck(string iacCutId, string iacCurId, string iacActId, string iacCliCassa);

        Task<int> GetAccountMaxIacId();

        Task<int> UpdateAccount(int iacId, string iacAccId, string iacCutId, string iacCurId, string iacDes, string iacActId, string iacCliCassa, string iacBraId, string iacHostprefix);

        Task<CustomerAccountModel?> GetAccountInfoAsync(string accId);

        Task<bool> UsaSpreadAsync(string accountId, string categoryId, CancellationToken cancellationToken);
    }
}
