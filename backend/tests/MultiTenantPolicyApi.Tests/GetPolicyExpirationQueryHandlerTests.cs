using FluentAssertions;
using MultiTenantPolicyApi.Application.Features.Policies.Queries.GetPolicyExpiration;
using MultiTenantPolicyApi.Infrastructure.Repositories;

namespace MultiTenantPolicyApi.Tests;

public class GetPolicyExpirationQueryHandlerTests
{
    [Fact]
    public async Task GetPolicyExpirationQueryHandler_ReturnsExactStoredDate_OrNullWhenNotFound()
    {
        var db = await TestDataFactory.CreateSeededContextAsync(TestDataFactory.TenantAId);
        var handler = new GetPolicyExpirationQueryHandler(new PolicyRepository(db));

        var found = await handler.Handle(new GetPolicyExpirationQuery("POL-A-001"), CancellationToken.None);
        var missing = await handler.Handle(new GetPolicyExpirationQuery("POL-DOES-NOT-EXIST"), CancellationToken.None);

        found.Should().NotBeNull();
        found!.PolicyNumber.Should().Be("POL-A-001");
        found.ExpirationDate.Should().Be(DateOnly.FromDateTime(DateTime.UtcNow).AddDays(10));

        missing.Should().BeNull();
    }
}
