using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Rebyulusyon.Data;

namespace Rebyulusyon.Pages;

[Authorize]
public class DashboardModel : PageModel
{
    private readonly ApplicationDbContext _db;
    private readonly UserManager<ApplicationUser> _userManager;

    public DashboardModel(ApplicationDbContext db, UserManager<ApplicationUser> userManager)
    {
        _db = db;
        _userManager = userManager;
    }

    [BindProperty(SupportsGet = true)]
    public string? Week { get; set; }

    public bool ShowPrevious { get; private set; }
    public string Username => User.Identity?.Name ?? "Student";
    public int Goal { get; private set; } = 20;
    public int WeekStartsOnDay { get; private set; }
    public int MinGoal => 5;
    public int MaxGoal => 100;
    public int GoalStep => 5;
    public int CardsToday { get; private set; }
    public int Streak { get; private set; }
    public int Last7 { get; private set; }
    public int Previous7 { get; private set; }
    public IReadOnlyList<DashboardStudyDay> Days { get; private set; } = Array.Empty<DashboardStudyDay>();
    public List<ReviewerProgress> InProgress { get; private set; } = new();
    public List<ReviewerProgress> Completed { get; private set; } = new();
    public int TotalReviewers { get; private set; }
    public int TotalCards { get; private set; }
    public int MasteredCards { get; private set; }
    public ReviewerProgress? Next => InProgress.FirstOrDefault();

    public async Task OnGetAsync()
    {
        ShowPrevious = string.Equals(Week, "last", StringComparison.OrdinalIgnoreCase);
        var user = await _userManager.GetUserAsync(User)
            ?? throw new InvalidOperationException("The signed-in account could not be found.");
        var userId = user.Id;
        Goal = user.DailyGoal;
        WeekStartsOnDay = user.WeekStartsOnDay;
        var today = DateTime.UtcNow.Date;
        var activities = await _db.StudyActivities
            .Where(item => item.UserId == userId)
            .OrderBy(item => item.StudiedAtUtc)
            .ToListAsync();

        // Count a flashcard once per day, even if the user rates it repeatedly that day.
        var activityDays = activities.GroupBy(item => item.StudiedAtUtc.Date)
            .ToDictionary(group => group.Key, group => group.Select(item => item.FlashcardId).Distinct().Count());
        CardsToday = activityDays.GetValueOrDefault(today);
        Last7 = SumDays(activityDays, today.AddDays(-6), today);
        Previous7 = SumDays(activityDays, today.AddDays(-13), today.AddDays(-7));

        var daysSinceWeekStart = ((int)today.DayOfWeek - WeekStartsOnDay + 7) % 7;
        var currentWeekStart = today.AddDays(-daysSinceWeekStart);
        var chartStart = ShowPrevious ? currentWeekStart.AddDays(-7) : currentWeekStart;
        Days = Enumerable.Range(0, 7)
            .Select(offset => chartStart.AddDays(offset))
            .Select(date => new DashboardStudyDay(date == today ? "Today" : date.ToString("ddd"),
                activityDays.GetValueOrDefault(date), date == today))
            .ToList();

        var studiedDates = activityDays.Keys.ToHashSet();
        var streakDate = studiedDates.Contains(today) ? today : today.AddDays(-1);
        while (studiedDates.Contains(streakDate))
        {
            Streak++;
            streakDate = streakDate.AddDays(-1);
        }

        // Include this user's own reviewers and public reviewers they have opened.
        var openedReviewers = await _db.ReviewerStudies
            .Where(item => item.UserId == userId)
            .ToListAsync();
        var openedReviewerIds = openedReviewers.Select(item => item.ReviewerId).ToHashSet();
        var openedReviewerIdList = openedReviewerIds.ToList();
        var lastOpenedByReviewer = openedReviewers.ToDictionary(item => item.ReviewerId, item => item.LastOpenedAtUtc);

        // Mastery uses the latest saved rating for each card, while the list can also include a reviewer just opened.
        var reviewers = await _db.Reviewers
            .Where(item => item.OwnerId == userId || (item.IsPublic && openedReviewerIdList.Contains(item.Id)))
            .Include(item => item.Subject)
            .Include(item => item.Cards)
            .ToListAsync();
        var latestByCard = activities
            .GroupBy(item => item.FlashcardId)
            .ToDictionary(group => group.Key, group => group.OrderByDescending(item => item.StudiedAtUtc).First());

        var progress = reviewers
            .Where(reviewer => reviewer.Cards.Count > 0 || openedReviewerIds.Contains(reviewer.Id))
            .Select(reviewer =>
            {
                var cardProgress = reviewer.Cards
                    .Where(card => latestByCard.ContainsKey(card.Id))
                    .Select(card => latestByCard[card.Id])
                    .ToList();
                var mastered = cardProgress.Count(item => item.GotIt);
                var lastStudied = cardProgress.Count == 0 ? (DateTime?)null : cardProgress.Max(item => item.StudiedAtUtc);
                var lastOpened = lastOpenedByReviewer.GetValueOrDefault(reviewer.Id);
                return new ReviewerProgress(reviewer, reviewer.Subject.Name, mastered,
                    reviewer.Cards.Count, lastStudied,
                    reviewer.Cards.Count > 0 && mastered == reviewer.Cards.Count,
                    lastOpened == default ? null : lastOpened,
                    reviewer.OwnerId == userId);
            })
            .OrderBy(item => item.LastActivityAtUtc.HasValue ? 0 : 1)
            .ThenByDescending(item => item.LastActivityAtUtc)
            .ToList();

        InProgress = progress.Where(item => !item.IsComplete).ToList();
        Completed = progress.Where(item => item.IsComplete).ToList();
        TotalReviewers = progress.Count;
        TotalCards = progress.Sum(item => item.CardCount);
        MasteredCards = progress.Sum(item => item.Mastered);
    }

