using Microsoft.EntityFrameworkCore;

namespace Instagram.Entities
{
    public class InstagramContext : DbContext
    {
        public InstagramContext()
        {

        }
        public InstagramContext(DbContextOptions<InstagramContext> options) : base(options)
        {

        }
        public virtual DbSet<User> User { get; set; }

        public virtual DbSet<PasswordResetToken> PasswordResetToken { get; set; }

        public virtual DbSet<Role> Role { get; set; }

        public virtual DbSet<UserRole> UserRole { get; set; }

        public virtual DbSet<UserMedia> UserMedia { get; set; }

        public virtual DbSet<Followers> Followers { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseMySql("ConnectionStrings:DefaultConnection", ServerVersion.Parse("8.0.28-mysql"))
                    .EnableSensitiveDataLogging()
                    .EnableDetailedErrors();
            }
        }
    }
}
