using Microsoft.EntityFrameworkCore;
using MultiTenantPolicyApi.Application.Common.Interfaces;
using MultiTenantPolicyApi.Domain.Entities;
using MultiTenantPolicyApi.Infrastructure.Persistence;

namespace MultiTenantPolicyApi.Infrastructure.Repositories;

public sealed class UserRepository : IUserRepository
{
    private readonly AppDbContext _db;

    public UserRepository(AppDbContext db)
    {
        _db = db;
    }

    public Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return _db.Users
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
    }

    public Task<User?> GetByRefreshTokenHashAsync(string refreshTokenHash, CancellationToken cancellationToken = default)
    {
        return _db.Users
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(u => u.RefreshTokenHash == refreshTokenHash, cancellationToken);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return _db.SaveChangesAsync(cancellationToken);
    }
}
