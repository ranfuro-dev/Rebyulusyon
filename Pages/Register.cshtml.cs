// This line lets us check the password and save the new account.
using Microsoft.AspNetCore.Identity;

// This line lets us check form input such as required fields and matching passwords.
using System.ComponentModel.DataAnnotations;

// This line lets us use "IActionResult", which lets a page answer with a redirect.
using Microsoft.AspNetCore.Mvc;

// This line lets us use "BindProperty" and "PageModel".
using Microsoft.AspNetCore.Mvc.RazorPages;

// This line lets us use our custom account class.
using Rebyulusyon.Data;

// This is the "address" of this code inside your project.
namespace Rebyulusyon.Pages;

// This is for the C# side of the GET STARTED (REGISTER) page.
public class RegisterModel : PageModel
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;

    // ASP.NET gives us the account tools we need to create the account and sign in.
    public RegisterModel(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    // These properties receive the values typed into the registration form.
    [BindProperty]
    [Required]
    [StringLength(20, MinimumLength = 3)]
    public string Username { get; set; } = "";

    [BindProperty]
    [Required]
    [EmailAddress]
    public string Email { get; set; } = "";

    [BindProperty]
    [Required]
    [MinLength(8)]
    public string Password { get; set; } = "";

    [BindProperty]
    [Required]
    [Compare(nameof(Password), ErrorMessage = "The passwords do not match.")]
    public string ConfirmPassword { get; set; } = "";

    // If someone is already signed in, send them to the Dashboard.
    public IActionResult OnGet()
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectToPage("/Dashboard");
        }

        return Page();
    }

    // This runs when someone submits the registration form.
    public async Task<IActionResult> OnPostAsync()
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectToPage("/Dashboard");
        }

        if (!ModelState.IsValid)
        {
            return Page();
        }

        // Make the username and email consistent by removing extra spaces.
        var user = new ApplicationUser
        {
            UserName = Username.Trim(),
            Email = Email.Trim()
        };

        // ASP.NET Identity checks the password rules and saves the account to PostgreSQL.
        var result = await _userManager.CreateAsync(user, Password);

        if (!result.Succeeded)
        {
            // Show Identity errors, such as a username or email that is already in use.
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return Page();
        }

        // Sign in the new account and open the Dashboard.
        await _signInManager.SignInAsync(user, isPersistent: false);
        return RedirectToPage("/Dashboard");
    }
}