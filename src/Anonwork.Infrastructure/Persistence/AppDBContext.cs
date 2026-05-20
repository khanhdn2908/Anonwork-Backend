using Microsoft.EntityFrameworkCore;
using Anonwork.Domain.Entities;

namespace Anonwork.Infrastructure.Persistence
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users => Set<User>();
        public DbSet<Post> Posts => Set<Post>();
        public DbSet<Comment> Comments => Set<Comment>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
               .Property(x => x.Role)
               .HasConversion<string>();

            modelBuilder.Entity<PostTag>()
                .HasKey(pt => new { pt.PostId, pt.Tag });
        }
    }
}