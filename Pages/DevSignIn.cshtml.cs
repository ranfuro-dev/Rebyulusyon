// This line lets us use the Claim and ClaimsIdentity types (the facts about the person who is logged in).
using System.Security.Claims;

// This line lets us use SignInAsync, which gives the browser a login cookie.
using Microsoft.AspNetCore.Authentication;

// This line lets us use "IActionResult", which lets a page answer with a redirect or "not found".
using Microsoft.AspNetCore.Mvc;

// This line lets us use "PageModel", the starting point for a Razor Page's C# code.
using Microsoft.AspNetCore.Mvc.RazorPages;

// This line lets us use the name of the login cookie that protects private pages.
using Microsoft.AspNetCore.Identity;

// This is the "address" of this code inside your project. It is the same as your other pages.
namespace Rebyulusyon.Pages;

// ==========================================================
// TEMPORARY, ONLY FOR DEVELOPMENT. DELETE THIS FILE (and DevSignIn.cshtml) when the real login is built.
// It lets you open /DevSignIn on your own computer to be logged in as a PRETEND student ("ranfuro"),
// so you can look at the pages that need a login, and test Log out.
// It is NOT a real login: there is no password. That is why it only works while the website runs in "Development"
// (when you press Ctrl+F5 in Visual Studio). On a real website, it answers "not found" and does nothing.
// ==========================================================
public class DevSignInModel : PageModel
{
    private readonly IWebHostEnvironment _environment;

    // ASP.NET gives us the information about where the website is running.
    public DevSignInModel(IWebHostEnvironment environment)
    {
        _environment = environment;
    }

    public async Task<IActionResult> OnGetAsync()
    {
        // THE SAFETY RULE: outside of Development, this page does not exist.
        if (!_environment.IsDevelopment())
        {
            return NotFound();
        }

        // The facts about the pretend student: an id, and the username.
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, "dev-student-1"),
            new(ClaimTypes.Name, "ranfuro")
        };
        var identity = new ClaimsIdentity(claims, "DevSignIn");

        // This gives the browser the SAME login cookie that protected pages check.
        // Without the cookie name, ASP.NET may use a different cookie that does not unlock the Dashboard.
        await HttpContext.SignInAsync(
            IdentityConstants.ApplicationScheme,
            new ClaimsPrincipal(identity));

        // Like after a real login: go to the Dashboard.
        return RedirectToPage("/Dashboard");
    }
}