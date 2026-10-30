// This line lets us use "IActionResult", which lets a page answer with "not found".
using Microsoft.AspNetCore.Mvc;

// This line lets us use "PageModel", the starting point for a Razor Page's C# code.
using Microsoft.AspNetCore.Mvc.RazorPages;

// This line lets us use the shared sample reviewers (Samples/ReviewerSamples.cs).
using Rebyulusyon.Samples;

// This is the "address" of this code inside your project. It is the same as your other pages.
namespace Rebyulusyon.Pages;

// This is for the C# side of the FLASHCARD STUDY page (Pages/FlashcardStudy.cshtml).
// It finds ONE reviewer by its number, checks that it is public, and gets all of its flashcards.
// NOTE: the reviewers and flashcards are sample data for now. They do NOT come from a database.
public class FlashcardStudyModel : PageModel
{
    // This is the reviewer that the page shows (the title, the creator, and so on).
    // "null!" means: it is empty at the start, but we fill it in OnGet before the page is shown.
    public SampleReviewer Reviewer { get; private set; } = null!;

    // These are ALL the flashcards of the reviewer: the questions AND the answers.
    // The browser needs the answers to turn the card over, so this is only filled for PUBLIC reviewers.
    public IReadOnlyList<SampleCard> Cards { get; private set; } = Array.Empty<SampleCard>();

    // "OnGet" runs when someone OPENS this page, for example /FlashcardStudy/5.
    // The "id" is the number from the web address. ASP.NET fills it in by itself.
    public IActionResult OnGet(int id)
    {
        // THE PRIVACY RULE: look for the reviewer by its number, but ONLY among the PUBLIC reviewers.
        var reviewer = ReviewerSamples.FindPublic(id);

        // A PRIVATE reviewer and a number that does not exist get the SAME answer: "not found".
        // So nobody can tell if a private reviewer exists, and the answers of a private reviewer are never sent.
        if (reviewer is null)
        {
            return NotFound();
        }

        // Get the flashcards. If a reviewer has none, there is nothing to study, so we say "not found".
        var cards = ReviewerSamples.CardsFor(id);
        if (cards.Count == 0)
        {
            return NotFound();
        }

        Reviewer = reviewer;
        Cards = cards;
        return Page();
    }
}