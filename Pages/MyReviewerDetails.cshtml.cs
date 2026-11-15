// Only signed-in users can open this page.
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Rebyulusyon.Data;

namespace Rebyulusyon.Pages;

[Authorize]
public class MyReviewerDetailsModel : PageModel
{
    private readonly ApplicationDbContext _db;
    private readonly UserManager<ApplicationUser> _userManager;

    // ASP.NET provides the database and account tools to this page.
    public MyReviewerDetailsModel(
        ApplicationDbContext db,
        UserManager<ApplicationUser> userManager)
    {
        _db = db;
        _userManager = userManager;
    }

    // These properties hold the reviewer's details and flashcards.
    public Reviewer Reviewer { get; private set; } = null!;
    public List<Flashcard> Cards { get; private set; } = new();

    // This receives the visibility choice from the Public or Private options.
    [BindProperty]
    public bool IsPublic { get; set; }

    // The reviewerId comes from an address like /MyReviewerDetails/12.
    public async Task<IActionResult> OnGetAsync(int reviewerId)
    {
        var ownerId = _userManager.GetUserId(User)!;

        // Load this reviewer only if it belongs to the signed-in user.
        var reviewer = await _db.Reviewers
            .Include(item => item.Subject)
            .Include(item => item.Owner)
            .Include(item => item.Cards)
            .FirstOrDefaultAsync(item =>
                item.Id == reviewerId && item.OwnerId == ownerId);

        // A missing or other user's reviewer returns the same not-found result.
        if (reviewer is null)
        {
            return NotFound();
        }

        Reviewer = reviewer;
        // Opening one of your own reviewers also refreshes it in the dashboard's continue list.
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
        IsPublic = reviewer.IsPublic;
        Cards = reviewer.Cards
            .OrderBy(card => card.SortOrder)
            .ToList();

        await _db.SaveChangesAsync();

        return Page();
    }

    // Save the visibility choice only when this reviewer belongs to the signed-in user.
    public async Task<IActionResult> OnPostUpdateVisibilityAsync(int reviewerId)
    {
        var ownerId = _userManager.GetUserId(User)!;
        var reviewer = await _db.Reviewers.FirstOrDefaultAsync(item =>
            item.Id == reviewerId && item.OwnerId == ownerId);

        if (reviewer is null)
        {
            return NotFound();
        }

        reviewer.IsPublic = IsPublic;
        await _db.SaveChangesAsync();

        TempData["VisibilityMessage"] = IsPublic
            ? "This reviewer is now public and can appear in Explore Reviewers."
            : "This reviewer is now private and no longer appears in Explore Reviewers.";

        return RedirectToPage(new { reviewerId });
    }
}
