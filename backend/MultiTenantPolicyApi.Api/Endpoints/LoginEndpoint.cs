using Carter;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using MultiTenantPolicyApi.Application.Common.Models;
using MultiTenantPolicyApi.Application.Features.Auth.Commands.Login;

namespace MultiTenantPolicyApi.Api.Endpoints;

public sealed class LoginEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/auth/login", async ([FromBody] LoginRequest request, IMediator mediator) =>
        {
            var result = await mediator.Send(new LoginCommand(request.Email, request.Password));
            return result is null ? Results.Unauthorized() : Results.Ok(result);
        })
        .WithName("Login")
        .WithTags("Auth")
        .AllowAnonymous();
    }
}
