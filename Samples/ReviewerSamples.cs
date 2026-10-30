// This is the "address" of this code inside your project.
// The folder is called Samples, so the address ends with ".Samples".
namespace Rebyulusyon.Samples;

// This describes ONE sample reviewer. A "record" is a quick way to make a small box of information.
// Id = a number for each reviewer (used in the web address, like /ReviewerDetails/5)
// Title, Category, Description = the words about the reviewer
// CardCount = how many flashcards it has
// Creator = the username of the person who made it (shown as "Created by @username")
// IsPublic = true means everyone can see it. false means it is PRIVATE.
public record SampleReviewer(int Id, string Title, string Category, string Description, int CardCount, string Creator, bool IsPublic);

// This describes ONE flashcard: the question on the front and the answer on the back.
public record SampleCard(string Question, string Answer);

// This is for ALL the sample reviewers, in one place.
// Both the Explore Reviewers page and the Reviewer Details page read from here.
// If you want to change a sample reviewer, change it HERE, and both pages will use the new version.
// NOTE: everything in this file is TEMPORARY. It does NOT come from a database.
// Later, the database will replace this file.
public static class ReviewerSamples
{
    // This is the list of all sample reviewers.
    // The first 12 are PUBLIC (the last word on each line is true).
    // The last 4 are PRIVATE (the last word is false). They are here to show that private reviewers are never shown to other people.
    public static readonly IReadOnlyList<SampleReviewer> Reviewers = new List<SampleReviewer>
    {
        // --- PUBLIC reviewers (everyone can see these) ---
        new(1,  "Algebra Basics: Factoring Practice",       "Mathematics",          "Practice factoring trinomials, perfect squares, and common factors with quick questions.",      24, "ranfuro",        true),
        new(2,  "Trigonometry: Identities and the Unit Circle", "Mathematics",      "Review the key identities, angle formulas, and unit circle values one card at a time.",         30, "maria_santos",   true),
        new(3,  "Calculus: Limits and Derivatives",         "Mathematics",          "Learn the limit laws and the basic derivative rules, then test yourself with examples.",         28, "kuya_ben",       true),
        new(4,  "Physics: Motion and Forces",               "Science",              "Go over velocity, acceleration, and Newton's laws with short definition and problem cards.",     22, "ate_lyn",        true),
        new(5,  "Chemistry: Periodic Table Trends",         "Science",              "Compare atomic size, electronegativity, and ionization energy across groups and periods.",       18, "claire_studies", true),
        new(6,  "Biology: Cell Parts and Functions",        "Science",              "Match each cell part to its job, from the nucleus to the mitochondria.",                         26, "ranfuro",        true),
        new(7,  "Digital Logic: Gates and Truth Tables",    "Computer Engineering", "Learn the AND, OR, NOT, NAND, NOR, and XOR gates and how to read their truth tables.",           20, "dev.paolo",      true),
        new(8,  "Number Systems and Conversions",           "Computer Engineering", "Convert between binary, octal, decimal, and hexadecimal with worked examples.",                 16, "nikka_r",        true),
        new(9,  "C# Basics: Variables and Loops",           "Programming",          "Review data types, variables, if statements, and for and while loops in C#.",                    25, "jmendoza",       true),
        new(10, "Python Lists, Tuples, and Dictionaries",   "Programming",          "Know when to use each collection type and the common methods that go with them.",               21, "dev.paolo",      true),
        new(11, "Philippine History: Key Events Timeline",  "General Education",    "Remember the major events from the pre-colonial period to modern times, in order.",             32, "maria_santos",   true),
        new(12, "Understanding the Self: Key Terms",        "General Education",    "Review the main ideas and thinkers from your Understanding the Self course.",                   19, "ate_lyn",        true),

        // --- PRIVATE reviewers (only their creators should ever see these) ---
        new(13, "Board Exam Weak Topics",                   "Science",              "My own list of topics that I keep forgetting.",                                                  14, "ranfuro",        false),
        new(14, "Midterm Mistakes Log",                     "Mathematics",          "The questions I got wrong on my last midterm.",                                                   9, "maria_santos",   false),
        new(15, "My Calculus Cheat Notes",                  "Mathematics",          "Short notes for my calculus quizzes.",                                                           12, "kuya_ben",       false),
        new(16, "Interview Practice Drafts",                "Programming",          "Draft answers for my coding interview practice.",                                                11, "jmendoza",       false),
    };

