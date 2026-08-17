using Microsoft.EntityFrameworkCore;
using MultiTenantPolicyApi.Application.Common.Interfaces;
using MultiTenantPolicyApi.Domain.Entities;
using MultiTenantPolicyApi.Infrastructure.Persistence;

namespace MultiTenantPolicyApi.Tests;

public static class TestDataFactory
{
    public static readonly Guid TenantAId = DataSeeder.TenantAId;
    public static readonly Guid TenantBId = DataSeeder.TenantBId;
    public static readonly Guid CustomerA1Id = DataSeeder.CustomerA1Id;
    public static readonly Guid CustomerB1Id = DataSeeder.CustomerB1Id;

    public static async Task<AppDbContext> CreateSeededContextAsync(Guid tenantId)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var seedContext = new AppDbContext(options, new StubTenantContext(Guid.Empty, false));
        await SeedAsync(seedContext);
        await seedContext.DisposeAsync();

        return new AppDbContext(options, new StubTenantContext(tenantId));
    }

    private static async Task SeedAsync(AppDbContext db)
    {
        var tenantA = new Tenant { Id = TenantAId, Name = "TenantA" };
        var tenantB = new Tenant { Id = TenantBId, Name = "TenantB" };

        var customerA1 = new Customer { Id = CustomerA1Id, TenantId = TenantAId, Name = "CustomerA1" };
        var customerA2 = new Customer { Id = DataSeeder.CustomerA2Id, TenantId = TenantAId, Name = "CustomerA2" };
        var customerB1 = new Customer { Id = CustomerB1Id, TenantId = TenantBId, Name = "CustomerB1" };

        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        db.Tenants.AddRange(tenantA, tenantB);
        db.Customers.AddRange(customerA1, customerA2, customerB1);
        db.Policies.AddRange(
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
                CustomerId = DataSeeder.CustomerA2Id,
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
            });

        await db.SaveChangesAsync();
    }
}
