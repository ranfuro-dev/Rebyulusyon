// This line lets us use "PageModel", the starting point for a Razor Page's C# code.
using Microsoft.AspNetCore.Mvc.RazorPages;

// This line lets us use [BindProperty], which fills a property from the web address.
using Microsoft.AspNetCore.Mvc;

// This line lets us use the shared sample reviewers (Samples/ReviewerSamples.cs).
using Rebyulusyon.Samples;

// This is the "address" of this code inside your project. It is the same as your other pages.
namespace Rebyulusyon.Pages;

// This is for the C# side of the EXPLORE REVIEWERS page (Pages/Reviewers.cshtml).
// It decides which sample reviewers the page shows.
// The sample reviewers are not typed in this file. They are in Samples/ReviewerSamples.cs,
// so the Explore Reviewers page and the Reviewer Details page use the SAME list.
// IMPORTANT: The class in this file must be called ReviewersModel, because Reviewers.cshtml uses that name.
public class ReviewersModel : PageModel
{
    // These are the category chips, in the order they show on the page.
    // "All Reviewers" is not in this list, because it means "no category chosen".
    public string[] CategoryNames { get; } =
    {
        "Mathematics",
        "Science",
        "Computer Engineering",
        "Programming",
        "General Education"
    };

    // This is for the words typed in the search bar.
    // [BindProperty(SupportsGet = true)] means: fill this from the web address, for example /Reviewers?q=python
    [BindProperty(SupportsGet = true)]
    public string? Q { get; set; }

    // This is for the chosen category chip, from the web address, for example /Reviewers?category=Science
    // If it is empty, it means "All Reviewers".
    [BindProperty(SupportsGet = true)]
    public string? Category { get; set; }

    // These are the reviewers that the page will show after the search and the category are applied.
    // "private set" means only this file can change it.
    public List<SampleReviewer> Results { get; private set; } = new();

    // This is how many PUBLIC reviewers exist in total. It is used for "Showing 5 of 12 public reviewers".
    // It never counts the private ones.
    public int TotalPublic { get; private set; }

    // "OnGet" runs every time someone OPENS this page, or uses the search bar or a chip.
    // The steps below are in a fixed order, and the ORDER is the privacy rule.
    public void OnGet()
    {
        // STEP 1: Clean what the visitor typed.
        // Remove spaces at the start and the end. If nothing is left, there is no search.
        Q = string.IsNullOrWhiteSpace(Q) ? null : Q.Trim();

        // A search can be at most 60 letters. This matches the "maxlength" in the search box.
        if (Q is { Length: > 60 })
        {
            Q = Q[..60];
        }

        // The category must be one of the names in our list. If it is not (or it is empty),
        // we ignore it and show all categories. This stops people from using made-up categories.
        Category = CategoryNames.FirstOrDefault(name => name.Equals(Category, StringComparison.OrdinalIgnoreCase));

        // STEP 2: THE PRIVACY RULE. Keep ONLY the public reviewers, BEFORE anything else.
        // Because this is the first filter, no search and no category can ever bring a private reviewer back.
        // The private reviewers are not even sent to the browser.
        // (Later, with a database, the same rule goes into the database question: "only public ones".)
        var visible = ReviewerSamples.Reviewers.Where(reviewer => reviewer.IsPublic).ToList();
        TotalPublic = visible.Count;

        // STEP 3: If a category chip was chosen, keep only the reviewers of that category.
        IEnumerable<SampleReviewer> filtered = visible;
        if (Category is not null)
        {
            filtered = filtered.Where(reviewer => reviewer.Category == Category);
        }

        // STEP 4: If something was typed in the search bar, keep only the reviewers that match.
        // The search is split into words. EVERY word must be found somewhere in the title, the description,
        // the category, or the creator name. Capital and small letters do not matter.
        if (Q is not null)
        {
            var words = Q.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                         .Select(word => word.TrimStart('@')) // "@ranfuro" works the same as "ranfuro"
                         .Where(word => word.Length > 0)
                         .ToArray();

            filtered = filtered.Where(reviewer =>
            {
                var text = $"{reviewer.Title} {reviewer.Description} {reviewer.Category} {reviewer.Creator}";
                return words.All(word => text.Contains(word, StringComparison.OrdinalIgnoreCase));
            });
        }

        // STEP 5: Save the final list. The page will show these cards.
        Results = filtered.ToList();
    }

    // This is for the color of the category tag on a card (blue, red, or yellow).
    // The colors are chosen in ReviewerSamples, so both pages use the same colors.
    public string ToneOf(string category) => ReviewerSamples.ToneOf(category);

    // This makes the web address for a chip. For example: /Reviewers?category=Science&q=cell
    // It keeps the words from the search bar, so choosing a chip does not erase the search.
    // If the category is null ("All Reviewers"), it is left out of the address.
    public string PageLink(string? category) =>
        Url.Page("/Reviewers", new { category, q = Q }) ?? "/Reviewers";
}