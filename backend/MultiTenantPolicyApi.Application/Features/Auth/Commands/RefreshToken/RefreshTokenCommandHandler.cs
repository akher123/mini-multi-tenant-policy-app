using MediatR;
using MultiTenantPolicyApi.Application.Common.Interfaces;
using MultiTenantPolicyApi.Application.Common.Models;

namespace MultiTenantPolicyApi.Application.Features.Auth.Commands.RefreshToken;

public sealed class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, AuthResponse?>
{
    private readonly IUserRepository _userRepository;
    private readonly ITokenService _tokenService;

    public RefreshTokenCommandHandler(IUserRepository userRepository, ITokenService tokenService)
    {
        _userRepository = userRepository;
        _tokenService = tokenService;
    }

    public async Task<AuthResponse?> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var hash = _tokenService.HashRefreshToken(request.RefreshToken);
        var user = await _userRepository.GetByRefreshTokenHashAsync(hash, cancellationToken);

        if (user is null ||
            user.RefreshTokenExpiresAt is null ||
            user.RefreshTokenExpiresAt <= DateTime.UtcNow ||
            !_tokenService.VerifyRefreshToken(request.RefreshToken, user.RefreshTokenHash))
        {
            return null;
        }

        var newRefreshToken = _tokenService.GenerateRefreshToken();
        user.RefreshTokenHash = _tokenService.HashRefreshToken(newRefreshToken);
        user.RefreshTokenExpiresAt = DateTime.UtcNow.AddDays(7);
        await _userRepository.SaveChangesAsync(cancellationToken);

        var accessToken = _tokenService.GenerateAccessToken(user);
        var expiresAt = DateTime.UtcNow.AddMinutes(15);

        return new AuthResponse(accessToken, newRefreshToken, expiresAt);
    }
}
