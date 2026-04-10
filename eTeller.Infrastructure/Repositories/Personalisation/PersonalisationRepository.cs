using eTeller.Application.Contracts.Personalisation;
using eTeller.Application.Contracts.Trace;
using eTeller.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace eTeller.Infrastructure.Repositories.StoreProcedures.Personalisation
{
    public class PersonalisationRepository : BaseSimpleRepository<eTeller.Domain.Models.Personalisation>, IPersonalisationRepository
    {
        private readonly ITraceRepository _traceRepository;

        public PersonalisationRepository(eTellerDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<eTeller.Domain.Models.Personalisation?> PersonalisationUpdateAsync(string parId, string parDes, string parValue, string originalParId)
        {
            var result = await _context.Personalisation
                 .FromSqlInterpolated($@"
                                EXEC dbo.sp_Personalization_Update
                                    @PAR_ID = {parId},
                                    @PAR_DES = {parDes},
                                    @PAR_Value = {parValue},
                                    @Original_PAR_ID = {originalParId}
                            ")
                 .AsNoTracking()
                 .ToListAsync();

            return result.FirstOrDefault();
        }
    }
}
