using MediatR;
using MultiTenantPolicyApi.Application.Common.Interfaces;
using MultiTenantPolicyApi.Application.Common.Models;

namespace MultiTenantPolicyApi.Application.Features.Policies.Queries.GetPolicyExpiration;

public sealed class GetPolicyExpirationQueryHandler : IRequestHandler<GetPolicyExpirationQuery, PolicyExpirationDto?>
{
    private readonly IPolicyRepository _policyRepository;

    public GetPolicyExpirationQueryHandler(IPolicyRepository policyRepository)
    {
        _policyRepository = policyRepository;
    }

    public Task<PolicyExpirationDto?> Handle(GetPolicyExpirationQuery request, CancellationToken cancellationToken)
    {
        return _policyRepository.GetExpirationByPolicyNumberAsync(request.PolicyNumber, cancellationToken);
    }
}
