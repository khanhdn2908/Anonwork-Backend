using Anonwork.Application.Interfaces;
using Anonwork.Infrastructure.Common;
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
            services.AddScoped<IAppDbContext>(sp => sp.GetRequiredService<AppDbContext>());

            var redisUrl = configuration["REDIS_URL"]
                ?? throw new InvalidOperationException("REDIS_URL is not configured.");

            var redisConfig = redisUrl.Replace("redis://", "").Replace("rediss://", "");

            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = redisConfig;
                options.InstanceName = "anonwork:";
            });

            // Jwt options
            services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));

            // Cloudinary options
            services.Configure<CloudinaryOptions>(configuration.GetSection(CloudinaryOptions.SectionName));

            // Sepay options
            services.Configure<SepayOptions>(configuration.GetSection(SepayOptions.SectionName));

            // Email options
            services.Configure<EmailOptions>(configuration.GetSection(EmailOptions.SectionName));

            // Unit of Work & Generic Repository
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

            // Services
            services.AddScoped<IJwtService, JwtService>();
            services.AddSingleton<IPasswordHasher, PasswordHasher>();
            services.AddScoped<ICloudinaryService, CloudinaryService>();
            services.AddScoped<ISepayService, SepayService>();
            services.AddScoped<IEmailSender, EmailSender>();

            return services;
        }
    }
}