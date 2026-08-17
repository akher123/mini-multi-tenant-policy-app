using Carter;
using MediatR;
using MultiTenantPolicyApi.Application.Features.Policies.Queries.GetExpiringPolicies;

namespace MultiTenantPolicyApi.Api.Endpoints;

public sealed class GetExpiringPoliciesEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/policies/expiring", async (int withinDays, IMediator mediator) =>
        {
            var result = await mediator.Send(new GetExpiringPoliciesQuery(withinDays));
            return Results.Ok(result);
        })
        .WithName("GetExpiringPolicies")
        .WithTags("Policies")
        .RequireAuthorization();
    }
}
