using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.RateLimiting;
using Rebyulusyon.Data;
using System.Threading.RateLimiting;
using System.Xml.Linq;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages(options =>
{
    // These pages are ONLY for people who are logged in.
    // Everyone else is sent to the Login page (the "LoginPath" set in the login cookie below).
    // When you build a new page for the dashboard, add its name to this list.
    options.Conventions.AuthorizePage("/Dashboard");
    options.Conventions.AuthorizePage("/MySubjects");
    options.Conventions.AuthorizePage("/MyReviewers");
    options.Conventions.AuthorizePage("/Progress");
    options.Conventions.AuthorizePage("/Settings");
});

// Slow automated Contact form submissions without limiting normal page browsing.
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    options.OnRejected = async (context, cancellationToken) =>
    {
        context.HttpContext.Response.ContentType = "text/plain; charset=utf-8";
        await context.HttpContext.Response.WriteAsync(
            "Too many messages were sent. Please wait a little while and try again.",
            cancellationToken);
    };

    options.AddPolicy("ContactMessages", httpContext =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 5,
                Window = TimeSpan.FromMinutes(10),
                QueueLimit = 0,
                AutoReplenishment = true
            }));
});

// ----- Database -----
// Reads the connection string from appsettings.json and connects to PostgreSQL.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(connectionString));

// ----- Accounts (ASP.NET Core Identity) -----
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    // Password rules (checked on the server)
    options.Password.RequiredLength = 8;
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = false;   // symbols are allowed, not forced

    // Every email can be used by only one account
    options.User.RequireUniqueEmail = true;

    // Usernames: letters, numbers, underscore and period only.
    // "@" is NOT allowed, which lets the login box tell an email apart from a username.
    options.User.AllowedUserNameCharacters =
        "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789_.";

    // After 5 wrong passwords, lock the account for 5 minutes (slows down password guessing)
    options.Lockout.AllowedForNewUsers = true;
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
})
.AddEntityFrameworkStores<ApplicationDbContext>()   // store accounts in our database
.AddDefaultTokenProviders();

// ----- The login cookie -----
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Login";            // where signed-out visitors are sent
    options.AccessDeniedPath = "/Login";
    options.Cookie.HttpOnly = true;          // JavaScript cannot read the cookie
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;   // sent over HTTPS only
    options.Cookie.SameSite = SameSiteMode.Lax;                // helps block cross-site attacks
    options.ExpireTimeSpan = TimeSpan.FromDays(14);            // used when "Remember me" is checked
    options.SlidingExpiration = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();
app.UseRateLimiter();

app.UseAuthentication();   // "who is this visitor?" (reads the login cookie). MUST come before UseAuthorization.
app.UseAuthorization();    // "are they allowed in?"

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

// Give search engines a sitemap containing only the public pages.
app.MapGet("/sitemap.xml", (HttpRequest request) =>
{
    var origin = $"{request.Scheme}://{request.Host}";
    XNamespace sitemapNamespace = "http://www.sitemaps.org/schemas/sitemap/0.9";
    var publicPaths = new[] { "/", "/About", "/Reviewers", "/Contact", "/Privacy" };
    var sitemap = new XDocument(
        new XElement(sitemapNamespace + "urlset",
            publicPaths.Select(path => new XElement(sitemapNamespace + "url",
                new XElement(sitemapNamespace + "loc", $"{origin}{path}")))));

    return Results.Text(sitemap.ToString(SaveOptions.DisableFormatting), "application/xml; charset=utf-8");
});

// Keep member pages out of search results and point crawlers to the public sitemap.
app.MapGet("/robots.txt", (HttpRequest request) =>
{
    var sitemapUrl = $"{request.Scheme}://{request.Host}/sitemap.xml";
    var robots = $"User-agent: *\nAllow: /\nDisallow: /Dashboard\nDisallow: /MySubjects\nDisallow: /MyReviewers\nDisallow: /MyReviewerDetails\nDisallow: /ReviewerCards\nDisallow: /Progress\nDisallow: /Settings\nDisallow: /Study\nDisallow: /FlashcardStudy\nSitemap: {sitemapUrl}\n";
    return Results.Text(robots, "text/plain; charset=utf-8");
});

// Send unknown page URLs to a friendly page that still returns the correct 404 status.
app.MapFallbackToPage("/NotFound");

app.Run();
