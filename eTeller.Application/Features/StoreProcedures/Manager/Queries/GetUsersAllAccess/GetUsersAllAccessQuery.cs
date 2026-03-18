using MediatR;
using eTeller.Application.Mappings.Manager;

namespace eTeller.Application.Features.StoreProcedures.Manager.Queries.GetUsersAllAccess
{
    public class GetUsersAllAccessQuery : IRequest<IEnumerable<USERS_AllAccessVm>>
    {
        public string UsrId { get; set; }
        public string? FunlikeName { get; set; }
        public string? FunlikeDes { get; set; }
        public bool Tutti { get; set; }

        public GetUsersAllAccessQuery(string usrId, string? funlikeName = null, string? funlikeDes = null, bool tutti = false)
        {
            UsrId = usrId;
            FunlikeName = funlikeName;
            FunlikeDes = funlikeDes;
            Tutti = tutti;
        }
    }
}
