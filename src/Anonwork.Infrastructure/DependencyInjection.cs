using Anonwork.Domain.Repositories;
using Anonwork.Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
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
            services.AddSingleton<Supabase.Client>(provider =>
            {
                var url = configuration["Supabase:Url"];
                var key = configuration["Supabase:Key"];

                var options = new SupabaseOptions
                {
                    AutoConnectRealtime = false
                };

                var client = new Supabase.Client(url, key, options);

                client.InitializeAsync().Wait();

                return client;
            });

            services.AddScoped<IUserRepository, UserRepository>();

            return services;
        }
    }
}
