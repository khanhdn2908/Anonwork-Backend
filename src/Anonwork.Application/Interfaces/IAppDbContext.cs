using Anonwork.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Anonwork.Application.Interfaces;

public interface IAppDbContext
{
    public DbSet<Bookmark> Bookmarks { get; }
    public DbSet<Comment> Comments { get; }
    public DbSet<Conversation> Conversations { get; }
    public DbSet<ConversationMember> ConversationMembers { get; }
    public DbSet<EmailVerificationToken> EmailVerificationTokens { get; }
    public DbSet<Follow> Follows { get; }
    public DbSet<Message> Messages { get; }
    public DbSet<Notification> Notifications { get; }
    public DbSet<Post> Posts { get; }
    public DbSet<PostImage> PostImages { get; }
    public DbSet<PostTag> PostTags { get; }
    public DbSet<Report> Reports { get; }
    public DbSet<Subject> Subjects { get; }
    public DbSet<User> Users { get; }
    public DbSet<Vote> Votes { get; }
    public DbSet<SubscriptionPlan> SubscriptionPlans { get; }
    public DbSet<Order> Orders { get; }
    public DbSet<Permission> Permissions { get; }
    public  DbSet<Role> Roles { get; }
    public DbSet<RolePermission> RolePermissions { get; }
    public DbSet<UserRole> UserRoles { get; }
    public DbSet<UserSubscription> UserSubscriptions { get; }

    DbSet<T> Set<T>()
           where T : class;
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}