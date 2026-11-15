using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Rebyulusyon.Data;

namespace Rebyulusyon.Pages;

// Loads one public reviewer and its preview from PostgreSQL.
public class ReviewerDetailsModel : PageModel
{
    private readonly ApplicationDbContext _db;
    private readonly UserManager<ApplicationUser> _userManager;

    public ReviewerDetailsModel(ApplicationDbContext db, UserManager<ApplicationUser> userManager)
    {
        _db = db;
        _userManager = userManager;
    }

    public Reviewer Reviewer { get; private set; } = null!;
    public List<Flashcard> Preview { get; private set; } = new();

    // Only public reviewers can be opened from Explore Reviewers.
    public async Task<IActionResult> OnGetAsync(int reviewerId)
    {
        var reviewer = await _db.Reviewers
            .AsNoTracking()
            .Include(item => item.Subject)
            .Include(item => item.Owner)
            .Include(item => item.Cards)
            .FirstOrDefaultAsync(item => item.Id == reviewerId && item.IsPublic);

        if (reviewer is null)
        {
            return NotFound();
        }

        Reviewer = reviewer;
        Preview = reviewer.Cards.OrderBy(card => card.SortOrder).Take(3).ToList();

        // If the visitor is signed in, add this public reviewer to their dashboard history.
        var userId = _userManager.GetUserId(User);
        if (userId is not null)
        {
            var visit = await _db.ReviewerStudies.FirstOrDefaultAsync(item =>
                item.UserId == userId && item.ReviewerId == reviewerId);
            if (visit is null)
            {
                _db.ReviewerStudies.Add(new ReviewerStudy
                {
                    UserId = userId,
                    ReviewerId = reviewerId,
                    LastOpenedAtUtc = DateTime.UtcNow
                });
            }
            else
            {
                visit.LastOpenedAtUtc = DateTime.UtcNow;
            }

            await _db.SaveChangesAsync();
        }

        return Page();
    }
}
