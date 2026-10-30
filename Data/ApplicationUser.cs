using Microsoft.AspNetCore.Identity;

namespace Rebyulusyon.Data;

// The account of one person.
// IdentityUser already includes: Id, UserName, Email, PasswordHash (the scrambled
// password, never the real one), lockout fields, and more.
// We make our own class so we can add extra fields later without redoing the database.
public class ApplicationUser : IdentityUser
{
    // All reviewers this person created (used later for "My reviewers").
    public ICollection<Reviewer> Reviewers { get; set; } = new List<Reviewer>();
}