// This is the "address" of this code inside your project.
// The folder is called Samples, so the address ends with ".Samples".
namespace Rebyulusyon.Samples;

// This describes ONE sample reviewer. A "record" is a quick way to make a small box of information.
// Id = a number for each reviewer (the flashcards are saved under this number)
// Slug = the short name used in the web address, like algebra-basics-factoring
//        NEW: Use only small letters, numbers, and hyphens (-). Every slug must be DIFFERENT.
// Title, Category, Description = the words about the reviewer
// CardCount = how many flashcards it has
// Creator = the username of the person who made it (shown as "Created by @username")
// IsPublic = true means everyone can see it. false means it is PRIVATE.
public record SampleReviewer(int Id, string Slug, string Title, string Category, string Description, int CardCount, string Creator, bool IsPublic);

// This describes ONE flashcard: the question on the front and the answer on the back.
public record SampleCard(string Question, string Answer);

// This is for ALL the sample reviewers and their flashcards, in one place.
// The Explore Reviewers page, the Reviewer Details page, and the Flashcard Study page all read from here.
// If you want to change a sample reviewer, change it HERE, and every page will use the new version.
// NOTE: everything in this file is TEMPORARY. It does NOT come from a database.
// Later, the database will replace this file.
public static class ReviewerSamples
{
    // This is the list of all sample reviewers.
    // The first 12 are PUBLIC (the last word on each line is true).
    // The last 4 are PRIVATE (the last word is false). They are here to show that private reviewers are never shown to other people.
    // CHANGED: Every reviewer now has a slug (the second thing on each line).
    public static readonly IReadOnlyList<SampleReviewer> Reviewers = new List<SampleReviewer>
    {
        // --- PUBLIC reviewers (everyone can see these) ---
        new(1,  "algebra-basics-factoring",            "Algebra Basics: Factoring Practice",       "Mathematics",          "Practice factoring trinomials, perfect squares, and common factors with quick questions.",      5, "ranfuro",        true),
        new(2,  "trigonometry-identities-unit-circle", "Trigonometry: Identities and the Unit Circle", "Mathematics",      "Review the key identities, angle formulas, and unit circle values one card at a time.",         5, "maria_santos",   true),
        new(3,  "calculus-limits-derivatives",         "Calculus: Limits and Derivatives",         "Mathematics",          "Learn the limit laws and the basic derivative rules, then test yourself with examples.",         5, "kuya_ben",       true),
        new(4,  "physics-motion-forces",               "Physics: Motion and Forces",               "Science",              "Go over velocity, acceleration, and Newton's laws with short definition and problem cards.",     5, "ate_lyn",        true),
        new(5,  "chemistry-periodic-table-trends",     "Chemistry: Periodic Table Trends",         "Science",              "Compare atomic size, electronegativity, and ionization energy across groups and periods.",       5, "claire_studies", true),
        new(6,  "biology-cell-parts-functions",        "Biology: Cell Parts and Functions",        "Science",              "Match each cell part to its job, from the nucleus to the mitochondria.",                         5, "ranfuro",        true),
        new(7,  "digital-logic-gates-truth-tables",    "Digital Logic: Gates and Truth Tables",    "Computer Engineering", "Learn the AND, OR, NOT, NAND, NOR, and XOR gates and how to read their truth tables.",           5, "dev.paolo",      true),
        new(8,  "number-systems-conversions",          "Number Systems and Conversions",           "Computer Engineering", "Convert between binary, octal, decimal, and hexadecimal with worked examples.",                 5, "nikka_r",        true),
        new(9,  "csharp-basics-variables-loops",       "C# Basics: Variables and Loops",           "Programming",          "Review data types, variables, if statements, and for and while loops in C#.",                    5, "jmendoza",       true),
        new(10, "python-lists-tuples-dictionaries",    "Python Lists, Tuples, and Dictionaries",   "Programming",          "Know when to use each collection type and the common methods that go with them.",               5, "dev.paolo",      true),
        new(11, "philippine-history-key-events",       "Philippine History: Key Events Timeline",  "General Education",    "Remember the major events from the pre-colonial period to modern times, in order.",             5, "maria_santos",   true),
        new(12, "understanding-the-self-key-terms",    "Understanding the Self: Key Terms",        "General Education",    "Review the main ideas and thinkers from your Understanding the Self course.",                   5, "ate_lyn",        true),

        // --- PRIVATE reviewers (only their creators should ever see these) ---
        new(13, "board-exam-weak-topics",              "Board Exam Weak Topics",                   "Science",              "My own list of topics that I keep forgetting.",                                                  14, "ranfuro",        false),
        new(14, "midterm-mistakes-log",                "Midterm Mistakes Log",                     "Mathematics",          "The questions I got wrong on my last midterm.",                                                   9, "maria_santos",   false),
        new(15, "my-calculus-cheat-notes",             "My Calculus Cheat Notes",                  "Mathematics",          "Short notes for my calculus quizzes.",                                                           12, "kuya_ben",       false),
        new(16, "interview-practice-drafts",           "Interview Practice Drafts",                "Programming",          "Draft answers for my coding interview practice.",                                                11, "jmendoza",       false),
    };

