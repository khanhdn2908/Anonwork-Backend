
using Anonwork.Application.Interfaces;
using Anonwork.Infrastructure.Persistence;
using Anonwork.Infrastructure.Repositories;
using Anonwork.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anonwork.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
        {

            services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"))
                       .UseSnakeCaseNamingConvention());

            var redisUrl = configuration["REDIS_URL"]
                ?? throw new InvalidOperationException("REDIS_URL is not configured.");

            var redisConfig = redisUrl.Replace("redis://", "").Replace("rediss://", "");

            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = redisConfig;
                options.InstanceName = "anonwork:";
            });

            // Jwt options
            services.Configure<JwtOptions>(configuration.GetSection("Jwt"));

            // Repositories
            services.AddScoped<IUserRepository, UserRepository>();

            // Services
            services.AddScoped<IJwtService, JwtService>();
            services.AddSingleton<IPasswordHasher, PasswordHasher>();

            return services;
        }
    }
}
