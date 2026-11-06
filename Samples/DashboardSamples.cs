// This line lets us write date names like "Mon" in English, whatever language the computer uses.
using System.Globalization;

// This is the "address" of this code inside your project. It is the same as ReviewerSamples.cs.
namespace Rebyulusyon.Samples;

// This describes ONE day on the chart.
// Label = the short day name (like "Mon"), or the word "Today" for the last day
// Cards = how many flashcards were studied that day
// IsToday = true for the last day
public record StudyDay(string Label, int Cards, bool IsToday);

// This describes ONE reviewer that the sample student is studying.
// Reviewer = the reviewer itself (taken from ReviewerSamples, so the title and the link are always correct)
// Mastered = how many of its cards the student marked "I got it"
// DaysAgo = how many days ago the student last studied it (0 = today). null means "never studied".
public record StudiedReviewer(SampleReviewer Reviewer, int Mastered, int? DaysAgo)
{
    // True when every card of the reviewer is mastered.
    public bool IsComplete => Mastered >= Reviewer.CardCount;

    // The progress as a whole number from 0 to 100, for the progress bar.
    public int Percent => Reviewer.CardCount <= 0
        ? 0
        : (int)Math.Round(Mastered * 100.0 / Reviewer.CardCount, MidpointRounding.AwayFromZero);

    // The words that say when the reviewer was last studied.
    public string LastStudiedText => DaysAgo switch
    {
        null => "Not started yet",
        0 => "Studied today",
        1 => "Studied yesterday",
        _ => $"Studied {DaysAgo} days ago"
    };
}

// This is for ALL the sample numbers of the Dashboard, in one place.
// NOTE: everything in this file is TEMPORARY. It does NOT come from a database.
// It is a PRETEND student, so the Dashboard has something to show before login exists.
// Later, the database will replace this file with the real progress of the person who is logged in.
public static class DashboardSamples
{
    // The username of the pretend student.
    public const string Username = "ranfuro";

    // The daily goal at the start (in cards), and the smallest, the biggest, and the step of the goal buttons.
    public const int DefaultGoal = 20;
    public const int MinGoal = 5;
    public const int MaxGoal = 100;
    public const int GoalStep = 5;

    // How many flashcards the student studied on each of the last 14 days. The OLDEST day is first, TODAY is last.
    // The first 7 numbers are the "previous 7 days". The last 7 numbers are the "last 7 days".
    // Every other number on the Dashboard (today, the streak, the totals) is worked out from this one list,
    // so the numbers on the page always add up.
    private static readonly int[] DailyCards = { 10, 15, 18, 12, 9, 14, 0, 8, 14, 20, 16, 11, 18, 12 };

    // This gives back the 7 days for the chart.
    // previousWeek = false gives the last 7 days (ending today). true gives the 7 days before that.
    public static IReadOnlyList<StudyDay> Days(bool previousWeek)
    {
        var list = new List<StudyDay>();
        var firstIndex = previousWeek ? 0 : 7;
        var todayIndex = DailyCards.Length - 1;

        for (var i = firstIndex; i < firstIndex + 7; i++)
        {
            // The real date of this day. The last number in the list is today.
            var date = DateTime.Today.AddDays(i - todayIndex);
            var isToday = i == todayIndex;
            var label = isToday ? "Today" : date.ToString("ddd", CultureInfo.InvariantCulture);
            list.Add(new StudyDay(label, DailyCards[i], isToday));
        }

        return list;
    }

    // How many cards were studied today (the last number in the list).
    public static int CardsToday => DailyCards[^1];

    // The total of one group of 7 days.
    public static int Total(bool previousWeek) => Days(previousWeek).Sum(day => day.Cards);

    // The study streak: how many days in a row (ending today) have at least 1 card.
    public static int Streak
    {
        get
        {
            var streak = 0;
            for (var i = DailyCards.Length - 1; i >= 0 && DailyCards[i] > 0; i--)
            {
                streak++;
            }
            return streak;
        }
    }

    // The reviewers the pretend student is studying: the slug (the name of the reviewer in the web address),
    // how many cards are mastered, and when it was last studied.
    // Only public reviewers can be used here, because a reviewer is looked up by FindPublicBySlug.
    private static readonly (string Slug, int Mastered, int? DaysAgo)[] Studying =
    {
        ("algebra-basics-factoring", 3, 0),
        ("physics-motion-forces", 1, 1),
        ("csharp-basics-variables-loops", 5, 3),
        ("philippine-history-key-events", 0, null),
    };

    // This gives back the reviewers the student is studying.
    // If a slug is not found in ReviewerSamples, that line is skipped, so the Dashboard never crashes.
    public static IReadOnlyList<StudiedReviewer> StudiedReviewers()
    {
        var list = new List<StudiedReviewer>();

        foreach (var (slug, mastered, daysAgo) in Studying)
        {
            var reviewer = ReviewerSamples.FindPublicBySlug(slug);
            if (reviewer is null)
            {
                continue;
            }

            // The mastered number can never be bigger than the number of cards.
            list.Add(new StudiedReviewer(reviewer, Math.Min(mastered, reviewer.CardCount), daysAgo));
        }

        return list;
    }
}