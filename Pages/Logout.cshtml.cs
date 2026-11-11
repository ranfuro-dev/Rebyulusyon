// This line lets us use the name of the login cookie that we want to remove.
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;

// This line lets us use "IActionResult", which lets a page answer with a redirect.
using Microsoft.AspNetCore.Mvc;

// This line lets us use "PageModel", the starting point for a Razor Page's C# code.
using Microsoft.AspNetCore.Mvc.RazorPages;

// This is the "address" of this code inside project.
namespace Rebyulusyon.Pages;

// This is the C# side of LOG OUT (the page at "/Logout").
public class LogoutModel : PageModel
{
    // Typing /Logout in the address bar only sends the person Home.
    // Logging out happens only when the form sends a POST.
    public IActionResult OnGet()
    {
        return LocalRedirect("/");
    }

    // This runs when the Log out form is submitted.
    public async Task<IActionResult> OnPostAsync()
    {
        // Remove the application login cookie that protects the dashboard pages.
        await HttpContext.SignOutAsync(IdentityConstants.ApplicationScheme);

        // Send the person to the Home page.
        return LocalRedirect("/");
    }
}