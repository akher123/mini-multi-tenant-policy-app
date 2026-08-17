using Microsoft.EntityFrameworkCore;
using MultiTenantPolicyApi.Application.Common.Interfaces;
using MultiTenantPolicyApi.Application.Common.Models;
using MultiTenantPolicyApi.Infrastructure.Persistence;

namespace MultiTenantPolicyApi.Infrastructure.Repositories;

public sealed class PolicyRepository : IPolicyRepository
{
    private readonly AppDbContext _db;

    public PolicyRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<PolicyDto?> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default)
    {
        var policy = await _db.Policies
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.CustomerId == customerId, cancellationToken);

        return policy is null
            ? null
            : new PolicyDto(policy.PolicyNumber, policy.ExpirationDate, policy.PremiumAmount);
    }

    public async Task<IReadOnlyList<ExpiringPolicyDto>> GetExpiringAsync(int withinDays, CancellationToken cancellationToken = default)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var cutoff = today.AddDays(withinDays);

        return await _db.Policies
            .AsNoTracking()
            .Include(p => p.Customer)
            .Where(p => p.ExpirationDate >= today && p.ExpirationDate <= cutoff)
            .OrderBy(p => p.ExpirationDate)
            .Select(p => new ExpiringPolicyDto(
                p.PolicyNumber,
                p.Customer.Name,
                p.ExpirationDate,
                p.PremiumAmount))
            .ToListAsync(cancellationToken);
    }

    public async Task<PolicyExpirationDto?> GetExpirationByPolicyNumberAsync(string policyNumber, CancellationToken cancellationToken = default)
    {
        var policy = await _db.Policies
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.PolicyNumber == policyNumber, cancellationToken);

        return policy is null
            ? null
            : new PolicyExpirationDto(policy.PolicyNumber, policy.ExpirationDate);
    }
}
