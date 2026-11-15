// This keeps signed-out visitors away from this study page.
using Microsoft.AspNetCore.Authorization;

// These lines let us find the signed-in user and load data from PostgreSQL.
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

// These lines let this file handle Razor Page requests.
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

// These lines let us use the Reviewer and Flashcard database models.
using Rebyulusyon.Data;

namespace Rebyulusyon.Pages;

// Only signed-in users can study their own reviewers.
[Authorize]
public class StudyModel : PageModel
{
    private readonly ApplicationDbContext _db;
    private readonly UserManager<ApplicationUser> _userManager;

    // ASP.NET provides the database and account tools to this page.
    public StudyModel(
        ApplicationDbContext db,
        UserManager<ApplicationUser> userManager)
    {
        _db = db;
        _userManager = userManager;
    }

    // These are filled with the reviewer's details and cards from PostgreSQL.
    public Reviewer Reviewer { get; private set; } = null!;
    public List<Flashcard> Cards { get; private set; } = new();
    public bool IsOwner { get; private set; }
    public string BackUrl => IsOwner ? $"/ReviewerCards/{Reviewer.Id}" : $"/ReviewerDetails/{Reviewer.Id}";
    public string BackLabel => IsOwner ? "← Back to manage flashcards" : "← Back to reviewer";

    // The reviewerId comes from an address like /Study/12.
    public async Task<IActionResult> OnGetAsync(int reviewerId)
    {
        var ownerId = _userManager.GetUserId(User)!;

        // Allow the owner to study any of their reviewers and other people to study public ones.
        var reviewer = await _db.Reviewers
            .Include(item => item.Subject)
            .FirstOrDefaultAsync(item =>
                item.Id == reviewerId && (item.OwnerId == ownerId || item.IsPublic));

        // Private reviewers belonging to another user still return not found.
        if (reviewer is null)
        {
            return NotFound();
        }

        Reviewer = reviewer;
        IsOwner = reviewer.OwnerId == ownerId;

        // Remember that this signed-in user opened the reviewer, even before rating a card.
        var visit = await _db.ReviewerStudies.FirstOrDefaultAsync(item =>
            item.UserId == ownerId && item.ReviewerId == reviewerId);
        if (visit is null)
        {
            _db.ReviewerStudies.Add(new ReviewerStudy
            {
                UserId = ownerId,
                ReviewerId = reviewerId,
                LastOpenedAtUtc = DateTime.UtcNow
            });
        }
        else
        {
            visit.LastOpenedAtUtc = DateTime.UtcNow;
        }
        await _db.SaveChangesAsync();

        // Load this reviewer's flashcards in the order they were added.
        Cards = await _db.Flashcards
            .Where(card => card.ReviewerId == reviewerId)
            .OrderBy(card => card.SortOrder)
            .ToListAsync();

        return Page();
    }

    // Save each card rating so the dashboard can calculate real study progress.
    public async Task<IActionResult> OnPostRecordAsync(int reviewerId, int flashcardId, bool gotIt)
    {
        var userId = _userManager.GetUserId(User)!;
        var canStudyCard = await _db.Flashcards.AnyAsync(card =>
            card.Id == flashcardId && card.ReviewerId == reviewerId &&
            (card.Reviewer.OwnerId == userId || card.Reviewer.IsPublic));

        if (!canStudyCard)
        {
            return NotFound();
        }

        _db.StudyActivities.Add(new StudyActivity
        {
            UserId = userId,
            FlashcardId = flashcardId,
            GotIt = gotIt,
            StudiedAtUtc = DateTime.UtcNow
        });

        await _db.SaveChangesAsync();
        return new JsonResult(new { saved = true });
    }
}
