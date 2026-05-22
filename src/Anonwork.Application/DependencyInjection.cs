using Anonwork.Application.Features.Auth;
using Anonwork.Application.Features.Posts;
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
            // ── Auth Use Cases ──────────────────────
            services.AddScoped<RegisterUseCase>();
            services.AddScoped<LoginUseCase>();
            services.AddScoped<RefreshTokenUseCase>();  
            services.AddScoped<LogoutUseCase>();

            // ── Posts Use Cases ─────────────────────
            services.AddScoped<CreatePostUseCase>();
            services.AddScoped<GetPostByIdUseCase>();
            services.AddScoped<GetPostsUseCase>();
            services.AddScoped<GetPostsBySubjectUseCase>();
            services.AddScoped<UpdatePostUseCase>();
            services.AddScoped<DeletePostUseCase>();

            return services;
        }
    }
}
