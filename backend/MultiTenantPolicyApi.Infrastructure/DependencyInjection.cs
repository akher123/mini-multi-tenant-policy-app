using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MultiTenantPolicyApi.Application.Common.Interfaces;
using MultiTenantPolicyApi.Infrastructure.Auth;
using MultiTenantPolicyApi.Infrastructure.Persistence;
using MultiTenantPolicyApi.Infrastructure.Repositories;

namespace MultiTenantPolicyApi.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.SectionName));

        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<IPolicyRepository, PolicyRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddSingleton<IPasswordHasher, PasswordHasher>();
        services.AddScoped<IDataSeeder, DataSeeder>();

        return services;
    }
}
