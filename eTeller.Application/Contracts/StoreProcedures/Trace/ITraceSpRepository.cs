using static eTeller.Application.Contracts.Commons.IBaseSimpleRepository;

namespace eTeller.Application.Contracts.StoreProcedures.Trace
{
    public interface ITraceSpRepository : IBaseSimpleRepository<eTeller.Domain.Models.Trace>
    {
        Task<int> InsertTrace(
            DateTime traTime,
            string traUser,
            string traFunCode,
            string? traSubFun,
            string traStation,
            string traTabNam,
            string traEntCode,
            string? traRevTrxTrace,
            string? traDes,
            string? traExtRef,
            bool traError);

        Task<IEnumerable<Domain.Models.Trace>> GetTraceAll(
            string? traUser,
            string? traFunCode,
            string? traStation,
            string? traTabNam,
            string? traEntCode,
            bool? traError,
            DateTime? dataFrom,
            DateTime? dataTo);

        //Task<IEnumerable<eTeller.Application.Mappings.Trace.TraceWithFunctionVm>> GetTraceWithFunctionAsync(
        //    string? traUser,
        //    string? traFunCode,
        //    string? traStation,
        //    string? traTabNam,
        //    string? traEntCode,
        //    bool? traError,
        //    DateTime? dataFrom,
        //    DateTime? dataTo);
    }
}
