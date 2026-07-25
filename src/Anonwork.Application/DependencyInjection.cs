using Anonwork.Application.Features.AnonImages;
using Anonwork.Application.Features.Auth;
using Anonwork.Application.Features.Bookmarks;
using Anonwork.Application.Features.Comments;
using Anonwork.Application.Features.Follows;
using Anonwork.Application.Features.Maintenance;
using Anonwork.Application.Features.Payments;
using Anonwork.Application.Features.Permissions;
using Anonwork.Application.Features.PostRatings;
using Anonwork.Application.Features.Posts;
using Anonwork.Application.Features.Roles;
using Anonwork.Application.Features.Search;
using Anonwork.Application.Features.Subjects;
using Anonwork.Application.Features.SubscriptionPlans;
using Anonwork.Application.Features.UserSubscriptions;
using Anonwork.Application.Features.Users;
using Microsoft.Extensions.DependencyInjection;
using Anonwork.Application.Interfaces;

namespace Anonwork.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // ── Auth Use Cases ──────────────────────
        services.AddScoped<RegisterUseCase>();
        services.AddScoped<LoginUseCase>();
        services.AddScoped<GoogleLoginUseCase>();
        services.AddScoped<RefreshTokenUseCase>();
        services.AddScoped<LogoutUseCase>();
        services.AddScoped<VerifyEmailUseCase>();
        services.AddScoped<ForgotPasswordUseCase>();
        services.AddScoped<ResetPasswordUseCase>();
        // CleanupEmailVerificationTokensUseCase is currently commented out in source
        services.AddScoped<CleanupUnpaidExpiredOrdersUseCase>();

        // ── Posts Use Cases ─────────────────────
        services.AddScoped<CreatePostUseCase>();
        services.AddScoped<GetPostByIdUseCase>();
        services.AddScoped<GetPostsUseCase>();
        services.AddScoped<GetPostsBySubjectUseCase>();
        services.AddScoped<GetTopPostsByTimeUseCase>();
        services.AddScoped<UpdatePostUseCase>();
        services.AddScoped<DeletePostUseCase>();
        services.AddScoped<DeletePostUseCasePermanent>();
        services.AddScoped<TogglePostVoteUseCase>();

        // ── PostRatings Use Cases ───────────────
        services.AddScoped<RatePostUseCase>();
        services.AddScoped<GetPostRatingSummaryUseCase>();
        services.AddScoped<DeletePostRatingUseCase>();

        // ── Subjects Use Cases ──────────────────
        services.AddScoped<GetSubjectsUseCase>();
        services.AddScoped<GetSubjectByIdUseCase>();
        services.AddScoped<CreateSubjectUseCase>();
        services.AddScoped<UpdateSubjectUseCase>();
        services.AddScoped<DeleteSubjectUseCase>();
        services.AddScoped<DeleteSubjectUseCasePermanent>();

        // ── Anon Images Use Cases ───────────────
        services.AddScoped<GetAllAnonImagesUseCase>();
        services.AddScoped<GetAnonImageByIdUseCase>();
        services.AddScoped<CreateAnonImageUseCase>();
        services.AddScoped<UpdateAnonImageUseCase>();
        services.AddScoped<DeleteAnonImageUseCase>();
        services.AddScoped<DeleteAnonImageUseCasePermanent>();

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
        services.AddScoped<GetUserUseCase>();
        services.AddScoped<UpdateUserUseCase>();
        services.AddScoped<ToggleUserAnonDefaultUseCase>();
        services.AddScoped<AssignAnonImageToUserUseCase>();
        services.AddScoped<DeleteUserUseCase>();
        services.AddScoped<DeleteUserUseCasePermanent>();
        services.AddScoped<GetAllUsersUseCase>();
        services.AddScoped<GetTopContributorsUseCase>();
        services.AddScoped<AssignRoleToUserUseCase>();
        services.AddScoped<RemoveRoleFromUserUseCase>();
        services.AddScoped<GetUserRolesUseCase>();

        // ── Comments Use Cases ──────────────────
        services.AddScoped<CreateCommentUseCase>();
        services.AddScoped<GetCommentsByPostUseCase>();
        services.AddScoped<UpdateCommentUseCase>();
        services.AddScoped<DeleteCommentUseCase>();
        services.AddScoped<DeleteCommentUseCasePermanent>();
        services.AddScoped<ToggleCommentVoteUseCase>();

        // ── Payments Use Cases ──────────────────
        services.AddScoped<CreateOrderUseCase>();
        services.AddScoped<GetOrderStatusUseCase>();
        services.AddScoped<HandleSepayWebhookUseCase>();
        services.AddScoped<RenewSubscriptionUseCase>();

        // ── Roles Use Cases ─────────────────────
        services.AddScoped<GetAllRolesUseCase>();
        services.AddScoped<GetRoleByIdUseCase>();
        services.AddScoped<CreateRoleUseCase>();
        services.AddScoped<UpdateRoleUseCase>();
        services.AddScoped<DeleteRoleUseCase>();
        services.AddScoped<DeleteRoleUseCasePermanent>();
        services.AddScoped<AssignPermissionToRoleUseCase>();
        services.AddScoped<AssignPermissionsToRoleUseCase>();
        services.AddScoped<RemovePermissionFromRoleUseCase>();
        services.AddScoped<GetRolePermissionsUseCase>();

        // ── Permissions Use Cases ───────────────
        services.AddScoped<GetAllPermissionsUseCase>();
        services.AddScoped<GetPermissionByIdUseCase>();
        services.AddScoped<CreatePermissionUseCase>();
        services.AddScoped<UpdatePermissionUseCase>();
        services.AddScoped<DeletePermissionUseCase>();
        services.AddScoped<DeletePermissionUseCasePermanent>();

        // ── SubscriptionPlans Use Cases ─────────
        services.AddScoped<GetAllSubscriptionPlansUseCase>();
        services.AddScoped<GetSubscriptionPlanByIdUseCase>();
        services.AddScoped<GetSubscriptionPlanBySlugUseCase>();
        services.AddScoped<CreateSubscriptionPlanUseCase>();
        services.AddScoped<UpdateSubscriptionPlanUseCase>();
        services.AddScoped<DeleteSubscriptionPlanUseCase>();

        // ── UserSubscriptions Use Cases ─────────
        services.AddScoped<CreateUserSubscriptionUseCase>();
        services.AddScoped<GetUserSubscriptionByIdUseCase>();
        services.AddScoped<GetUserSubscriptionsByUserIdUseCase>();
        services.AddScoped<UpdateUserSubscriptionUseCase>();
        services.AddScoped<DeleteUserSubscriptionUseCase>();

        // ── Bookmarks Use Cases ─────────────────
        services.AddScoped<CreateBookmarkUseCase>();
        services.AddScoped<DeleteBookmarkUseCase>();
        services.AddScoped<GetBookmarksUseCase>();
        services.AddScoped<IsBookmarkedUseCase>();

        // ── Search Use Cases ────────────────────
        services.AddScoped<SearchAllUseCase>();
        services.AddScoped<SearchPostsUseCase>();
        services.AddScoped<SearchUsersUseCase>();

        return services;
    }
}
