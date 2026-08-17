namespace MultiTenantPolicyApi.Domain.Entities;

public class Policy
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public Guid CustomerId { get; set; }
    public string PolicyNumber { get; set; } = string.Empty;
    public DateOnly ExpirationDate { get; set; }
    public decimal PremiumAmount { get; set; }

    public Tenant Tenant { get; set; } = null!;
    public Customer Customer { get; set; } = null!;
}
