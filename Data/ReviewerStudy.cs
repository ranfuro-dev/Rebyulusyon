namespace Rebyulusyon.Data;

// Remembers the last time a signed-in user opened a reviewer.
public class ReviewerStudy
{
    public int Id { get; set; }

    // The account that opened the reviewer.
    public string UserId { get; set; } = "";

    // The reviewer can be owned by this user or shared publicly by another user.
    public int ReviewerId { get; set; }

    // Save this in UTC so the dashboard can sort recently opened reviewers.
    public DateTime LastOpenedAtUtc { get; set; } = DateTime.UtcNow;
}
