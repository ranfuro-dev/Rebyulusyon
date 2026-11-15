// This line lets us check passwords and find the user's account.
using Microsoft.AspNetCore.Identity;

// This line lets us check that required form fields were entered.
using System.ComponentModel.DataAnnotations;

// This line lets us use "IActionResult" and redirects.
using Microsoft.AspNetCore.Mvc;

// This line lets us use "BindProperty" and "PageModel".
using Microsoft.AspNetCore.Mvc.RazorPages;

// This line lets us use our custom account class.
using Rebyulusyon.Data;

// This is the "address" of this code inside your project.
namespace Rebyulusyon.Pages;

// This is for the C# side of the LOG IN page.
public class LoginModel : PageModel
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;

    // ASP.NET gives us the account tools needed to find the user and check their password.
    public LoginModel(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    // These properties receive the values entered in the login form.
    [BindProperty]
    [Required(ErrorMessage = "Please enter your username or email address.")]
    public string UserNameOrEmail { get; set; } = "";

    [BindProperty]
    [Required(ErrorMessage = "Please enter your password.")]
    public string Password { get; set; } = "";

    [BindProperty]
    public bool RememberMe { get; set; }

    // This is the page the visitor wanted before they were sent to Login.
    [BindProperty(SupportsGet = true)]
    public string? ReturnUrl { get; set; }

    // If someone is already signed in, send them to their requested page or the Dashboard.
    public IActionResult OnGet()
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectAfterLogin();
        }

        return Page();
    }

    // This runs when someone submits the login form.
    public async Task<IActionResult> OnPostAsync()
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectAfterLogin();
        }

        if (!ModelState.IsValid)
        {
            return Page();
        }

        // An address containing "@" is treated as an email; otherwise it is treated as a username.
        var loginName = UserNameOrEmail.Trim();
        var user = loginName.Contains('@')
            ? await _userManager.FindByEmailAsync(loginName)
            : await _userManager.FindByNameAsync(loginName);

        if (user is null)
        {
            ModelState.AddModelError(string.Empty, "Invalid username/email or password.");
            return Page();
        }

        // Identity checks the password and applies the configured lockout rules.
        var result = await _signInManager.PasswordSignInAsync(
            user.UserName!,
            Password,
            RememberMe,
            lockoutOnFailure: true);

        if (result.Succeeded)
        {
            return RedirectAfterLogin();
        }

        if (result.IsLockedOut)
        {
            ModelState.AddModelError(string.Empty, "Too many unsuccessful attempts. Try again later.");
        }
        else
        {
            ModelState.AddModelError(string.Empty, "Invalid username/email or password.");
        }

        return Page();
    }

    // Only send the visitor to a local page; otherwise open the Dashboard.
    private IActionResult RedirectAfterLogin()
    {
        if (!string.IsNullOrEmpty(ReturnUrl) && Url.IsLocalUrl(ReturnUrl))
        {
            return LocalRedirect(ReturnUrl);
        }

        return RedirectToPage("/Dashboard");
    }
}