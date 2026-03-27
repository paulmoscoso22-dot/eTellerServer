using eTeller.Application.Mappings.Manager;
using MediatR;

namespace eTeller.Application.Features.StoreProcedures.Manager.Queries.GetAllUsersByUsrId
{
    public class GetAllUsersByUsrIdQuery : IRequest<IEnumerable<InfoAutorizzazioneUtenteVm>>
    {
        public string UsrId { get; set; }
        public string? FunlikeName { get; set; }
        public string? FunlikeDes { get; set; }
        public bool Tutti { get; set; }

        public GetAllUsersByUsrIdQuery(string usrId, string? funlikeName = null, string? funlikeDes = null, bool tutti = false)
        {
            UsrId = usrId;
            FunlikeName = funlikeName;
            FunlikeDes = funlikeDes;
            Tutti = tutti;
        }
    }
}
