using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Rebyulusyon.Pages;

// Return a real 404 status while showing the visitor a useful page.
public class NotFoundModel : PageModel
{
    public void OnGet()
    {
        Response.StatusCode = StatusCodes.Status404NotFound;
    }
}
