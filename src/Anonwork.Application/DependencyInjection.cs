using Anonwork.Application.UseCases.Auth;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anonwork.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddScoped<RegisterUseCase>();
            services.AddScoped<LoginUseCase>();
            services.AddScoped<RefreshTokenUseCase>();  
            services.AddScoped<LogoutUseCase>();       
            return services;
        }
    }
}
