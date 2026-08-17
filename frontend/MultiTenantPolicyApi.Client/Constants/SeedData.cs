namespace MultiTenantPolicyApi.Client.Constants;

/// <summary>
/// Seeded demo IDs matching backend DataSeeder (for assignment demos).
/// </summary>
public static class SeedData
{
    public static readonly Guid TenantAId = Guid.Parse("11111111-1111-1111-1111-111111111111");
    public static readonly Guid TenantBId = Guid.Parse("22222222-2222-2222-2222-222222222222");

    public static readonly Guid CustomerA1Id = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
    public static readonly Guid CustomerA2Id = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb");
    public static readonly Guid CustomerB1Id = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc");

    public const string PolicyA1 = "POL-A-001";
    public const string PolicyA2 = "POL-A-002";
    public const string PolicyB1 = "POL-B-001";

    public const string TenantAEmail = "admin@tenanta.local";
    public const string TenantBEmail = "admin@tenantb.local";
    public const string DemoPassword = "Password123!";
}
