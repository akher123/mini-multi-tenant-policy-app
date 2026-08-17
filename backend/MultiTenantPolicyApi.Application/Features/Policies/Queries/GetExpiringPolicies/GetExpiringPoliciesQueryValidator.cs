using FluentValidation;

namespace MultiTenantPolicyApi.Application.Features.Policies.Queries.GetExpiringPolicies;

public sealed class GetExpiringPoliciesQueryValidator : AbstractValidator<GetExpiringPoliciesQuery>
{
    public GetExpiringPoliciesQueryValidator()
    {
        RuleFor(x => x.WithinDays).InclusiveBetween(1, 365);
    }
}
