// This line lets us use "IActionResult", which lets a page answer with "not found".
using Microsoft.AspNetCore.Mvc;

// This line lets us use "PageModel", the starting point for a Razor Page's C# code.
using Microsoft.AspNetCore.Mvc.RazorPages;

// This line lets us use the shared sample reviewers (Samples/ReviewerSamples.cs).
using Rebyulusyon.Samples;

// This is the "address" of this code inside your project. It is the same as your other pages.
namespace Rebyulusyon.Pages;

// This is for the C# side of the REVIEWER DETAILS page (Pages/ReviewerDetails.cshtml).
// It finds ONE reviewer by its number and checks that it is public.
// NOTE: the reviewers are sample data for now. They do NOT come from a database.
public class ReviewerDetailsModel : PageModel
{
    // This is the reviewer that the page shows (the title, the creator, and so on).
    // "private set" means only this file can change it.
    // "null!" means: it is empty at the start, but we fill it in OnGet before the page is shown.
    public SampleReviewer Reviewer { get; private set; } = null!;

    // These are the first flashcards for the "Preview" card.
    // We only keep the QUESTIONS in the page. The answers stay hidden.
    public IReadOnlyList<SampleCard> Preview { get; private set; } = Array.Empty<SampleCard>();

    // "OnGet" runs when someone OPENS this page, for example /ReviewerDetails/5.
    // The "id" is the number from the web address. ASP.NET fills it in by itself.
    // "IActionResult" means this method can answer in two ways: show the page, or say "not found".
    public IActionResult OnGet(int id)
    {
        // THE PRIVACY RULE: look for the reviewer by its number, but ONLY among the PUBLIC reviewers.
        var reviewer = ReviewerSamples.FindPublic(id);

        // If there is no public reviewer with this number, say "not found".
        // This happens for numbers that do not exist AND for PRIVATE reviewers, with the SAME answer.
        // So nobody can tell if a private reviewer exists. (Later, the owner will be able to open their own.)
        if (reviewer is null)
        {
            return NotFound();
        }

        // The reviewer was found and it is public, so we save it and its preview for the page.
        Reviewer = reviewer;
        Preview = ReviewerSamples.PreviewFor(id);
        return Page();
    }

    // This is for the color of the category tag (blue, red, or yellow).
    public string ToneOf(string category) => ReviewerSamples.ToneOf(category);
}