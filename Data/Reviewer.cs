namespace Rebyulusyon.Data;

// One reviewer belongs to a subject and contains flashcards.
public class Reviewer
{
    public int Id { get; set; }

    public string Title { get; set; } = "";
    public string? Description { get; set; }

    // False means private. Nothing is shared unless the owner makes it public.
    public bool IsPublic { get; set; } = false;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // The subject this reviewer is organized under.
    public int SubjectId { get; set; }
    public Subject Subject { get; set; } = null!;

    // The creator of this reviewer.
    public string OwnerId { get; set; } = "";
    public ApplicationUser Owner { get; set; } = null!;

    // The flashcards in this reviewer.
    public ICollection<Flashcard> Cards { get; set; } = new List<Flashcard>();
}