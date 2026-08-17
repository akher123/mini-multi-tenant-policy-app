namespace MultiTenantPolicyApi.Client.Services;

public sealed class ApiSettings
{
    public const string SectionName = "Api";

    public string BaseUrl { get; set; } = "http://localhost:5105";
}
