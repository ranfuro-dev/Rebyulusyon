namespace Rebyulusyon.Data;

// One subject belongs to one user and can contain several reviewers.
public class Subject
{
    public int Id { get; set; }

    public string Name { get; set; } = "";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // This color is calculated from the name, so it stays consistent without a database column.
    [System.ComponentModel.DataAnnotations.Schema.NotMapped]
    public string Tone => SubjectTone.For(Name);

    // The user who created this subject.
    public string OwnerId { get; set; } = "";
    public ApplicationUser Owner { get; set; } = null!;

    // The reviewers organized under this subject.
    public ICollection<Reviewer> Reviewers { get; set; } = new List<Reviewer>();
}
