namespace Rebyulusyon.Data;

// One reviewer (a set of flashcards).
// This is only the foundation: no page uses it yet.
public class Reviewer
{
    public int Id { get; set; }

    public string Title { get; set; } = "";
    public string? Description { get; set; }    // the "?" means this one may be empty

    // false = private: only the owner can open it. This is the default, so nothing is
    // shared by accident. true = public: other users can discover it.
    public bool IsPublic { get; set; } = false;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // The creator's user ID. Later, cards will show "Created by @" + Owner.UserName.
    // When we build reviewer pages, EVERY query must be filtered on the server, for example:
    //   reviewers.Where(r => r.IsPublic || r.OwnerId == currentUserId)
    public string OwnerId { get; set; } = "";
    public ApplicationUser Owner { get; set; } = null!;
}