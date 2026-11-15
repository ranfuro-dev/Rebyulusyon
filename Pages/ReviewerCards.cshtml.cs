// This line keeps signed-out visitors away from this page.
using Microsoft.AspNetCore.Authorization;

// This line lets us validate question and answer fields.
using System.ComponentModel.DataAnnotations;

// This line lets us get the ID of the signed-in user.
using Microsoft.AspNetCore.Identity;

// This line lets us read and save data in PostgreSQL.
using Microsoft.EntityFrameworkCore;

// These lines let us use Razor Page handlers and responses.
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

// This line lets us use our database model classes.
using Rebyulusyon.Data;

namespace Rebyulusyon.Pages;

// [Authorize] means only a signed-in user can open this page.
[Authorize]
public class ReviewerCardsModel : PageModel
{
    private readonly ApplicationDbContext _db;
    private readonly UserManager<ApplicationUser> _userManager;

    public ReviewerCardsModel(
        ApplicationDbContext db,
        UserManager<ApplicationUser> userManager)
    {
        _db = db;
        _userManager = userManager;
    }

    // These properties receive the values typed into the Add Flashcard form.
    [BindProperty]
    [Required]
    [StringLength(5000)]
    public string Question { get; set; } = "";

    [BindProperty]
    [Required]
    [StringLength(5000)]
    public string Answer { get; set; } = "";

    // These are filled in after confirming that the signed-in user owns the reviewer.
    public Reviewer Reviewer { get; private set; } = null!;
    public List<Flashcard> Cards { get; private set; } = new();

    // The reviewerId comes from the address, such as /ReviewerCards/12.
    public async Task<IActionResult> OnGetAsync(int reviewerId)
    {
        if (!await LoadPageDataAsync(reviewerId))
        {
            return NotFound();
        }

        return Page();
    }

    // This runs when someone submits the Add Flashcard form.
    public async Task<IActionResult> OnPostAsync(int reviewerId)
    {
        Question = Question.Trim();
        Answer = Answer.Trim();

        if (!ModelState.IsValid)
        {
            if (!await LoadPageDataAsync(reviewerId))
            {
                return NotFound();
            }

            return Page();
        }

        var ownerId = _userManager.GetUserId(User)!;

        // Check ownership again before saving. A changed URL cannot add a card to someone else's reviewer.
        var reviewerExists = await _db.Reviewers.AnyAsync(reviewer =>
            reviewer.Id == reviewerId && reviewer.OwnerId == ownerId);

        if (!reviewerExists)
        {
            return NotFound();
        }

        // Put this card after the reviewer's existing cards.
        var lastSortOrder = await _db.Flashcards
            .Where(card => card.ReviewerId == reviewerId)
            .Select(card => (int?)card.SortOrder)
            .MaxAsync() ?? -1;

        var flashcard = new Flashcard
        {
            Question = Question,
            Answer = Answer,
            ReviewerId = reviewerId,
            SortOrder = lastSortOrder + 1
        };

        _db.Flashcards.Add(flashcard);
        await _db.SaveChangesAsync();

        // Redirect after saving so refreshing does not submit the form again.
        return RedirectToPage(new { reviewerId });
    }

    // Delete one flashcard only when its reviewer belongs to the signed-in user.
    public async Task<IActionResult> OnPostDeleteAsync(int reviewerId, int flashcardId)
    {
        var ownerId = _userManager.GetUserId(User)!;

        var flashcard = await _db.Flashcards.FirstOrDefaultAsync(card =>
            card.Id == flashcardId &&
            card.ReviewerId == reviewerId &&
            card.Reviewer.OwnerId == ownerId);

        if (flashcard is null)
        {
            return NotFound();
        }

        _db.Flashcards.Remove(flashcard);
        await _db.SaveChangesAsync();

        // Return to the flashcard list after deleting the card.
        return Redirect($"/ReviewerCards/{reviewerId}#flashcards");
    }

    // Load this reviewer's cards only after confirming the reviewer belongs to the signed-in user.
    private async Task<bool> LoadPageDataAsync(int reviewerId)
    {
        var ownerId = _userManager.GetUserId(User)!;

        var reviewer = await _db.Reviewers
            .Include(item => item.Subject)
            .FirstOrDefaultAsync(item =>
                item.Id == reviewerId && item.OwnerId == ownerId);

        if (reviewer is null)
        {
            return false;
        }

        Reviewer = reviewer;

        Cards = await _db.Flashcards
            .Where(card => card.ReviewerId == reviewerId)
            .OrderBy(card => card.SortOrder)
            .ToListAsync();

        return true;
    }
}
