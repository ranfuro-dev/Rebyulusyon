// Lets us use the Razor Pages base class called PageModel
using Microsoft.AspNetCore.Mvc.RazorPages;

// CHANGE THIS to match the @namespace line in Pages/_ViewImports.cshtml.
// Example: if that file says "@namespace MyApp.Pages", write "namespace MyApp.Pages;"
namespace Rebyulusyon.Pages;

// IndexModel is the C# side of Index.cshtml.
// ": PageModel" means it inherits all the page features from Microsoft's base class.
public class IndexModel : PageModel
{
    // OnGet runs when someone opens the Home Page.
    // It is empty for now. Later it will load data for the page.
    public void OnGet()
    {
    }
}