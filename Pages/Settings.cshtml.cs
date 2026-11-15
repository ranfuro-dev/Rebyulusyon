using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Rebyulusyon.Data;

namespace Rebyulusyon.Pages;

[Authorize]
public class SettingsModel : PageModel
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;

    public SettingsModel(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    public string CurrentUsername { get; private set; } = "";

    [BindProperty] public string NewUsername { get; set; } = "";
    [BindProperty] public string UsernamePassword { get; set; } = "";
    [BindProperty] public int DailyGoal { get; set; } = 20;
    [BindProperty] public int WeekStartsOnDay { get; set; }
    [BindProperty] public string CurrentPassword { get; set; } = "";
    [BindProperty] public string NewPassword { get; set; } = "";
    [BindProperty] public string ConfirmNewPassword { get; set; } = "";

    public async Task OnGetAsync()
    {
        await LoadAccountAsync();
        NewUsername = CurrentUsername;
    }

    // Change the username after checking the current password.
    public async Task<IActionResult> OnPostChangeUsernameAsync()
    {
        var user = await GetCurrentUserAsync();
        if (string.IsNullOrWhiteSpace(NewUsername) || NewUsername.Trim().Length is < 3 or > 20)
        {
            ModelState.AddModelError(nameof(NewUsername), "Username must be between 3 and 20 characters.");
            return await ReturnPageAsync();
        }

        if (!await _userManager.CheckPasswordAsync(user, UsernamePassword))
        {
            ModelState.AddModelError(nameof(UsernamePassword), "Your current password is incorrect.");
            return await ReturnPageAsync();
        }

        var result = await _userManager.SetUserNameAsync(user, NewUsername.Trim());
        if (!result.Succeeded)
        {
            AddIdentityErrors(result);
            return await ReturnPageAsync();
        }

        await _signInManager.RefreshSignInAsync(user);
        TempData["ProfileMessage"] = "Your username has been updated.";
        return RedirectToPage();
    }

    // Save the card goal and selected first weekday to this user's account.
    public async Task<IActionResult> OnPostSaveStudyPreferencesAsync()
    {
        if (DailyGoal is < 5 or > 100 || DailyGoal % 5 != 0)
        {
            ModelState.AddModelError(nameof(DailyGoal), "Choose a daily goal from 5 to 100 in steps of 5.");
            return await ReturnPageAsync();
        }

        if (WeekStartsOnDay is < 0 or > 6)
        {
            ModelState.AddModelError(nameof(WeekStartsOnDay), "Choose a day of the week.");
            return await ReturnPageAsync();
        }

        var user = await GetCurrentUserAsync();
        user.DailyGoal = DailyGoal;
        user.WeekStartsOnDay = WeekStartsOnDay;
        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            AddIdentityErrors(result);
            return await ReturnPageAsync();
        }

        TempData["PreferencesMessage"] = "Your study preferences have been saved.";
        return RedirectToPage();
    }

    // Change the password through Identity so its password rules and security stamp are applied.
    public async Task<IActionResult> OnPostChangePasswordAsync()
    {
        if (string.IsNullOrWhiteSpace(NewPassword) || NewPassword != ConfirmNewPassword)
        {
            ModelState.AddModelError(nameof(ConfirmNewPassword), "The new passwords do not match.");
            return await ReturnPageAsync();
        }

        var user = await GetCurrentUserAsync();
        var result = await _userManager.ChangePasswordAsync(user, CurrentPassword, NewPassword);
        if (!result.Succeeded)
        {
            AddIdentityErrors(result);
            return await ReturnPageAsync();
        }

        await _signInManager.RefreshSignInAsync(user);
        TempData["SecurityMessage"] = "Your password has been changed.";
        return RedirectToPage();
    }

    private async Task<ApplicationUser> GetCurrentUserAsync() =>
        await _userManager.GetUserAsync(User)
        ?? throw new InvalidOperationException("The signed-in account could not be found.");

    private async Task LoadAccountAsync()
    {
        var user = await GetCurrentUserAsync();
        CurrentUsername = user.UserName ?? "";
        DailyGoal = user.DailyGoal;
        WeekStartsOnDay = user.WeekStartsOnDay;
    }

    private async Task<IActionResult> ReturnPageAsync()
    {
        await LoadAccountAsync();
        return Page();
    }

    private void AddIdentityErrors(IdentityResult result)
    {
        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error.Description);
        }
    }
}
