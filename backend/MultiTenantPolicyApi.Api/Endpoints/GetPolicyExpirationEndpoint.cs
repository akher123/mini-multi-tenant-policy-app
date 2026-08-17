using MultiTenantPolicyApi.Application.Features.Policies.Queries.GetPolicyExpiration;

namespace MultiTenantPolicyApi.Api.Endpoints;

public sealed class GetPolicyExpirationEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/policies/{policyNumber}/expiration", async (string policyNumber, IMediator mediator) =>
        {
            var result = await mediator.Send(new GetPolicyExpirationQuery(policyNumber));
            return result is null ? Results.NotFound() : Results.Ok(result);
        })
        .WithName("GetPolicyExpiration")
        .WithTags("Policies")
        .RequireAuthorization();
    }
}
