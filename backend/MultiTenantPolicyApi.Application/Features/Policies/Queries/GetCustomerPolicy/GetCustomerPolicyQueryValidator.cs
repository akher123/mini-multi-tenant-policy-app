using FluentValidation;

namespace MultiTenantPolicyApi.Application.Features.Policies.Queries.GetCustomerPolicy;

public sealed class GetCustomerPolicyQueryValidator : AbstractValidator<GetCustomerPolicyQuery>
{
    public GetCustomerPolicyQueryValidator()
    {
        RuleFor(x => x.CustomerId).NotEmpty();
    }
}
