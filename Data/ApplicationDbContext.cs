using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Rebyulusyon.Data;

// The "gateway" between your C# code and the database.
// IdentityDbContext adds all the account tables (AspNetUsers, AspNetRoles, and so on).
public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    // Becomes a table named "Reviewers".
    public DbSet<Reviewer> Reviewers => Set<Reviewer>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);   // keep Identity's own table setup

        builder.Entity<Reviewer>(entity =>
        {
            entity.Property(r => r.Title).HasMaxLength(150).IsRequired();
            entity.Property(r => r.Description).HasMaxLength(1000);

            // Every reviewer belongs to exactly one user.
            // If a user is deleted, their reviewers are deleted too.
            entity.HasOne(r => r.Owner)
                  .WithMany(u => u.Reviewers)
                  .HasForeignKey(r => r.OwnerId)
                  .OnDelete(DeleteBehavior.Cascade);

            // Makes "show public reviewers, newest first" fast later.
            entity.HasIndex(r => new { r.IsPublic, r.CreatedAt });
        });
    }
}