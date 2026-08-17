using FluentAssertions;
using MultiTenantPolicyApi.Application.Features.Policies.Queries.GetExpiringPolicies;
using MultiTenantPolicyApi.Infrastructure.Repositories;

namespace MultiTenantPolicyApi.Tests;

public class GetExpiringPoliciesQueryHandlerTests
{
    [Fact]
    public async Task GetExpiringPoliciesQueryHandler_ReturnsOnlyCurrentTenantPolicies()
    {
        var db = await TestDataFactory.CreateSeededContextAsync(TestDataFactory.TenantAId);
        var handler = new GetExpiringPoliciesQueryHandler(new PolicyRepository(db));

        var result = await handler.Handle(new GetExpiringPoliciesQuery(30), CancellationToken.None);

        result.Should().HaveCount(1);
        result[0].PolicyNumber.Should().Be("POL-A-001");
        result.Should().NotContain(p => p.PolicyNumber == "POL-B-001");
    }
}
