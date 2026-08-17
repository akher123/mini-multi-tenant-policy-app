using MediatR;
using MultiTenantPolicyApi.Application.Common.Models;

namespace MultiTenantPolicyApi.Application.Features.Auth.Commands.RefreshToken;

public sealed record RefreshTokenCommand(string RefreshToken) : IRequest<AuthResponse?>;
