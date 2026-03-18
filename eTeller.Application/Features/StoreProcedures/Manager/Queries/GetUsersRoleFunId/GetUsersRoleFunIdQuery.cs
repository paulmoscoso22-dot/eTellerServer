using MediatR;
using System.Collections.Generic;
using eTeller.Application.Mappings.Manager;

namespace eTeller.Application.Features.StoreProcedures.Manager.Queries.GetUsersRoleFunId
{
    public class GetUsersRoleFunIdQuery : IRequest<IEnumerable<UsersRoleFunctionVm>>
    {
        public int FunId { get; set; }
    }
}
