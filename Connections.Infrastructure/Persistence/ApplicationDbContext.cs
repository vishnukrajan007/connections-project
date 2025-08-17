using Connections.Domain.Entities;
using Connections.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Connections.Infrastructure.Persistence
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<RefreshToken>(b =>
            {
                b.HasKey(r => r.Id);
                b.Property(r => r.Token).IsRequired();
                b.Property(r => r.UserId).IsRequired();
                b.HasIndex(r => r.Token).IsUnique();
            });
        }
    }
}