    // These are the first 3 flashcards of each PUBLIC reviewer, used for the "Preview" on the Reviewer Details page.
    // The number in the square brackets is the reviewer's Id.
    // The private reviewers have no preview, because their pages are never shown to other people.
    private static readonly Dictionary<int, SampleCard[]> Previews = new()
    {
        [1] = new SampleCard[]
        {
            new("Factor x² + 5x + 6.", "(x + 2)(x + 3)"),
            new("Factor x² − 9.", "(x + 3)(x − 3), because it is a difference of squares."),
            new("Factor 6x + 12.", "6(x + 2), by taking out the common factor 6."),
        },
        [2] = new SampleCard[]
        {
            new("What is sin²θ + cos²θ?", "It is always equal to 1."),
            new("What point on the unit circle belongs to 90°?", "(0, 1)"),
            new("What is tan θ in terms of sin θ and cos θ?", "tan θ = sin θ ÷ cos θ"),
        },
        [3] = new SampleCard[]
        {
            new("What is the derivative of x²?", "2x"),
            new("What is the limit of sin(x) ÷ x as x gets close to 0?", "1"),
            new("What is the derivative of a constant, like 7?", "0"),
        },
        [4] = new SampleCard[]
        {
            new("What does Newton's second law say?", "F = ma. Force equals mass times acceleration."),
            new("What is the unit of force?", "The newton (N)."),
            new("How do you find acceleration?", "Divide the change in velocity by the change in time."),
        },
        [5] = new SampleCard[]
        {
            new("How does atomic size change from left to right across a period?", "It decreases."),
            new("Which element is the most electronegative?", "Fluorine."),
            new("How does ionization energy change as you go down a group?", "It decreases."),
        },
        [6] = new SampleCard[]
        {
            new("Which cell part holds the DNA and controls the cell?", "The nucleus."),
            new("Which cell part makes most of the cell's energy (ATP)?", "The mitochondria."),
            new("Which cell part does photosynthesis in a plant cell?", "The chloroplast."),
        },
        [7] = new SampleCard[]
        {
            new("What does an AND gate output when both inputs are 1?", "1"),
            new("What does a NOT gate do?", "It flips the input: 0 becomes 1, and 1 becomes 0."),
            new("When does an XOR gate output 1?", "When the two inputs are different."),
        },
        [8] = new SampleCard[]
        {
            new("What is the binary number 1010 in decimal?", "10"),
            new("What is the decimal number 15 in hexadecimal?", "F"),
            new("Which digits does the octal system use?", "0 to 7."),
        },
        [9] = new SampleCard[]
        {
            new("Which C# keyword makes a variable for whole numbers?", "int"),
            new("Which loop repeats while a condition is true?", "The while loop."),
            new("What does // do in C#?", "It starts a comment on that line."),
        },
        [10] = new SampleCard[]
        {
            new("Which Python collection is ordered and can be changed?", "A list."),
            new("Which Python collection is ordered but cannot be changed?", "A tuple."),
            new("Which Python collection stores key and value pairs?", "A dictionary."),
        },
        [11] = new SampleCard[]
        {
            new("When did the Philippines declare independence from Spain?", "June 12, 1898."),
            new("Who is known as the founder of the Katipunan?", "Andrés Bonifacio."),
            new("When did the EDSA People Power Revolution happen?", "February 1986."),
        },
        [12] = new SampleCard[]
        {
            new("Which philosopher said, \"I think, therefore I am\"?", "René Descartes."),
            new("Who said that the unexamined life is not worth living?", "Socrates."),
            new("Who proposed the ideas of the id, ego, and superego?", "Sigmund Freud."),
        },
    };

    // THE PRIVACY RULE for the Reviewer Details page.
    // This finds a reviewer by its number, but ONLY if it is PUBLIC.
    // If the reviewer is private, or the number does not exist, it gives back "null" (nothing).
    // The page then shows "not found" in both cases, so nobody can tell the difference.
    public static SampleReviewer? FindPublic(int id) =>
        Reviewers.FirstOrDefault(reviewer => reviewer.Id == id && reviewer.IsPublic);

    // This gives back the first flashcards of a reviewer for the preview.
    // If there are none, it gives back an empty list.
    public static IReadOnlyList<SampleCard> PreviewFor(int id) =>
        Previews.TryGetValue(id, out var cards) ? cards : Array.Empty<SampleCard>();

    // This is for the color of the category tag (blue, red, or yellow).
    // The names match the .tone-blue, .tone-red, and .tone-yellow styles in site.css.
    public static string ToneOf(string category) => category switch
    {
        "Mathematics" => "blue",
        "Science" => "red",
        "Computer Engineering" => "yellow",
        "Programming" => "blue",
        _ => "red" // General Education
    };
}