// Lets us use the Razor Pages base class called PageModel
using Microsoft.AspNetCore.Mvc.RazorPages;

// CHANGE THIS to match the @namespace line in Pages/_ViewImports.cshtml.
// Example: if that file says "@namespace MyApp.Pages", write "namespace MyApp.Pages;"
namespace Rebyulusyon.Pages;

// This class is the C# "brain" of Index.cshtml. ASP.NET pairs them by name.
// ": PageModel" means it inherits all the page features from Microsoft's base class.
public class IndexModel : PageModel
{
    // A "record" is a short way to make a small data holder with 4 pieces of text.
    // Each feature card has: an icon name, a color, a title, and a description.
    public record Feature(string Icon, string Tone, string Title, string Description);

    // The list of six cards shown in the features section.
    // TEMPORARY MOCK CONTENT: typed by hand here. It is not from a database.
    // "{ get; }" means other files can READ this list but not replace it.
    public List<Feature> Features { get; } = new()
    {
        // Each line creates one Feature: (icon name, color, title, description).
        // Icon names must exist in _Icon.cshtml. Colors are: blue, red, yellow.
        new("book",   "blue",   "Create Your Own Reviewers", "Turn your notes into a reviewer in minutes. Write the question in front, the answer at the back — that is it."),
        new("folder", "red",    "Organize by Subject",       "Subjects are yours to name. Math, Programming, Comp Eng, Board Exam — however you actually study."),
        new("layers", "yellow", "Study With Flashcards",     "Flip, recall, and mark what you remember. Active recall on a clean, distraction-free screen."),
        new("chart",  "blue",   "Track Your Progress",       "See cards reviewed, subject mastery and weekly activity without digging through analytics."),
        new("flame",  "red",    "Build Study Streaks",       "A quiet nudge to review daily. Small sessions, kept consistently, win the exam."),
        new("target", "yellow", "Set a Daily Goal",          "Decide how many cards a day is realistic, then keep that promise to yourself."),
    };


    // A "record" for one step: a number, a short title, and a description.
    public record Step(int Number, string Title, string Description);

    // The three steps shown in the "review routine" section.
    // TEMPORARY MOCK CONTENT: typed by hand here, not from a database.
    public List<Step> Steps { get; } = new()
    {
        // Each line creates one Step: (number, title, description)
        new(1, "Create a subject", "Name it the way your syllabus does."),
        new(2, "Add a reviewer",   "Type your flashcards or paste from your notes."),
        new(3, "Study and repeat", "Mark what you remember, revisit what needs practice."),
    };


    // The four checklist items shown in the "AI Study Tools" card.
    // TEMPORARY MOCK CONTENT: typed by hand here. These features are not built yet.
    // "string[]" is an array: a fixed list of text values.
    public string[] AiTools { get; } =
    {
        "Generate flashcards from your notes",
        "Upload PDF or lecture handouts",
        "AI-explained answers",
        "Personalized review recommendations",
    };

    // OnGet runs when someone opens the Home Page.
    // It is empty for now. Later it will load data from the database.
    public void OnGet()
    {
    }
}