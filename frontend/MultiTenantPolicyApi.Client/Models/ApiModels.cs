namespace MultiTenantPolicyApi.Client.Models;

public sealed record AuthResponse(string AccessToken, string RefreshToken, DateTime ExpiresAt);

public sealed record LoginRequest(string Email, string Password);

public sealed record PolicyDto(string PolicyNumber, DateOnly ExpirationDate, decimal PremiumAmount);

public sealed record ExpiringPolicyDto(
    string PolicyNumber,
    string CustomerName,
    DateOnly ExpirationDate,
    decimal PremiumAmount);

public sealed record PolicyExpirationDto(string PolicyNumber, DateOnly ExpirationDate);
