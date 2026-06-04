using Anonwork.Application.Common.Authorization;
using Anonwork.Application.Interfaces;
using Anonwork.Infrastructure.Common;
using Anonwork.Infrastructure.Persistence;
using Anonwork.Infrastructure.Repositories;
using Anonwork.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Resend;

namespace Anonwork.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"))
                   .UseSnakeCaseNamingConvention());
        services.AddScoped<IAppDbContext>(sp => sp.GetRequiredService<AppDbContext>());

        var redisUrl = configuration["REDIS_URL"]
            ?? throw new InvalidOperationException("REDIS_URL is not configured.");

        var redisConfig = redisUrl.Replace("redis://", string.Empty).Replace("rediss://", string.Empty);

        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = redisConfig;
            options.InstanceName = "anonwork:";
        });

        services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));
        services.Configure<CloudinaryOptions>(configuration.GetSection(CloudinaryOptions.SectionName));
        services.Configure<SepayOptions>(configuration.GetSection(SepayOptions.SectionName));
        services.Configure<EmailOptions>(configuration.GetSection(EmailOptions.SectionName));
        services.Configure<ResendClientOptions>(options =>
        {
            options.ApiToken = configuration[$"{EmailOptions.SectionName}:ResendApiKey"]
                ?? configuration["RESEND_API_KEY"]
                ?? string.Empty;
        });

        services.AddHttpClient<IResend, ResendClient>();

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

        services.AddScoped<IJwtService, JwtService>();
        services.AddSingleton<IPasswordHasher, PasswordHasher>();
        services.AddScoped<ICloudinaryService, CloudinaryService>();
        services.AddScoped<ISepayService, SepayService>();
        services.AddScoped<IEmailSender, EmailSender>();
        services.AddScoped<IRolePermissionService, RolePermissionService>();
        services.AddScoped<IAuthorizationHandler, PermissionHandler>();
        services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();

        return services;
    }
}
