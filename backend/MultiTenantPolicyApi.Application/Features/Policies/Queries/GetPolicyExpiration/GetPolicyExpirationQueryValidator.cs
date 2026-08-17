using FluentValidation;

namespace MultiTenantPolicyApi.Application.Features.Policies.Queries.GetPolicyExpiration;

public sealed class GetPolicyExpirationQueryValidator : AbstractValidator<GetPolicyExpirationQuery>
{
    public GetPolicyExpirationQueryValidator()
    {
        RuleFor(x => x.PolicyNumber).NotEmpty();
    }
}
