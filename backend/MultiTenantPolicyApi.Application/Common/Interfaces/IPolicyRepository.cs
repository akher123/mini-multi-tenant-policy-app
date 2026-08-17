using MultiTenantPolicyApi.Application.Common.Models;

namespace MultiTenantPolicyApi.Application.Common.Interfaces;

public interface IPolicyRepository
{
    Task<PolicyDto?> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ExpiringPolicyDto>> GetExpiringAsync(int withinDays, CancellationToken cancellationToken = default);
    Task<PolicyExpirationDto?> GetExpirationByPolicyNumberAsync(string policyNumber, CancellationToken cancellationToken = default);
}
