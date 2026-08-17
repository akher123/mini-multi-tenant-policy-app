using MediatR;
using MultiTenantPolicyApi.Application.Common.Models;

namespace MultiTenantPolicyApi.Application.Features.Auth.Commands.Login;

public sealed record LoginCommand(string Email, string Password) : IRequest<AuthResponse?>;
