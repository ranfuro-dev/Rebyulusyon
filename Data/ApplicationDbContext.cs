using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Rebyulusyon.Data;

// The gateway between the app and PostgreSQL.
// IdentityDbContext also provides the account tables.
public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }

    public DbSet<Subject> Subjects => Set<Subject>();
    public DbSet<Reviewer> Reviewers => Set<Reviewer>();
    public DbSet<Flashcard> Flashcards => Set<Flashcard>();
    public DbSet<StudyActivity> StudyActivities => Set<StudyActivity>();
    public DbSet<ReviewerStudy> ReviewerStudies => Set<ReviewerStudy>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<ApplicationUser>(entity =>
        {
            // Existing accounts get the app's standard study preferences when this migration is applied.
            entity.Property(user => user.DailyGoal).HasDefaultValue(20);
            entity.Property(user => user.WeekStartsOnDay).HasDefaultValue(0);
        });

        builder.Entity<Subject>(entity =>
        {
            entity.Property(s => s.Name).HasMaxLength(100).IsRequired();

            entity.HasOne(s => s.Owner)
                  .WithMany(u => u.Subjects)
                  .HasForeignKey(s => s.OwnerId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(s => s.OwnerId);
        });

        builder.Entity<Reviewer>(entity =>
        {
            entity.Property(r => r.Title).HasMaxLength(150).IsRequired();
            entity.Property(r => r.Description).HasMaxLength(1000);

            entity.HasOne(r => r.Owner)
                  .WithMany(u => u.Reviewers)
                  .HasForeignKey(r => r.OwnerId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(r => r.Subject)
                  .WithMany(s => s.Reviewers)
                  .HasForeignKey(r => r.SubjectId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(r => new { r.IsPublic, r.CreatedAt });
            entity.HasIndex(r => new { r.OwnerId, r.SubjectId });
        });

        builder.Entity<Flashcard>(entity =>
        {
            entity.Property(c => c.Question).HasMaxLength(5000).IsRequired();
            entity.Property(c => c.Answer).HasMaxLength(5000).IsRequired();

            entity.HasOne(c => c.Reviewer)
                  .WithMany(r => r.Cards)
                  .HasForeignKey(c => c.ReviewerId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(c => new { c.ReviewerId, c.SortOrder });
        });

        builder.Entity<StudyActivity>(entity =>
        {
            entity.Property(activity => activity.UserId).IsRequired();

            entity.HasOne<ApplicationUser>()
                  .WithMany()
                  .HasForeignKey(activity => activity.UserId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne<Flashcard>()
                  .WithMany()
                  .HasForeignKey(activity => activity.FlashcardId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(activity => new { activity.UserId, activity.StudiedAtUtc });
            entity.HasIndex(activity => new { activity.UserId, activity.FlashcardId, activity.StudiedAtUtc });
        });

        builder.Entity<ReviewerStudy>(entity =>
        {
            entity.Property(study => study.UserId).IsRequired();

            entity.HasOne<ApplicationUser>()
                  .WithMany()
                  .HasForeignKey(study => study.UserId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne<Reviewer>()
                  .WithMany()
                  .HasForeignKey(study => study.ReviewerId)
                  .OnDelete(DeleteBehavior.Cascade);

            // One row per user and reviewer, updated each time they reopen it.
            entity.HasIndex(study => new { study.UserId, study.ReviewerId }).IsUnique();
            entity.HasIndex(study => new { study.UserId, study.LastOpenedAtUtc });
        });
    }
}
