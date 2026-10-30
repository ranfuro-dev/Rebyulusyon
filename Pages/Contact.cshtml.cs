// This line lets us use "PageModel", the starting point for a Razor Page's C# code.
using Microsoft.AspNetCore.Mvc.RazorPages;

// This is the "address" of this code inside your project. It is the same as your other pages.
namespace Rebyulusyon.Pages;

// This is for the C# side of the CONTACT page (Pages/Contact.cshtml).
// For now it is only a design preview: it does NOT read the form, send an email, or save any message.
public class ContactModel : PageModel
{
    // This is for the yellow notice at the top of the form.
    // It is false when the page first opens, and it becomes true after the form is sent.
    public bool Submitted { get; private set; }

    // "OnGet" runs when someone OPENS the page (for example, by clicking "Contact" in the footer).
    // It does nothing, because the page only shows the form.
    public void OnGet()
    {
    }

    // "OnPost" runs when someone SENDS the form (by clicking "Send message").
    // We do NOT read the name, email, subject, or message here, so nothing is sent or kept.
    // We only turn on the yellow notice. Real sending will be added later.
    public void OnPost()
    {
        Submitted = true;
    }
}