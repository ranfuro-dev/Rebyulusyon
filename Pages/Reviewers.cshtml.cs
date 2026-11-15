using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Rebyulusyon.Data;

namespace Rebyulusyon.Pages;

// Loads only public reviewers from PostgreSQL for the Explore Reviewers page.
public class ReviewersModel : PageModel
{
    private readonly ApplicationDbContext _db;

    public ReviewersModel(ApplicationDbContext db) => _db = db;

    // These category chips come from subjects that actually have public reviewers.
    public List<string> CategoryNames { get; private set; } = new();

    [BindProperty(SupportsGet = true)]
    public string? Q { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? Category { get; set; }

    public List<Reviewer> Results { get; private set; } = new();
    public int TotalPublic { get; private set; }

    // This runs when the page opens and whenever the search or a subject filter is used.
    public async Task OnGetAsync()
    {
        Q = string.IsNullOrWhiteSpace(Q) ? null : Q.Trim();
        if (Q is { Length: > 60 }) Q = Q[..60];

        // Apply the privacy filter in the database before collecting anything for the page.
        var publicReviewers = _db.Reviewers
            .AsNoTracking()
            .Where(reviewer => reviewer.IsPublic);

        TotalPublic = await publicReviewers.CountAsync();

        CategoryNames = await publicReviewers
            .Select(reviewer => reviewer.Subject.Name)
            .Distinct()
            .OrderBy(name => name)
            .ToListAsync();

        Category = CategoryNames.FirstOrDefault(name =>
            name.Equals(Category, StringComparison.OrdinalIgnoreCase));

        var query = publicReviewers
            .Include(reviewer => reviewer.Subject)
            .Include(reviewer => reviewer.Owner)
            .Include(reviewer => reviewer.Cards)
            .AsQueryable();

        if (Category is not null)
        {
            query = query.Where(reviewer => reviewer.Subject.Name == Category);
        }

        if (Q is not null)
        {
            // Each typed word must match the title, description, subject, or creator.
            var words = Q.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Select(word => word.TrimStart('@'))
                .Where(word => word.Length > 0);

            foreach (var word in words)
            {
                var pattern = $"%{word}%";
                query = query.Where(reviewer =>
                    EF.Functions.ILike(reviewer.Title, pattern) ||
                    (reviewer.Description != null && EF.Functions.ILike(reviewer.Description, pattern)) ||
                    EF.Functions.ILike(reviewer.Subject.Name, pattern) ||
                    (reviewer.Owner.UserName != null && EF.Functions.ILike(reviewer.Owner.UserName, pattern)));
            }
        }

        Results = await query
            .OrderByDescending(reviewer => reviewer.CreatedAt)
            .ToListAsync();
    }

    // Keep the chosen subject, search text, and dashboard layout in the filter links.
    public string PageLink(string? category)
    {
        var insideDashboard = Request.Query["inside"].ToString() == "dashboard";
        return Url.Page("/Reviewers", new
        {
            category,
            q = Q,
            inside = insideDashboard ? "dashboard" : null
        }) ?? "/Reviewers";
    }
}
