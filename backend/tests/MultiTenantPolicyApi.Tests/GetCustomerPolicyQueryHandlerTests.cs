using FluentAssertions;
using MultiTenantPolicyApi.Application.Features.Policies.Queries.GetCustomerPolicy;
using MultiTenantPolicyApi.Infrastructure.Repositories;

namespace MultiTenantPolicyApi.Tests;

public class GetCustomerPolicyQueryHandlerTests
{
    [Fact]
    public async Task GetCustomerPolicyQueryHandler_ReturnsPolicy_WhenCustomerBelongsToTenant()
    {
        var db = await TestDataFactory.CreateSeededContextAsync(TestDataFactory.TenantAId);
        var handler = new GetCustomerPolicyQueryHandler(new PolicyRepository(db));

        var result = await handler.Handle(new GetCustomerPolicyQuery(TestDataFactory.CustomerA1Id), CancellationToken.None);

        result.Should().NotBeNull();
        result!.PolicyNumber.Should().Be("POL-A-001");
        result.PremiumAmount.Should().Be(500m);
    }

    [Fact]
    public async Task GetCustomerPolicyQueryHandler_ReturnsNull_WhenCustomerIdBelongsToAnotherTenant()
    {
        // Arrange: seed TenantA + TenantB; context scoped to TenantA
        var db = await TestDataFactory.CreateSeededContextAsync(TestDataFactory.TenantAId);
        var handler = new GetCustomerPolicyQueryHandler(new PolicyRepository(db));

        // Act: request policy using TenantB's customerId under TenantA's context
        var result = await handler.Handle(new GetCustomerPolicyQuery(TestDataFactory.CustomerB1Id), CancellationToken.None);

        // Assert: EF global filter blocks cross-tenant access — no data leaked
        result.Should().BeNull();
    }
}
