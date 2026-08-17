using MediatR;
using MultiTenantPolicyApi.Application.Common.Interfaces;
using MultiTenantPolicyApi.Application.Common.Models;

namespace MultiTenantPolicyApi.Application.Features.Policies.Queries.GetExpiringPolicies;

public sealed class GetExpiringPoliciesQueryHandler : IRequestHandler<GetExpiringPoliciesQuery, IReadOnlyList<ExpiringPolicyDto>>
{
    private readonly IPolicyRepository _policyRepository;

    public GetExpiringPoliciesQueryHandler(IPolicyRepository policyRepository)
    {
        _policyRepository = policyRepository;
    }

    public Task<IReadOnlyList<ExpiringPolicyDto>> Handle(GetExpiringPoliciesQuery request, CancellationToken cancellationToken)
    {
        return _policyRepository.GetExpiringAsync(request.WithinDays, cancellationToken);
    }
}
