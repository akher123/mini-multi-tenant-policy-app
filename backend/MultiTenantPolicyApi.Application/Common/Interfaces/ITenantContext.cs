namespace MultiTenantPolicyApi.Application.Common.Interfaces;

public interface ITenantContext
{
    Guid TenantId { get; }
    bool IsAuthenticated { get; }
}
