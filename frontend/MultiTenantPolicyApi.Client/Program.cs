using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.JSInterop;
using MultiTenantPolicyApi.Client;
using MultiTenantPolicyApi.Client.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

var apiBaseUrl = builder.Configuration[$"{ApiSettings.SectionName}:BaseUrl"]
    ?? "http://localhost:5105";

builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<ApiAuthenticationStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(sp => sp.GetRequiredService<ApiAuthenticationStateProvider>());

builder.Services.AddScoped<AuthService>(sp =>
    new AuthService(new HttpClient { BaseAddress = new Uri(apiBaseUrl) }, sp.GetRequiredService<IJSRuntime>()));

builder.Services.AddHttpClient<PolicyApiClient>(client => client.BaseAddress = new Uri(apiBaseUrl))
    .AddHttpMessageHandler(sp =>
    {
        var authService = sp.GetRequiredService<AuthService>();
        return new AuthorizationMessageHandler(authService)
        {
            InnerHandler = new HttpClientHandler()
        };
    });

await builder.Build().RunAsync();
