using MultiTenantPolicyApi.Domain.Entities;

namespace MultiTenantPolicyApi.Application.Common.Interfaces;

public interface ITokenService
{
    string GenerateAccessToken(User user);
    string GenerateRefreshToken();
    string HashRefreshToken(string refreshToken);
    bool VerifyRefreshToken(string refreshToken, string? storedHash);
}
