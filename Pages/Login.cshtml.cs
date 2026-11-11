// This line lets us use "IActionResult", which lets a page answer with a redirect.
using Microsoft.AspNetCore.Mvc;

// This line lets us use "PageModel", the starting point for a Razor Page's C# code.
using Microsoft.AspNetCore.Mvc.RazorPages;

// This is the "address" of this code inside your project. It is the same as your other pages.
namespace Rebyulusyon.Pages;

// This is for the C# side of the LOG IN page (Pages/Login.cshtml).
// The form is still only a design preview: it does NOT check any login details yet.
public class LoginModel : PageModel
{
    // This is for the yellow notice at the top of the form.
    // It is false when the page first opens, and it becomes true after the form is sent.
    public bool Submitted { get; private set; }

    // This is the page the person wanted to open before they were sent to the Login page (for example /Dashboard).
    // The website fills it in by itself, from the web address: /Login?ReturnUrl=%2FDashboard
    [BindProperty(SupportsGet = true)]
    public string? ReturnUrl { get; set; }

    // "OnGet" runs when someone OPENS the page.
    // If the person is already logged in, there is no reason to show the Login page, so we send them to the Dashboard.
    public IActionResult OnGet()
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectToPage("/Dashboard");
        }

        return Page();
    }

    // "OnPost" runs when someone SENDS the form (by clicking "Log in").
    // We do NOT read the username or the password here, so nothing is checked or kept.
    public IActionResult OnPost()
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectToPage("/Dashboard");
        }

        Submitted = true;

        // LATER, when the real login is built: after the password is checked and the person is signed in,
        // the next line is what sends them to the Dashboard:
        //     return RedirectAfterLogin();

        return Page();
    }

    // THE RULE FOR "WHERE TO GO AFTER A SUCCESSFUL LOGIN". It is ready, but nothing calls it yet,
    // because there is no real login yet.
    // If the person was sent here from a protected page, go back to that page. Otherwise go to the Dashboard.
    // "Url.IsLocalUrl" only allows addresses inside this website, so nobody can use ReturnUrl to send people to another website.
    private IActionResult RedirectAfterLogin()
    {
        if (!string.IsNullOrEmpty(ReturnUrl) && Url.IsLocalUrl(ReturnUrl))
        {
            return LocalRedirect(ReturnUrl);
        }

        return RedirectToPage("/Dashboard");
    }
}