    // These are the flashcards of each PUBLIC reviewer. Each one has 5 cards.
    // The number in the square brackets is the reviewer's Id (not the slug).
    // The private reviewers have no cards here, because their pages are never shown to other people.
    // The Reviewer Details page shows the first 3 of these as the "Preview".
    // The Flashcard Study page uses all 5.
    private static readonly Dictionary<int, SampleCard[]> Cards = new()
    {
        [1] = new SampleCard[]
        {
            new("Factor x² + 5x + 6.", "(x + 2)(x + 3)"),
            new("Factor x² − 9.", "(x + 3)(x − 3), because it is a difference of squares."),
            new("Factor 6x + 12.", "6(x + 2), by taking out the common factor 6."),
            new("Factor x² + 6x + 9.", "(x + 3)², because it is a perfect square trinomial."),
            new("Factor 2x² + 8x.", "2x(x + 4), by taking out the common factor 2x."),
        },
        [2] = new SampleCard[]
        {
            new("What is sin²θ + cos²θ?", "It is always equal to 1."),
            new("What point on the unit circle belongs to 90°?", "(0, 1)"),
            new("What is tan θ in terms of sin θ and cos θ?", "tan θ = sin θ ÷ cos θ"),
            new("What is the value of cos 60°?", "1/2"),
            new("How many degrees is π radians?", "180°"),
        },
        [3] = new SampleCard[]
        {
            new("What is the derivative of x²?", "2x"),
            new("What is the limit of sin(x) ÷ x as x gets close to 0?", "1"),
            new("What is the derivative of a constant, like 7?", "0"),
            new("What does the power rule say about the derivative of xⁿ?", "It is n times x to the power of n − 1."),
            new("What is the derivative of 3x³?", "9x²"),
        },
        [4] = new SampleCard[]
        {
            new("What does Newton's second law say?", "F = ma. Force equals mass times acceleration."),
            new("What is the unit of force?", "The newton (N)."),
            new("How do you find acceleration?", "Divide the change in velocity by the change in time."),
            new("What does Newton's first law say?", "An object stays at rest, or keeps moving at a constant velocity, unless a net force acts on it."),
            new("What does Newton's third law say?", "For every action, there is an equal and opposite reaction."),
        },
        [5] = new SampleCard[]
        {
            new("How does atomic size change from left to right across a period?", "It decreases."),
            new("Which element is the most electronegative?", "Fluorine."),
            new("How does ionization energy change as you go down a group?", "It decreases."),
            new("What are the elements in Group 18 called?", "The noble gases."),
            new("Which group of the periodic table holds the alkali metals?", "Group 1."),
        },
        [6] = new SampleCard[]
        {
            new("Which cell part holds the DNA and controls the cell?", "The nucleus."),
            new("Which cell part makes most of the cell's energy (ATP)?", "The mitochondria."),
            new("Which cell part does photosynthesis in a plant cell?", "The chloroplast."),
            new("Which cell part controls what enters and leaves the cell?", "The cell membrane."),
            new("Which cell parts make proteins?", "The ribosomes."),
        },
        [7] = new SampleCard[]
        {
            new("What does an AND gate output when both inputs are 1?", "1"),
            new("What does a NOT gate do?", "It flips the input: 0 becomes 1, and 1 becomes 0."),
            new("When does an XOR gate output 1?", "When the two inputs are different."),
            new("What does a NAND gate output when both inputs are 1?", "0"),
            new("When does an OR gate output 0?", "Only when both inputs are 0."),
        },
        [8] = new SampleCard[]
        {
            new("What is the binary number 1010 in decimal?", "10"),
            new("What is the decimal number 15 in hexadecimal?", "F"),
            new("Which digits does the octal system use?", "0 to 7."),
            new("What is the decimal number 8 in binary?", "1000"),
            new("Which symbols does the hexadecimal system use?", "The digits 0 to 9 and the letters A to F."),
        },
        [9] = new SampleCard[]
        {
            new("Which C# keyword makes a variable for whole numbers?", "int"),
            new("Which loop repeats while a condition is true?", "The while loop."),
            new("What does // do in C#?", "It starts a comment on that line."),
            new("Which C# keyword makes a variable for text?", "string"),
            new("Which statement runs code only if a condition is true?", "The if statement."),
        },
        [10] = new SampleCard[]
        {
            new("Which Python collection is ordered and can be changed?", "A list."),
            new("Which Python collection is ordered but cannot be changed?", "A tuple."),
            new("Which Python collection stores key and value pairs?", "A dictionary."),
            new("Which list method adds an item to the end of a list?", "append()"),
            new("How do you get a value from a dictionary?", "Use square brackets with the key, like ages['ana']."),
        },
        [11] = new SampleCard[]
        {
            new("When did the Philippines declare independence from Spain?", "June 12, 1898."),
            new("Who is known as the founder of the Katipunan?", "Andrés Bonifacio."),
            new("When did the EDSA People Power Revolution happen?", "February 1986."),
            new("Who wrote the novel Noli Me Tángere?", "José Rizal."),
            new("In which year did Magellan arrive in the Philippines?", "1521."),
        },
        [12] = new SampleCard[]
        {
            new("Which philosopher said, \"I think, therefore I am\"?", "René Descartes."),
            new("Who said that the unexamined life is not worth living?", "Socrates."),
            new("Who proposed the ideas of the id, ego, and superego?", "Sigmund Freud."),
            new("Which philosopher described the mind as a blank slate (tabula rasa)?", "John Locke."),
            new("In Freud's model, which part follows the pleasure principle?", "The id."),
        },
    };

