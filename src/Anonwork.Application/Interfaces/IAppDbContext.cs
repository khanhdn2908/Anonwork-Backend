using Anonwork.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Anonwork.Application.Interfaces;

public interface IAppDbContext
{
    public DbSet<Bookmark> Bookmarks { get; }
    public DbSet<Comment> Comments { get; }
    public DbSet<Conversation> Conversations { get; }
    public DbSet<ConversationMember> ConversationMembers { get; }
    public DbSet<OneTimeToken> OneTimeTokens { get; }
    public DbSet<Follow> Follows { get; }
    public DbSet<Message> Messages { get; }
    public DbSet<Notification> Notifications { get; }
    public DbSet<Post> Posts { get; }
    public DbSet<PostMedia> PostMedia { get; }
    public DbSet<PostRating> PostRatings { get; }
    public DbSet<PostTag> PostTags { get; }
    public DbSet<Report> Reports { get; }
    public DbSet<Subject> Subjects { get; }
    public DbSet<AnonImage> AnonImages { get; }
    public DbSet<User> Users { get; }
    public DbSet<Vote> Votes { get; }
    public DbSet<SubscriptionPlan> SubscriptionPlans { get; }
    public DbSet<Order> Orders { get; }
    public DbSet<Permission> Permissions { get; }
    public  DbSet<Role> Roles { get; }
    public DbSet<RolePermission> RolePermissions { get; }
    public DbSet<UserRole> UserRoles { get; }
    public DbSet<UserSubscription> UserSubscriptions { get; }
    public DbSet<UserActivityLog> UserActivityLogs { get; }

    DbSet<T> Set<T>()
           where T : class;
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    public IDbContextTransaction BeginTransaction();
    public Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default);
}