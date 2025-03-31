using DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Contexts
{
    public class UserDbContext : DbContext
    {
        public UserDbContext(DbContextOptions<UserDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<UserNotification> UserNotifications { get; set; }
        public DbSet<UserTransaction> UserTransactions { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<UserTransaction>()
                .Property(t => t.EventName)
                .IsRequired(false);

            // enum to string
            modelBuilder.Entity<User>()
                .Property(u => u.Role)
                .HasConversion(
                    v => v.ToString(),
                    v => (UserRoles)Enum.Parse(typeof(UserRoles), v)
                );

            // enum to string
            modelBuilder.Entity<UserNotification>()
                .Property(u => u.Status)
                .HasConversion(
                    v => v.ToString(),
                    v => (NotificationStatuses)Enum.Parse(typeof(NotificationStatuses), v)
                );

            modelBuilder.Entity<RefreshToken>()
                .HasIndex(u => u.UserId)
                .IsUnique();

            modelBuilder.Entity<RefreshToken>()
                .HasIndex(u => u.Token)
                .IsUnique();
        }
    }
}
