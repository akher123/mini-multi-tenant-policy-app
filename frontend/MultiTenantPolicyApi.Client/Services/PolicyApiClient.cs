using System.Net.Http.Json;
using MultiTenantPolicyApi.Client.Models;

namespace MultiTenantPolicyApi.Client.Services;

public sealed class PolicyApiClient
{
    private readonly HttpClient _httpClient;

    public PolicyApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<PolicyDto?> GetCustomerPolicyAsync(Guid customerId, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetAsync($"api/customers/{customerId}/policy", cancellationToken);
        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return null;
        }

        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<PolicyDto>(cancellationToken);
    }

    public async Task<IReadOnlyList<ExpiringPolicyDto>> GetExpiringPoliciesAsync(
        int withinDays,
        CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetAsync($"api/policies/expiring?withinDays={withinDays}", cancellationToken);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<IReadOnlyList<ExpiringPolicyDto>>(cancellationToken)
            ?? [];
    }

    public async Task<PolicyExpirationDto?> GetPolicyExpirationAsync(
        string policyNumber,
        CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetAsync($"api/policies/{Uri.EscapeDataString(policyNumber)}/expiration", cancellationToken);
        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return null;
        }

        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<PolicyExpirationDto>(cancellationToken);
    }
}
