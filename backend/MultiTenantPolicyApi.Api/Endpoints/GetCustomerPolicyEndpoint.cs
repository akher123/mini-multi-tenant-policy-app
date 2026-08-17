using MultiTenantPolicyApi.Application.Features.Policies.Queries.GetCustomerPolicy;

namespace MultiTenantPolicyApi.Api.Endpoints;

public sealed class GetCustomerPolicyEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/customers/{customerId:guid}/policy", async (Guid customerId, IMediator mediator) =>
        {
            var result = await mediator.Send(new GetCustomerPolicyQuery(customerId));
            return result is null ? Results.NotFound() : Results.Ok(result);
        })
        .WithName("GetCustomerPolicy")
        .WithTags("Policies")
        .RequireAuthorization();
    }
}
