using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;
using Microsoft.JSInterop;
using MultiTenantPolicyApi.Client.Models;

namespace MultiTenantPolicyApi.Client.Services;

public sealed class AuthService
{
    private const string AccessTokenKey = "accessToken";
    private const string RefreshTokenKey = "refreshToken";

    private readonly HttpClient _httpClient;
    private readonly IJSRuntime _jsRuntime;

    public AuthService(HttpClient httpClient, IJSRuntime jsRuntime)
    {
        _httpClient = httpClient;
        _jsRuntime = jsRuntime;
    }

    public event Action? AuthenticationStateChanged;

    public async Task<bool> LoginAsync(string email, string password, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.PostAsJsonAsync(
            "api/auth/login",
            new LoginRequest(email, password),
            cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            return false;
        }

        var auth = await response.Content.ReadFromJsonAsync<AuthResponse>(cancellationToken);
        if (auth is null)
        {
            return false;
        }

        await SaveTokensAsync(auth);
        AuthenticationStateChanged?.Invoke();
        return true;
    }

    public async Task<bool> RefreshTokenAsync(CancellationToken cancellationToken = default)
    {
        var refreshToken = await _jsRuntime.InvokeAsync<string?>("localStorage.getItem", RefreshTokenKey);
        if (string.IsNullOrWhiteSpace(refreshToken))
        {
            return false;
        }

        var response = await _httpClient.PostAsJsonAsync(
            "api/auth/refresh",
            new RefreshTokenRequest(refreshToken),
            cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            await LogoutAsync();
            return false;
        }

        var auth = await response.Content.ReadFromJsonAsync<AuthResponse>(cancellationToken);
        if (auth is null)
        {
            return false;
        }

        await SaveTokensAsync(auth);
        AuthenticationStateChanged?.Invoke();
        return true;
    }

    public async Task LogoutAsync()
    {
        await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", AccessTokenKey);
        await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", RefreshTokenKey);
        AuthenticationStateChanged?.Invoke();
    }

    public async Task<string?> GetAccessTokenAsync()
    {
        return await _jsRuntime.InvokeAsync<string?>("localStorage.getItem", AccessTokenKey);
    }

    public async Task<ClaimsPrincipal> GetUserAsync()
    {
        var token = await GetAccessTokenAsync();
        if (string.IsNullOrWhiteSpace(token))
        {
            return new ClaimsPrincipal(new ClaimsIdentity());
        }

        try
        {
            var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);
            if (jwt.ValidTo < DateTime.UtcNow && !await RefreshTokenAsync())
            {
                return new ClaimsPrincipal(new ClaimsIdentity());
            }

            if (jwt.ValidTo < DateTime.UtcNow)
            {
                token = await GetAccessTokenAsync();
                if (string.IsNullOrWhiteSpace(token))
                {
                    return new ClaimsPrincipal(new ClaimsIdentity());
                }

                jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);
            }

            var identity = new ClaimsIdentity(jwt.Claims, authenticationType: "jwt");
            return new ClaimsPrincipal(identity);
        }
        catch
        {
            return new ClaimsPrincipal(new ClaimsIdentity());
        }
    }

    public static string? GetEmail(ClaimsPrincipal user) =>
        user.FindFirst(ClaimTypes.Email)?.Value
        ?? user.FindFirst("email")?.Value;

    public static Guid? GetTenantId(ClaimsPrincipal user)
    {
        var claim = user.FindFirst("tenant_id")?.Value;
        return Guid.TryParse(claim, out var tenantId) ? tenantId : null;
    }

    private async Task SaveTokensAsync(AuthResponse auth)
    {
        await _jsRuntime.InvokeVoidAsync("localStorage.setItem", AccessTokenKey, auth.AccessToken);
        await _jsRuntime.InvokeVoidAsync("localStorage.setItem", RefreshTokenKey, auth.RefreshToken);
    }
}

public sealed class AuthorizationMessageHandler : DelegatingHandler
{
    private readonly AuthService _authService;

    public AuthorizationMessageHandler(AuthService authService)
    {
        _authService = authService;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var response = await SendWithBearerAsync(request, cancellationToken);
        if (response.StatusCode != HttpStatusCode.Unauthorized)
        {
            return response;
        }

        response.Dispose();
        if (await _authService.RefreshTokenAsync(cancellationToken))
        {
            return await SendWithBearerAsync(request, cancellationToken);
        }

        return new HttpResponseMessage(HttpStatusCode.Unauthorized);
    }

    private async Task<HttpResponseMessage> SendWithBearerAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var token = await _authService.GetAccessTokenAsync();
        if (!string.IsNullOrWhiteSpace(token))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        return await base.SendAsync(request, cancellationToken);
    }
}
