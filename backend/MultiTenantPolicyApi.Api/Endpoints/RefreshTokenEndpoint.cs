using MultiTenantPolicyApi.Application.Features.Auth.Commands.RefreshToken;

namespace MultiTenantPolicyApi.Api.Endpoints;

public sealed class RefreshTokenEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/auth/refresh", async ([FromBody] RefreshTokenRequest request, IMediator mediator) =>
        {
            var result = await mediator.Send(new RefreshTokenCommand(request.RefreshToken));
            return result is null ? Results.Unauthorized() : Results.Ok(result);
        })
        .WithName("RefreshToken")
        .WithTags("Auth")
        .AllowAnonymous();
    }
}
