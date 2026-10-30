// This line lets us use "PageModel", the starting point for a Razor Page's C# code.
using Microsoft.AspNetCore.Mvc.RazorPages;

// This is the "address" of this code inside your project. It is the same as your other pages.
namespace Rebyulusyon.Pages;

// This is for the C# side of the LOG IN page (Pages/Login.cshtml).
// For now it is only a design preview: it does NOT read the form, check any login details,
// or keep anyone logged in.
public class LoginModel : PageModel
{
    // This is for the yellow notice at the top of the form.
    // It is false when the page first opens, and it becomes true after the form is sent.
    public bool Submitted { get; private set; }

    // "OnGet" runs when someone OPENS the page (for example, by clicking "Log In" in the top bar).
    // It does nothing, because the page only shows the form.
    public void OnGet()
    {
    }

    // "OnPost" runs when someone SENDS the form (by clicking "Log in").
    // We do NOT read the username, email, or password here, so nothing is checked or kept.
    // We only turn on the yellow notice. Real login will be added later.
    public void OnPost()
    {
        Submitted = true;
    }
}