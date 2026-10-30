// This line lets us use "PageModel", the starting point for a Razor Page's C# code.
using Microsoft.AspNetCore.Mvc.RazorPages;

// This is the "address" of this code inside my project. It is the same as my other pages.
namespace Rebyulusyon.Pages;

// This is for the C# side of the GET STARTED (REGISTER) page (Pages/Register.cshtml).
// For now it is only a design preview: it does NOT read the form, save anything, or create an account.
public class RegisterModel : PageModel
{
    // This is for the yellow notice at the top of the form.
    // It is false when the page first opens, and it becomes true after the form is sent.
    public bool Submitted { get; private set; }

    // "OnGet" runs when someone OPENS the page (for example, by clicking "Get Started").
    // It does nothing, because the page only shows the form.
    public void OnGet()
    {
    }

    // "OnPost" runs when someone SENDS the form (by clicking "Create account").
    // We do NOT read the username, email, or password here, so nothing is saved or kept.
    // We only turn on the yellow notice. Real sign-up will be added later.
    public void OnPost()
    {
        Submitted = true;
    }
}