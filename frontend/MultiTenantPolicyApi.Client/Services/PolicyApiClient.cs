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

    public async Task<ApiCallResult<PolicyDto>> GetCustomerPolicyAsync(
        Guid customerId,
        CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetAsync($"api/customers/{customerId}/policy", cancellationToken);
        return await MapResponseAsync<PolicyDto>(response, cancellationToken);
    }

    public async Task<ApiCallResult<IReadOnlyList<ExpiringPolicyDto>>> GetExpiringPoliciesAsync(
        int withinDays,
        CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetAsync($"api/policies/expiring?withinDays={withinDays}", cancellationToken);
        if (response.IsSuccessStatusCode)
        {
            var data = await response.Content.ReadFromJsonAsync<IReadOnlyList<ExpiringPolicyDto>>(cancellationToken) ?? [];
            return new ApiCallResult<IReadOnlyList<ExpiringPolicyDto>>(data, (int)response.StatusCode);
        }

        return new ApiCallResult<IReadOnlyList<ExpiringPolicyDto>>(
            null,
            (int)response.StatusCode,
            await ReadErrorAsync(response, cancellationToken));
    }

    public async Task<ApiCallResult<PolicyExpirationDto>> GetPolicyExpirationAsync(
        string policyNumber,
        CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetAsync(
            $"api/policies/{Uri.EscapeDataString(policyNumber)}/expiration",
            cancellationToken);

        return await MapResponseAsync<PolicyExpirationDto>(response, cancellationToken);
    }

    private static async Task<ApiCallResult<T>> MapResponseAsync<T>(
        HttpResponseMessage response,
        CancellationToken cancellationToken)
    {
        var statusCode = (int)response.StatusCode;

        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return new ApiCallResult<T>(default, statusCode);
        }

        if (!response.IsSuccessStatusCode)
        {
            return new ApiCallResult<T>(default, statusCode, await ReadErrorAsync(response, cancellationToken));
        }

        var data = await response.Content.ReadFromJsonAsync<T>(cancellationToken);
        return new ApiCallResult<T>(data, statusCode);
    }

    private static async Task<string?> ReadErrorAsync(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        try
        {
            return await response.Content.ReadAsStringAsync(cancellationToken);
        }
        catch
        {
            return response.ReasonPhrase;
        }
    }
}
