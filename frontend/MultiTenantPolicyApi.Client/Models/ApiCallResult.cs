namespace MultiTenantPolicyApi.Client.Models;

public sealed record ApiCallResult<T>(T? Data, int StatusCode, string? ErrorMessage = null)
{
    public bool IsSuccess => StatusCode is >= 200 and < 300;
    public bool IsNotFound => StatusCode == 404;
}
