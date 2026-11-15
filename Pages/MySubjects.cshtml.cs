// This line lets us validate the subject name.
using System.ComponentModel.DataAnnotations;

// This line lets us read and save subjects in PostgreSQL.
using Microsoft.EntityFrameworkCore;

// This line lets us get the ID of the signed-in user.
using Microsoft.AspNetCore.Identity;

// This line lets us use "IActionResult", "BindProperty", and "PageModel".
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

// This line lets us use our database and model classes.
using Rebyulusyon.Data;

namespace Rebyulusyon.Pages;

// This is the C# side of the My Subjects page.
public class MySubjectsModel : PageModel
{
    private readonly ApplicationDbContext _db;
    private readonly UserManager<ApplicationUser> _userManager;

    public MySubjectsModel(
        ApplicationDbContext db,
        UserManager<ApplicationUser> userManager)
    {
        _db = db;
        _userManager = userManager;
    }

    // This receives the name entered in the form.
    [BindProperty]
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = "";

    // These are only the subjects owned by the signed-in user.
    public List<Subject> Subjects { get; private set; } = new();

    public async Task OnGetAsync()
    {
        await LoadSubjectsAsync();
    }

    // This runs when the Add Subject form is submitted.
    public async Task<IActionResult> OnPostAsync()
    {
        Name = Name.Trim();

        if (!ModelState.IsValid)
        {
            await LoadSubjectsAsync();
            return Page();
        }

        var ownerId = _userManager.GetUserId(User)!;

        // Do not let this user create the same subject name twice.
        var subjectAlreadyExists = await _db.Subjects.AnyAsync(subject =>
            subject.OwnerId == ownerId &&
            subject.Name.ToLower() == Name.ToLower());

        if (subjectAlreadyExists)
        {
            ModelState.AddModelError(nameof(Name), "This Subject already exists.");
            await LoadSubjectsAsync();
            return Page();
        }

        var subject = new Subject
        {
            Name = Name,
            OwnerId = ownerId
        };

        _db.Subjects.Add(subject);
        await _db.SaveChangesAsync();

        // Redirect after saving so refreshing the page does not submit the form again.
        return RedirectToPage();
    }

    // Delete a subject only when it belongs to the signed-in user.
    public async Task<IActionResult> OnPostDeleteAsync(int subjectId)
    {
        var ownerId = _userManager.GetUserId(User)!;

        var subject = await _db.Subjects.FirstOrDefaultAsync(item =>
            item.Id == subjectId && item.OwnerId == ownerId);

        if (subject is null)
        {
            return NotFound();
        }

        // The database relationships also delete this subject's reviewers and their flashcards.
        _db.Subjects.Remove(subject);
        await _db.SaveChangesAsync();

        return RedirectToPage();
    }

    // Always filter by the signed-in user's ID so people only see their own subjects.
    private async Task LoadSubjectsAsync()
    {
        var ownerId = _userManager.GetUserId(User)!;

        Subjects = await _db.Subjects
            .Where(subject => subject.OwnerId == ownerId)
            .OrderBy(subject => subject.Name)
            .ToListAsync();
    }
}
