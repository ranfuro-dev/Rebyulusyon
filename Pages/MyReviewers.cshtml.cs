// This line lets us check the form fields, such as the required reviewer title.
using System.ComponentModel.DataAnnotations;

// This line lets us get the ID of the person who is signed in.
using Microsoft.AspNetCore.Identity;

// This line lets us read and save information in PostgreSQL.
using Microsoft.EntityFrameworkCore;

// This line lets us use "IActionResult", which lets a page show itself or redirect.
using Microsoft.AspNetCore.Mvc;

// This line lets us use "BindProperty" and "PageModel".
using Microsoft.AspNetCore.Mvc.RazorPages;

// This line lets us use the account, subject, reviewer, and database classes.
using Rebyulusyon.Data;

// This is the "address" of this code inside your project.
namespace Rebyulusyon.Pages;

// This is for the C# side of the My Reviewers page (Pages/MyReviewers.cshtml).
public class MyReviewersModel : PageModel
{
    // These give this page access to the database and the signed-in user's account.
    private readonly ApplicationDbContext _db;
    private readonly UserManager<ApplicationUser> _userManager;

    // ASP.NET gives us these tools when it creates this page.
    public MyReviewersModel(
        ApplicationDbContext db,
        UserManager<ApplicationUser> userManager)
    {
        _db = db;
        _userManager = userManager;
    }

    // These properties receive the values typed into the Create Reviewer form.
    [BindProperty]
    [Required]
    [StringLength(150)]
    public string Title { get; set; } = "";

    [BindProperty]
    [StringLength(1000)]
    public string? Description { get; set; }

    // The selected subject ID comes from the subject drop-down list.
    [BindProperty]
    [Required(ErrorMessage = "Choose a subject.")]
    public int? SubjectId { get; set; }

    // This optional filter comes from clicking a subject on the My Subjects page.
    [BindProperty(SupportsGet = true, Name = "subjectId")]
    public int? FilterSubjectId { get; set; }

    // This is filled only when the filter belongs to the signed-in user.
    public Subject? FilteredSubject { get; private set; }

    // This receives a name when the user chooses to add a new subject.
    [BindProperty]
    [StringLength(100)]
    public string? NewSubjectName { get; set; }

    // This records whether other people are allowed to view the reviewer.
    [BindProperty]
    public bool IsPublic { get; set; }

    // These lists are filled in OnGet and after an invalid form submission.
    public List<Subject> Subjects { get; private set; } = new();
    public List<Reviewer> Reviewers { get; private set; } = new();

    // "OnGetAsync" runs when someone opens the My Reviewers page.
    public async Task OnGetAsync()
    {
        await LoadPageDataAsync();
    }

    // "OnPostAsync" runs when someone submits the Create Reviewer form.
    public async Task<IActionResult> OnPostAsync()
    {
        // Remove spaces from the start and end of the text.
        Title = Title.Trim();
        Description = string.IsNullOrWhiteSpace(Description)
            ? null
            : Description.Trim();
        NewSubjectName = NewSubjectName?.Trim();

        // A new subject name is required only when "+ Add a Subject" was selected.
        if (SubjectId == -1 && string.IsNullOrWhiteSpace(NewSubjectName))
        {
            ModelState.AddModelError(nameof(NewSubjectName), "Enter a name for your new subject.");
        }

        // If a field is missing or invalid, reload the lists and show the page again.
        if (!ModelState.IsValid)
        {
            await LoadPageDataAsync();
            return Page();
        }

        // Get the ID of the person who is currently signed in.
        var ownerId = _userManager.GetUserId(User)!;

        // Use the selected subject, or create a new one for this reviewer.
        Subject subject;

        if (SubjectId == -1)
        {
            // Do not let this user add a subject name they already use.
            var subjectAlreadyExists = await _db.Subjects.AnyAsync(item =>
                item.OwnerId == ownerId &&
                item.Name.ToLower() == NewSubjectName!.ToLower());

            if (subjectAlreadyExists)
            {
                ModelState.AddModelError(nameof(NewSubjectName), "This Subject already exists.");
                await LoadPageDataAsync();
                return Page();
            }

            subject = new Subject
            {
                Name = NewSubjectName!,
                OwnerId = ownerId
            };
        }
        else
        {
            // Check on the server that the chosen subject belongs to this person.
            // This prevents someone from submitting another user's subject ID.
            var existingSubject = await _db.Subjects.FirstOrDefaultAsync(item =>
                item.Id == SubjectId && item.OwnerId == ownerId);

            if (existingSubject is null)
            {
                ModelState.AddModelError(nameof(SubjectId), "Choose one of your subjects.");
                await LoadPageDataAsync();
                return Page();
            }

            subject = existingSubject;
        }

        // Make a new reviewer and link it to this user and their chosen or newly added subject.
        var reviewer = new Reviewer
        {
            Title = Title,
            Description = Description,
            Subject = subject,
            OwnerId = ownerId,
            IsPublic = IsPublic
        };

        // Add the reviewer to PostgreSQL and save the change.
        _db.Reviewers.Add(reviewer);
        await _db.SaveChangesAsync();

        // Redirect after saving so refreshing the page does not submit the form again.
        return RedirectToPage();
    }

    // Delete a reviewer only when it belongs to the signed-in user.
    public async Task<IActionResult> OnPostDeleteAsync(int reviewerId)
    {
        var ownerId = _userManager.GetUserId(User)!;

        var reviewer = await _db.Reviewers.FirstOrDefaultAsync(item =>
            item.Id == reviewerId && item.OwnerId == ownerId);

        if (reviewer is null)
        {
            return NotFound();
        }

        // The database relationship also deletes this reviewer's flashcards.
        _db.Reviewers.Remove(reviewer);
        await _db.SaveChangesAsync();

        return RedirectToPage();
    }

    // Load only this person's subjects and reviewers.
    private async Task LoadPageDataAsync()
    {
        var ownerId = _userManager.GetUserId(User)!;

        // These are the subjects available in the drop-down list.
        Subjects = await _db.Subjects
            .Where(subject => subject.OwnerId == ownerId)
            .OrderBy(subject => subject.Name)
            .ToListAsync();

        // Apply the subject filter only if the subject belongs to this person.
        FilteredSubject = FilterSubjectId.HasValue
            ? Subjects.FirstOrDefault(subject => subject.Id == FilterSubjectId.Value)
            : null;

        if (FilterSubjectId.HasValue && FilteredSubject is null)
        {
            FilterSubjectId = null;
        }

        // Load only this person's reviewers, with their subject and flashcards.
        var reviewerQuery = _db.Reviewers
            .Include(reviewer => reviewer.Subject)
            .Include(reviewer => reviewer.Cards)
            .Where(reviewer => reviewer.OwnerId == ownerId);

        if (FilteredSubject is not null)
        {
            reviewerQuery = reviewerQuery.Where(reviewer => reviewer.SubjectId == FilteredSubject.Id);
        }

        Reviewers = await reviewerQuery
            .OrderByDescending(reviewer => reviewer.CreatedAt)
            .ToListAsync();
    }
}
