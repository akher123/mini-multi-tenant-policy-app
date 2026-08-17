using MultiTenantPolicyApi.Application.Common.Interfaces;

namespace MultiTenantPolicyApi.Tests;

public sealed class StubTenantContext : ITenantContext
{
    public StubTenantContext(Guid tenantId, bool isAuthenticated = true)
    {
        TenantId = tenantId;
        IsAuthenticated = isAuthenticated;
    }

    public Guid TenantId { get; }
    public bool IsAuthenticated { get; }
}
