using MediatR;
using MultiTenantPolicyApi.Application.Common.Interfaces;
using MultiTenantPolicyApi.Application.Common.Models;

namespace MultiTenantPolicyApi.Application.Features.Policies.Queries.GetCustomerPolicy;

public sealed class GetCustomerPolicyQueryHandler : IRequestHandler<GetCustomerPolicyQuery, PolicyDto?>
{
    private readonly IPolicyRepository _policyRepository;

    public GetCustomerPolicyQueryHandler(IPolicyRepository policyRepository)
    {
        _policyRepository = policyRepository;
    }

    public Task<PolicyDto?> Handle(GetCustomerPolicyQuery request, CancellationToken cancellationToken)
    {
        return _policyRepository.GetByCustomerIdAsync(request.CustomerId, cancellationToken);
    }
}
