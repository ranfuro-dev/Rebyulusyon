using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Rebyulusyon.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

// ----- Database -----
// Reads the connection string from appsettings.json and connects to SQL Server.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString));

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

app.UseAuthentication();   // "who is this visitor?" (reads the login cookie). MUST come before UseAuthorization.
app.UseAuthorization();    // "are they allowed in?"

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

app.Run();