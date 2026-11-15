namespace Rebyulusyon.Data;

// One flashcard belongs to one reviewer.
public class Flashcard
{
    public int Id { get; set; }

    public string Question { get; set; } = "";
    public string Answer { get; set; } = "";

    // This controls the order cards appear during study.
    public int SortOrder { get; set; }

    public int ReviewerId { get; set; }
    public Reviewer Reviewer { get; set; } = null!;
}