using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using MultiTenantPolicyApi.Client.Services;

namespace MultiTenantPolicyApi.Client.Services;

public sealed class ApiAuthenticationStateProvider : AuthenticationStateProvider
{
    private readonly AuthService _authService;

    public ApiAuthenticationStateProvider(AuthService authService)
    {
        _authService = authService;
        _authService.AuthenticationStateChanged += NotifyStateChanged;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var user = await _authService.GetUserAsync();
        return new AuthenticationState(user);
    }

    private void NotifyStateChanged()
    {
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }
}
