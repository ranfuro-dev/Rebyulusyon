// This line lets us use "PageModel", the starting point for a Razor Page's C# code.
using Microsoft.AspNetCore.Mvc.RazorPages;

// This is the "address" of this code inside your project.
// It must match the project name. "Rebyulusyon.Pages" is correct for your project, so do not change it.
namespace Rebyulusyon.Pages;

// This is for the C# side of the HOME PAGE (Pages/Home.cshtml).
// ASP.NET joins the two files because their names match (Home.cshtml and Home.cshtml.cs).
// The Home page reads the lists in this file and shows them as cards, steps, and a checklist.
// CHANGED: This class was called "IndexModel" before. It is now "HomeModel", to match the file name.
// ": PageModel" means this class gets all the normal page features from Microsoft.
public class HomeModel : PageModel
{
    // This describes ONE feature card. A "record" is a quick way to make a small box of information.
    // Every card has 4 things: the picture name (Icon), the circle color (Tone), the title, and the text.
    public record Feature(string Icon, string Tone, string Title, string Description);

    // This is for the six cards in the "Everything in one place" part of the Home page.
    // NOTE: these cards are typed by hand here. They do not come from a database.
    // "{ get; }" means the Home page can read this list but cannot replace it.
    public List<Feature> Features { get; } = new()
    {
        // Each line below is one card, in this order: (picture name, circle color, title, text).
        // The picture name must be one of the pictures in _Icon.cshtml.
        // The color can be "blue", "red", or "yellow".
        // To add a new card, copy one line and change the words. It will show up on the Home page by itself.
        new("book",   "blue",   "Create Your Own Reviewers", "Turn your notes into a reviewer in minutes. Write the question in front, the answer at the back — that is it."),
        new("folder", "red",    "Organize by Subject",       "Subjects are yours to name. Math, Programming, Comp Eng, Board Exam — however you actually study."),
        new("layers", "yellow", "Study With Flashcards",     "Flip, recall, and mark what you remember. Active recall on a clean, distraction-free screen."),
        new("chart",  "blue",   "Track Your Progress",       "See cards reviewed, subject mastery and weekly activity without digging through analytics."),
        new("flame",  "red",    "Build Study Streaks",       "A quiet nudge to review daily. Small sessions, kept consistently, win the exam."),
        new("target", "yellow", "Set a Daily Goal",          "Decide how many cards a day is realistic, then keep that promise to yourself."),
    };


    // This describes ONE step. Every step has a number, a short title, and a text under it.
    public record Step(int Number, string Title, string Description);

    // This is for the three steps in the pink "From scattered notes to a review routine" part of the Home page.
    // NOTE: these steps are typed by hand here. They do not come from a database.
    public List<Step> Steps { get; } = new()
    {
        // Each line below is one step, in this order: (number, title, text).
        new(1, "Create a subject", "Name it the way your syllabus does."),
        new(2, "Add a reviewer",   "Type your flashcards or paste from your notes."),
        new(3, "Study and repeat", "Mark what you remember, revisit what needs practice."),
    };


    // This is for the four checked lines in the yellow box of the "AI Study Tools" card on the Home page.
    // NOTE: these are typed by hand here. These tools are not built yet.
    // "string[]" means a simple list of words that does not change while the website is running.
    public string[] AiTools { get; } =
    {
        "Generate flashcards from your notes",
        "Upload PDF or lecture handouts",
        "AI-explained answers",
        "Personalized review recommendations",
    };

    // "OnGet" runs every time someone opens the Home page.
    // It is empty for now, because the lists above are already filled in.
    // Later, this is where the page can ask the database for real information (for example, the user's real progress).
    public void OnGet()
    {
    }
}