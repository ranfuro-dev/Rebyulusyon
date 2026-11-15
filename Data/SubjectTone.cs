namespace Rebyulusyon.Data;

// Picks one of the three subject colors and keeps the choice stable for the same subject name.
public static class SubjectTone
{
    public static string For(string subjectName)
    {
        var normalizedName = subjectName.Trim().ToUpperInvariant();

        // Match the three example subject labels to their shown colors.
        if (normalizedName == "MATHEMATICS") return "blue";
        if (normalizedName == "SCIENCE") return "red";
        if (normalizedName == "COMPUTER ENGINEERING") return "yellow";

        // Give any other subject a stable, pseudo-random color based on its name.
        // This keeps the color unchanged after refresh without adding a database column.
        unchecked
        {
            uint hash = 2166136261;

            foreach (var character in normalizedName)
            {
                hash ^= character;
                hash *= 16777619;
            }

            return (hash % 3) switch
            {
                0 => "blue",
                1 => "red",
                _ => "yellow"
            };
        }
    }
}
