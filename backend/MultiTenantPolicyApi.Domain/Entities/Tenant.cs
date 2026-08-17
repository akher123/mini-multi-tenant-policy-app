namespace MultiTenantPolicyApi.Domain.Entities;

public class Tenant
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;

    public ICollection<Customer> Customers { get; set; } = [];
    public ICollection<Policy> Policies { get; set; } = [];
    public ICollection<User> Users { get; set; } = [];
}
