// This line lets us use "IActionResult", which lets a page answer with a redirect.
using Microsoft.AspNetCore.Mvc;

// This line lets us use "PageModel", the starting point for a Razor Page's C# code.
using Microsoft.AspNetCore.Mvc.RazorPages;

// This is the "address" of this code inside your project. It is the same as your other pages.
namespace Rebyulusyon.Pages;

// This is for the C# side of the GET STARTED (REGISTER) page (Pages/Register.cshtml).
// The form is still only a design preview: it does NOT read the form, save anything, or create an account.
public class RegisterModel : PageModel
{
    // This is for the yellow notice at the top of the form.
    // It is false when the page first opens, and it becomes true after the form is sent.
    public bool Submitted { get; private set; }

    // "OnGet" runs when someone OPENS the page (for example, by clicking "Get Started").
    // If the person is already logged in, there is no reason to show this page, so we send them to the Dashboard.
    public IActionResult OnGet()
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectToPage("/Dashboard");
        }

        return Page();
    }

    // "OnPost" runs when someone SENDS the form (by clicking "Create account").
    // We do NOT read the username, email, or password here, so nothing is saved or kept.
    public IActionResult OnPost()
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectToPage("/Dashboard");
        }

        Submitted = true;

        // LATER, when the real sign-up is built: after the account is created and the person is signed in,
        // the next line is what sends them to the Dashboard:
        //     return RedirectToPage("/Dashboard");

        return Page();
    }
}