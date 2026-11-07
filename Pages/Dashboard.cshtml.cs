// This line lets us use [Authorize], which keeps people who are not logged in out of this page.
using Microsoft.AspNetCore.Authorization;

// This line lets us use [BindProperty], which fills a property from the web address.
using Microsoft.AspNetCore.Mvc;

// This line lets us use "PageModel", the starting point for a Razor Page's C# code.
using Microsoft.AspNetCore.Mvc.RazorPages;

// This line lets us use the sample numbers (Samples/DashboardSamples.cs and Samples/ReviewerSamples.cs).
using Rebyulusyon.Samples;

// This is the "address" of this code inside your project. It is the same as your other pages.
namespace Rebyulusyon.Pages;

// This is for the C# side of the DASHBOARD page (Pages/Dashboard.cshtml).
// It gets the numbers for the page and works out the totals.
// NOTE: the numbers are still SAMPLE numbers of a pretend student. Nothing comes from a database yet.
// [Authorize] means: only people who are logged in can open this page.
// Everyone else is sent to the Login page.
// (While you are building, you can put // in front of [Authorize] to look at the page. Remove the // afterwards!)
// IMPORTANT: The class in this file must be called DashboardModel, because Dashboard.cshtml uses that name.
//[Authorize]
public class DashboardModel : PageModel
{
    // This is for the chart switch. The web address /Dashboard?week=last shows the days before the last 7 days.
    // [BindProperty(SupportsGet = true)] means: fill this from the web address.
    [BindProperty(SupportsGet = true)]
    public string? Week { get; set; }

    // True when the chart shows the 7 days BEFORE the last 7 days.
    public bool ShowPrevious { get; private set; }

    // ===== THE NUMBERS FOR THE PAGE (all of them are worked out in OnGet below) =====

    public string Username => DashboardSamples.Username;

    // The daily goal at the start, and the limits of the goal buttons. They are written into the page for the JavaScript.
    public int Goal => DashboardSamples.DefaultGoal;
    public int MinGoal => DashboardSamples.MinGoal;
    public int MaxGoal => DashboardSamples.MaxGoal;
    public int GoalStep => DashboardSamples.GoalStep;

    public int CardsToday { get; private set; }
    public int Streak { get; private set; }
    public int Last7 { get; private set; }
    public int Previous7 { get; private set; }

    // The 7 days shown on the chart.
    public IReadOnlyList<StudyDay> Days { get; private set; } = Array.Empty<StudyDay>();

    // The reviewers that are not finished yet (shown in "Continue studying"), and the finished ones.
    public List<StudiedReviewer> InProgress { get; private set; } = new();
    public List<StudiedReviewer> Completed { get; private set; } = new();

    // The totals of all the reviewers the student studies.
    public int TotalReviewers { get; private set; }
    public int TotalCards { get; private set; }
    public int MasteredCards { get; private set; }

    // The reviewer to continue with (the one studied most recently that is not finished). It can be nothing (null).
    public StudiedReviewer? Next => InProgress.FirstOrDefault();

    // "OnGet" runs every time someone OPENS this page.
    public void OnGet()
    {
        // The chart: only the exact word "last" shows the earlier days. Anything else shows the last 7 days.
        ShowPrevious = string.Equals(Week, "last", StringComparison.OrdinalIgnoreCase);
        Days = DashboardSamples.Days(ShowPrevious);

        // The numbers for the cards at the top.
        CardsToday = DashboardSamples.CardsToday;
        Streak = DashboardSamples.Streak;
        Last7 = DashboardSamples.Total(previousWeek: false);
        Previous7 = DashboardSamples.Total(previousWeek: true);

        // The reviewers: the unfinished ones come first, the most recently studied on top.
        // A reviewer that was never studied (DaysAgo is null) goes last.
        var all = DashboardSamples.StudiedReviewers();
        InProgress = all.Where(r => !r.IsComplete)
                        .OrderBy(r => r.DaysAgo ?? int.MaxValue)
                        .ToList();
        Completed = all.Where(r => r.IsComplete).ToList();

        TotalReviewers = all.Count;
        TotalCards = all.Sum(r => r.Reviewer.CardCount);
        MasteredCards = all.Sum(r => r.Mastered);
    }

    // ===== SMALL HELPERS USED BY THE PAGE =====

    // A whole number from 0 to 100 (for example 12 of 20 is 60). It never goes over 100.
    public static int Percent(int value, int total) =>
        total <= 0 ? 0 : Math.Min(100, (int)Math.Round(value * 100.0 / total, MidpointRounding.AwayFromZero));

    // Adds an "s" when the number is not 1. Example: Plural(1, "day") is "day", and Plural(7, "day") is "days".
    public static string Plural(int number, string singular) => number == 1 ? singular : singular + "s";

    // How full the goal bar is (today compared to the goal).
    public int GoalPercent => Percent(CardsToday, Goal);

    // The small sentence under the goal bar.
    // The JavaScript writes the same sentences when the goal changes, so keep them the same.
    public string GoalNote
    {
        get
        {
            var remaining = Goal - CardsToday;
            if (remaining <= 0) return "Goal reached. Great work!";
            return remaining == 1 ? "1 more card to reach your goal." : $"{remaining} more cards to reach your goal.";
        }
    }

    // The sentence under the "Studied today" streak number.
    public string StreakNote =>
        Streak == 0 ? "Study a card today to start a streak."
        : CardsToday > 0 ? "You studied today. Keep it going!"
        : "Study today to keep your streak.";

    // The sentence under the "Last 7 days" number: more, fewer, or the same as the 7 days before.
    public string ComparisonNote
    {
        get
        {
            var difference = Last7 - Previous7;
            if (difference > 0) return $"{difference} more than the 7 days before.";
            if (difference < 0) return $"{-difference} fewer than the 7 days before.";
            return "The same as the 7 days before.";
        }
    }

    // ===== THE CHART =====

    // This is the number that the tallest possible bar stands for.
    // It is the bigger of the goal and the biggest day, plus 15% of free space at the top for the numbers.
    // The JavaScript uses the same rule when the goal changes.
    private double ChartScale => Math.Max(Goal, Days.Count == 0 ? 0 : Days.Max(d => d.Cards)) * 1.15;

    // The height of a bar, from 0 to 100 (percent of the chart). A day with cards always has at least a small bar.
    public int BarHeight(int cards) =>
        cards <= 0 ? 0 : Math.Max(2, (int)Math.Round(cards / ChartScale * 100, MidpointRounding.AwayFromZero));

    // The height where the dashed goal line is drawn.
    public int GoalLinePercent => (int)Math.Round(Goal / ChartScale * 100, MidpointRounding.AwayFromZero);

    // The sentence above the chart.
    public string ChartSummary => ShowPrevious
        ? $"{Previous7} {Plural(Previous7, "card")} in the previous 7 days."
        : $"{Last7} {Plural(Last7, "card")} in the last 7 days. {ComparisonNote}";

    // This is for the color of the category tag on a reviewer (blue, red, or yellow).
    // The colors are chosen in ReviewerSamples, so every page uses the same colors.
    public string ToneOf(string category) => ReviewerSamples.ToneOf(category);
}