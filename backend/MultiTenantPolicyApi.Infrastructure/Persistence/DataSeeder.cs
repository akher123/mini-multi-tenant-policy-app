using Microsoft.EntityFrameworkCore;
using MultiTenantPolicyApi.Application.Common.Interfaces;
using MultiTenantPolicyApi.Domain.Entities;
using MultiTenantPolicyApi.Infrastructure.Persistence;

namespace MultiTenantPolicyApi.Infrastructure.Persistence;

public interface IDataSeeder
{
    Task SeedAsync(CancellationToken cancellationToken = default);
}

public sealed class DataSeeder : IDataSeeder
{
    public static readonly Guid TenantAId = Guid.Parse("11111111-1111-1111-1111-111111111111");
    public static readonly Guid TenantBId = Guid.Parse("22222222-2222-2222-2222-222222222222");
    public static readonly Guid CustomerA1Id = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
    public static readonly Guid CustomerA2Id = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb");
    public static readonly Guid CustomerB1Id = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc");

    private readonly AppDbContext _db;
    private readonly IPasswordHasher _passwordHasher;

    public DataSeeder(AppDbContext db, IPasswordHasher passwordHasher)
    {
        _db = db;
        _passwordHasher = passwordHasher;
    }

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        if (await _db.Tenants.IgnoreQueryFilters().AnyAsync(cancellationToken))
        {
            return;
        }

        var tenantA = new Tenant { Id = TenantAId, Name = "TenantA" };
        var tenantB = new Tenant { Id = TenantBId, Name = "TenantB" };

        var customerA1 = new Customer { Id = CustomerA1Id, TenantId = TenantAId, Name = "CustomerA1" };
        var customerA2 = new Customer { Id = CustomerA2Id, TenantId = TenantAId, Name = "CustomerA2" };
        var customerB1 = new Customer { Id = CustomerB1Id, TenantId = TenantBId, Name = "CustomerB1" };

        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        var policies = new[]
        {
            new Policy
            {
                Id = Guid.NewGuid(),
                TenantId = TenantAId,
                CustomerId = CustomerA1Id,
                PolicyNumber = "POL-A-001",
                ExpirationDate = today.AddDays(10),
                PremiumAmount = 500m
            },
            new Policy
            {
                Id = Guid.NewGuid(),
                TenantId = TenantAId,
                CustomerId = CustomerA2Id,
                PolicyNumber = "POL-A-002",
                ExpirationDate = today.AddDays(90),
                PremiumAmount = 750m
            },
            new Policy
            {
                Id = Guid.NewGuid(),
                TenantId = TenantBId,
                CustomerId = CustomerB1Id,
                PolicyNumber = "POL-B-001",
                ExpirationDate = today.AddDays(10),
                PremiumAmount = 600m
            }
        };

        var users = new[]
        {
            new User
            {
                Id = Guid.NewGuid(),
                TenantId = TenantAId,
                Email = "admin@tenanta.local",
                PasswordHash = _passwordHasher.HashPassword("Password123!")
            },
            new User
            {
                Id = Guid.NewGuid(),
                TenantId = TenantBId,
                Email = "admin@tenantb.local",
                PasswordHash = _passwordHasher.HashPassword("Password123!")
            }
        };

        _db.Tenants.AddRange(tenantA, tenantB);
        _db.Customers.AddRange(customerA1, customerA2, customerB1);
        _db.Policies.AddRange(policies);
        _db.Users.AddRange(users);

        await _db.SaveChangesAsync(cancellationToken);
    }
}
