using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Optimus.Data.Entities;

namespace Optimus.Data
{
    public class ApplicationDbContext : IdentityDbContext<OptimusUser, IdentityRole<long>, long>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<UserAssessedRank> UserAssessedRanks { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserAssessedRank>().Property(x => x.StandardisedScore).HasColumnType("decimal(18,17)");

            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<OptimusUser>(b =>
            {
                // Each User can have many UserClaims
                b.HasMany(e => e.Claims)
                .WithOne()
                .HasForeignKey(uc => uc.UserId)
                .IsRequired();

                // Each User can have many UserLogins
                b.HasMany(e => e.Logins)
                .WithOne()
                .HasForeignKey(ul => ul.UserId)
                .IsRequired();

                // Each User can have many UserTokens
                b.HasMany(e => e.Tokens)
                .WithOne()
                .HasForeignKey(ut => ut.UserId)
                .IsRequired();

                // Each User can have many entries in the UserRole join table
                b.HasMany(e => e.UserRoles)
                .WithOne()
                .HasForeignKey(ur => ur.UserId)
                .IsRequired();

                //Each User can have many ranks
                b.HasMany(e => e.UserAssessedRanks)
                .WithOne()
                .HasForeignKey(e => e.UserId);
            });
        }
    }
}