    // Persist the dashboard goal as an account preference instead of browser-only storage.
    public async Task<IActionResult> OnPostUpdateGoalAsync(int dailyGoal)
    {
        if (dailyGoal is < 5 or > 100 || dailyGoal % 5 != 0)
        {
            return BadRequest();
        }

        var user = await _userManager.GetUserAsync(User);
        if (user is null)
        {
            return Unauthorized();
        }

        user.DailyGoal = dailyGoal;
        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            return BadRequest();
        }

        return new JsonResult(new { saved = true });
    }

    // Delete only this signed-in user's study history; their reviewers and flashcards remain saved.
    public async Task<IActionResult> OnPostResetProgressAsync()
    {
        var userId = _userManager.GetUserId(User)!;
        var deletedCount = await _db.StudyActivities
            .Where(item => item.UserId == userId)
            .ExecuteDeleteAsync();
        var deletedOpenReviewers = await _db.ReviewerStudies
            .Where(item => item.UserId == userId)
            .ExecuteDeleteAsync();

        TempData["ProgressResetMessage"] = deletedCount + deletedOpenReviewers > 0
            ? "Your study progress has been reset. Your reviewers and flashcards are still saved."
            : "There is no saved study progress to reset yet.";

        return RedirectToPage();
    }

    // Delete this user's reviewer, or remove another user's public reviewer from this dashboard.
    public async Task<IActionResult> OnPostRemoveReviewerAsync(int reviewerId)
    {
        var userId = _userManager.GetUserId(User)!;
        var reviewer = await _db.Reviewers.FirstOrDefaultAsync(item =>
            item.Id == reviewerId && (item.OwnerId == userId || item.IsPublic));

        if (reviewer is null)
        {
            return NotFound();
        }

        if (reviewer.OwnerId == userId)
        {
            // The database cascades this deletion to its flashcards and related study records.
            _db.Reviewers.Remove(reviewer);
            await _db.SaveChangesAsync();
            TempData["ProgressResetMessage"] = "Your reviewer and its flashcards were deleted.";
        }
        else
        {
            // Keep the creator's public reviewer, but remove this user's dashboard and learning history.
            var publicCardIds = await _db.Flashcards
                .Where(card => card.ReviewerId == reviewerId)
                .Select(card => card.Id)
                .ToListAsync();
            if (publicCardIds.Count > 0)
            {
                await _db.StudyActivities
                    .Where(item => item.UserId == userId && publicCardIds.Contains(item.FlashcardId))
                    .ExecuteDeleteAsync();
            }

            var visit = await _db.ReviewerStudies.FirstOrDefaultAsync(item =>
                item.UserId == userId && item.ReviewerId == reviewerId);
            if (visit is not null)
            {
                _db.ReviewerStudies.Remove(visit);
                await _db.SaveChangesAsync();
            }

            TempData["ProgressResetMessage"] = "The public reviewer was removed from your dashboard. The creator's reviewer is unchanged.";
        }

        return RedirectToPage();
    }

    private static int SumDays(Dictionary<DateTime, int> counts, DateTime first, DateTime last)
    {
        var total = 0;
        for (var date = first; date <= last; date = date.AddDays(1))
            total += counts.GetValueOrDefault(date);
        return total;
    }

    public static int Percent(int value, int total) =>
        total <= 0 ? 0 : Math.Min(100, (int)Math.Round(value * 100.0 / total, MidpointRounding.AwayFromZero));

    public static string Plural(int number, string singular) => number == 1 ? singular : singular + "s";
    public int GoalPercent => Percent(CardsToday, Goal);
    public string GoalNote => Goal - CardsToday <= 0 ? "Goal reached. Great work!" :
        Goal - CardsToday == 1 ? "1 more card to reach your goal." : $"{Goal - CardsToday} more cards to reach your goal.";
    public string StreakNote => Streak == 0 ? "Study a card today to start a streak." :
        CardsToday > 0 ? "You studied today. Keep it going!" : "Study today to keep your streak.";
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

    private double ChartScale => Math.Max(Goal, Days.Count == 0 ? 0 : Days.Max(day => day.Cards)) * 1.15;
    public int BarHeight(int cards) => cards <= 0 ? 0 : Math.Max(2, (int)Math.Round(cards / ChartScale * 100, MidpointRounding.AwayFromZero));
    public int GoalLinePercent => (int)Math.Round(Goal / ChartScale * 100, MidpointRounding.AwayFromZero);
    public string ChartSummary
    {
        get
        {
            var total = Days.Sum(day => day.Cards);
            var period = ShowPrevious ? "the previous week" : "this week";
            return $"{total} {Plural(total, "card")} in {period}.";
        }
    }

    public record DashboardStudyDay(string Label, int Cards, bool IsToday);

    public class ReviewerProgress
    {
        public Reviewer Reviewer { get; }
        public string SubjectName { get; }
        public int Mastered { get; }
        public int CardCount { get; }
        public DateTime? LastStudiedAtUtc { get; }
        public DateTime? LastOpenedAtUtc { get; }
        public bool IsComplete { get; }
        public bool IsOwnedByCurrentUser { get; }
        public int Percent => DashboardModel.Percent(Mastered, CardCount);
        public DateTime? LastActivityAtUtc
        {
            get
            {
                if (LastStudiedAtUtc is null) return LastOpenedAtUtc;
                if (LastOpenedAtUtc is null) return LastStudiedAtUtc;
                return LastStudiedAtUtc > LastOpenedAtUtc ? LastStudiedAtUtc : LastOpenedAtUtc;
            }
        }
        public string LastStudiedText => LastOpenedAtUtc > LastStudiedAtUtc
            ? ActivityLabel(LastOpenedAtUtc.Value, "Opened")
            : LastStudiedAtUtc is null
            ? LastOpenedAtUtc is null ? "Not started yet" : ActivityLabel(LastOpenedAtUtc.Value, "Opened")
            : ActivityLabel(LastStudiedAtUtc.Value, "Studied");

        private static string ActivityLabel(DateTime activityAt, string action) =>
            activityAt.Date == DateTime.UtcNow.Date ? $"{action} today" :
            activityAt.Date == DateTime.UtcNow.Date.AddDays(-1) ? $"{action} yesterday" :
            $"{action} {activityAt:MMM d}";

        public ReviewerProgress(Reviewer reviewer, string subjectName, int mastered, int cardCount,
            DateTime? lastStudiedAtUtc, bool isComplete, DateTime? lastOpenedAtUtc,
            bool isOwnedByCurrentUser)
        {
            Reviewer = reviewer;
            SubjectName = subjectName;
            Mastered = mastered;
            CardCount = cardCount;
            LastStudiedAtUtc = lastStudiedAtUtc;
            LastOpenedAtUtc = lastOpenedAtUtc;
            IsComplete = isComplete;
            IsOwnedByCurrentUser = isOwnedByCurrentUser;
        }
    }
}
