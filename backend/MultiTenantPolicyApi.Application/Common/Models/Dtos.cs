namespace MultiTenantPolicyApi.Application.Common.Models;

public record AuthResponse(string AccessToken, string RefreshToken, DateTime ExpiresAt);

public record PolicyDto(string PolicyNumber, DateOnly ExpirationDate, decimal PremiumAmount);

public record ExpiringPolicyDto(
    string PolicyNumber,
    string CustomerName,
    DateOnly ExpirationDate,
    decimal PremiumAmount);

public record PolicyExpirationDto(string PolicyNumber, DateOnly ExpirationDate);

public record LoginRequest(string Email, string Password);

public record RefreshTokenRequest(string RefreshToken);
