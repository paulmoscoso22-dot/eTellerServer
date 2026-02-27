using eTeller.Application.Mappings.Transaction;
using MediatR;

namespace eTeller.Application.Features.Archivi.Report.StoreProcedure.Queries.GetOperazioniAnnullate
{
    public record GetSpOperazioniAnnullateQuery(
        string trxCassa,
        DateTime trxDataDal,
        DateTime trxDataAl,
        int trxStatus,
        string trxBraId
    ) : IRequest<IEnumerable<TransactionOperationAnnullateVm>>;
}