    // THE PRIVACY RULE.
    // CHANGED: This now finds a reviewer by its SLUG (the name in the web address), and ONLY if it is PUBLIC.
    // Capital and small letters do not matter, so /Algebra-Basics-Factoring works too.
    // If the reviewer is private, or the slug does not exist, or nothing was given, it gives back "null" (nothing).
    // The pages then show "not found" in all of these cases, so nobody can tell the difference.
    public static SampleReviewer? FindPublicBySlug(string? slug) =>
        string.IsNullOrWhiteSpace(slug)
            ? null
            : Reviewers.FirstOrDefault(reviewer =>
                reviewer.IsPublic && string.Equals(reviewer.Slug, slug, StringComparison.OrdinalIgnoreCase));

    // This gives back ALL the flashcards of a reviewer, for the Flashcard Study page.
    // It checks the privacy rule AGAIN here, so the cards of a PRIVATE reviewer are never given out,
    // even if some page forgets to check. If there are no cards, it gives back an empty list.
    // CHANGED: It now takes the reviewer itself (it took a number before).
    public static IReadOnlyList<SampleCard> CardsFor(SampleReviewer reviewer) =>
        reviewer.IsPublic && Cards.TryGetValue(reviewer.Id, out var cards) ? cards : Array.Empty<SampleCard>();

    // This gives back only the first 3 flashcards, for the "Preview" on the Reviewer Details page.
    // It uses the same cards as CardsFor, so the preview and the study page always agree.
    public static IReadOnlyList<SampleCard> PreviewFor(SampleReviewer reviewer) =>
        CardsFor(reviewer).Take(3).ToList();

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