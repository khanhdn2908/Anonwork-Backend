using Anonwork.Domain.Repositories;
using Anonwork.Infrastructure.Persistence;
using Anonwork.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Supabase;
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


            //services.AddSingleton<Supabase.Client>(provider =>
            //{
            //    var url = configuration["Supabase:Url"];
            //    var key = configuration["Supabase:Key"];

            //    var options = new SupabaseOptions
            //    {
            //        AutoConnectRealtime = false
            //    };

            //    var client = new Supabase.Client(url, key, options);

            //    client.InitializeAsync().Wait();

            //    return client;
            //});



            return services;
        }
    }
}
