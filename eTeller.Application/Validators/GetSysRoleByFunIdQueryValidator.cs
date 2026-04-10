using FluentValidation;
using eTeller.Application.Features.Manager.Queries.Roles.GetSysRoleByFunId;

namespace eTeller.Application.Validators
{
    public class GetSysRoleByFunIdQueryValidator : AbstractValidator<GetSysRoleByFunIdQuery>
    {
        public GetSysRoleByFunIdQueryValidator()
        {
            RuleFor(x => x.FunId)
                .GreaterThan(0)
                .WithErrorCode("1305");
        }
    }
}
