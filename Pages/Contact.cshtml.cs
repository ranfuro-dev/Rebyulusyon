// This line lets us use the Razor Pages features for the Contact page.
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Net.Mail;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

// This is the address of the Contact page code in this project.
namespace Rebyulusyon.Pages;

// This is the C# side of the Contact page. It checks the form and sends messages by email.
public class ContactModel : PageModel
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<ContactModel> _logger;

    // These values are filled from the matching boxes in Contact.cshtml after the form is sent.
    [BindProperty]
    [Required(ErrorMessage = "Please enter your username.")]
    [StringLength(20)]
    public string Username { get; set; } = "";

    [BindProperty]
    [Required(ErrorMessage = "Please choose a subject.")]
    [StringLength(100)]
    public string Subject { get; set; } = "";

    [BindProperty]
    [Required(ErrorMessage = "Please enter a message.")]
    [StringLength(1000, MinimumLength = 10, ErrorMessage = "Your message must be between 10 and 1000 characters.")]
    public string Message { get; set; } = "";

    // These messages let the page tell the visitor whether the email was sent.
    public string? SuccessMessage { get; private set; }
    public string? ErrorMessage { get; private set; }

    // ASP.NET provides the private settings and logger when it creates this page.
    public ContactModel(IConfiguration configuration, ILogger<ContactModel> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    // "OnGet" runs when someone opens the Contact page.
    public void OnGet()
    {
    }

    // "OnPostAsync" runs when someone clicks "Send message".
    public async Task<IActionResult> OnPostAsync()
    {
        // Do not try sending if any required form value is missing or invalid.
        if (!ModelState.IsValid)
        {
            ErrorMessage = "Please check the form and correct the highlighted fields.";
            return Page();
        }

        // Read the mail server settings. The password and addresses are kept in User Secrets.
        var emailSettings = _configuration.GetSection("Email");
        var host = emailSettings["SmtpHost"];
        var username = emailSettings["SmtpUsername"];
        var password = emailSettings["SmtpPassword"];
        var fromAddress = emailSettings["FromAddress"];
        var toAddress = emailSettings["ToAddress"];
        var portText = emailSettings["SmtpPort"];

        // Show a safe setup message if any required setting has not been added yet.
        if (string.IsNullOrWhiteSpace(host)
            || string.IsNullOrWhiteSpace(username)
            || string.IsNullOrWhiteSpace(password)
            || string.IsNullOrWhiteSpace(fromAddress)
            || string.IsNullOrWhiteSpace(toAddress)
            || !int.TryParse(portText, out var port))
        {
            _logger.LogError("Contact email settings are missing or invalid.");
            ErrorMessage = "Email is not configured yet. Please try again later.";
            return Page();
        }

        try
        {
            // Build a plain-text email so visitor-provided text is never treated as HTML.
            using var mail = new MailMessage
            {
                From = new MailAddress(fromAddress, "Rebyulusyon Contact Form"),
                Subject = $"Rebyulusyon contact: {Subject.Trim()}",
                Body = $"Username: {Username.Trim()}\nSubject: {Subject.Trim()}\n\nMessage:\n{Message.Trim()}",
                IsBodyHtml = false
            };
            mail.To.Add(new MailAddress(toAddress));

            // Send the email using the SMTP account configured in User Secrets.
            using var smtp = new SmtpClient(host, port)
            {
                EnableSsl = true,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(username, password)
            };

            await smtp.SendMailAsync(mail);
            SuccessMessage = "Your message was sent. Thank you for contacting us!";

            // Clear the form after a successful send so it is ready for another message.
            ModelState.Clear();
            Username = "";
            Subject = "";
            Message = "";
        }
        catch (Exception exception)
        {
            // Keep technical mail-server details in the server log, not on the public page.
            _logger.LogError(exception, "The Contact form email could not be sent.");
            ErrorMessage = "We could not send your message right now. Please try again later.";
        }

        return Page();
    }
}
