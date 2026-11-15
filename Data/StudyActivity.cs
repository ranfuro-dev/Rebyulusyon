namespace Rebyulusyon.Data;

// One saved answer in a user's study history.
public class StudyActivity
{
    public int Id { get; set; }

    // The account that answered the flashcard.
    public string UserId { get; set; } = "";

    // The flashcard that was rated.
    public int FlashcardId { get; set; }

    // True means "I got it"; false means "I missed it".
    public bool GotIt { get; set; }

    // Store the answer time in UTC so chart days stay consistent.
    public DateTime StudiedAtUtc { get; set; } = DateTime.UtcNow;
}
