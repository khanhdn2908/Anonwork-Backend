using Anonwork.Application.Features.Auth;
using Anonwork.Application.Features.Follows;
using Anonwork.Application.Features.Payments;
using Anonwork.Application.Features.Posts;
using Anonwork.Application.Features.Subjects;
using Anonwork.Application.Features.SubscriptionPlans;
using Anonwork.Application.Features.Users;
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

            // ── Subjects Use Cases ──────────────────
            services.AddScoped<GetSubjectsUseCase>();
            services.AddScoped<GetSubjectByIdUseCase>();
            services.AddScoped<CreateSubjectUseCase>();
            services.AddScoped<UpdateSubjectUseCase>();
            services.AddScoped<DeleteSubjectUseCase>();

            // ── Follows Use Cases ──────────────────
            services.AddScoped<FollowUserUseCase>();
            services.AddScoped<GetFollowByIdUseCase>();
            services.AddScoped<GetFollowersUseCase>();
            services.AddScoped<GetFollowingUseCase>();
            services.AddScoped<GetFollowStatsUseCase>();
            services.AddScoped<IsFollowingUseCase>();
            services.AddScoped<UnfollowUserUseCase>();

            // ── Users Use Cases ─────────────────────
            services.AddScoped<GetMeUseCase>();
            services.AddScoped<UpdateUserUseCase>();
            services.AddScoped<DeleteUserUseCase>();
            services.AddScoped<GetAllUsersUseCase>();

            // ── Payments Use Cases ──────────────────
            services.AddScoped<CreateOrderUseCase>();
            services.AddScoped<GetOrderStatusUseCase>();
            services.AddScoped<HandleSepayWebhookUseCase>();
            services.AddScoped<RenewSubscriptionUseCase>();

            // ── SubscriptionPlans Use Cases ─────────
            services.AddScoped<GetAllSubscriptionPlansUseCase>();
            services.AddScoped<GetSubscriptionPlanByIdUseCase>();
            services.AddScoped<GetSubscriptionPlanBySlugUseCase>();
            services.AddScoped<CreateSubscriptionPlanUseCase>();
            services.AddScoped<UpdateSubscriptionPlanUseCase>();
            services.AddScoped<DeleteSubscriptionPlanUseCase>();

            return services;
        }
    }
}
