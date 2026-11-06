// ==========================================================
// This file is dashboard.js.
// It is used by the Dashboard page only. It makes the daily goal buttons ("-" and "+") work.
// When the goal changes, it updates the goal bar, the sentence under it, the dashed goal line on the chart,
// and the height of the bars, so the chart always makes sense.
// The goal is kept in THIS browser only (in "localStorage"), so it is still there when you come back.
// NOTE: It is not saved on a server or in a database yet. Later, the database will save it for each person.
// ==========================================================

(function () {

    // This finds the Dashboard. If this page is not the Dashboard, we stop here.
    const root = document.getElementById("dashboard");
    if (!root) return;

    // ===== THE NUMBERS THE PAGE GIVES US =====
    // Dashboard.cshtml writes these as "data-" words on the page, so this file never has to guess them.
    const today = Number(root.dataset.today);          // cards studied today
    const minGoal = Number(root.dataset.minGoal);      // the smallest goal
    const maxGoal = Number(root.dataset.maxGoal);      // the biggest goal
    const step = Number(root.dataset.goalStep);        // how much a click on "-" or "+" changes the goal
    let goal = Number(root.dataset.defaultGoal);       // the goal now (it starts as the default)

    // The name of the place in the browser where the goal is kept.
    const STORAGE_KEY = "rebyulusyon.dailyGoal";

    // ===== THE PARTS OF THE PAGE =====
    // These lines find the parts by their "id" in Dashboard.cshtml.
    const goalValue = document.getElementById("goalValue");      // the number between "-" and "+"
    const goalOf = document.getElementById("goalOf");            // the goal number in "12 / 20 cards"
    const goalLineText = document.getElementById("goalLineText"); // the goal number under the chart
    const goalBar = document.getElementById("goalBar");          // the progress bar (for screen readers)
    const goalFill = document.getElementById("goalFill");        // the blue part of the bar
    const goalNote = document.getElementById("goalNote");        // the sentence under the bar
    const goalDown = document.getElementById("goalDown");        // the "-" button
    const goalUp = document.getElementById("goalUp");            // the "+" button
    const goalLine = document.getElementById("goalLine");        // the dashed line on the chart
    const bars = Array.from(document.querySelectorAll("#weekChart [data-value]")); // the bars of the chart

    // If something important is missing, we stop here, so the page still works without the buttons.
    if (!goalValue || !goalOf || !goalFill || !goalNote || !goalDown || !goalUp) return;

    // ===== READ THE SAVED GOAL =====
    // If the browser has a saved goal and it is a good number (a whole number, in range, and a multiple of the step),
    // we use it. If not, we keep the default goal.
    try {
        const saved = Number(window.localStorage.getItem(STORAGE_KEY));
        if (Number.isInteger(saved) && saved >= minGoal && saved <= maxGoal && saved % step === 0) {
            goal = saved;
        }
    } catch (error) {
        // Some browsers do not let a page keep things (for example in private mode). That is fine. We just use the default.
    }

    // ===== SHOW EVERYTHING FOR THE CURRENT GOAL =====
    function render() {
        // The goal number in the three places where it is written.
        goalValue.textContent = goal;
        goalOf.textContent = goal;
        if (goalLineText) goalLineText.textContent = goal;

        // The goal bar: how much of the goal is done today (never more than 100%).
        const percent = Math.min(100, Math.round((today / goal) * 100));
        goalFill.style.width = percent + "%";
        if (goalBar) {
            goalBar.setAttribute("aria-valuemax", String(goal));
            goalBar.setAttribute("aria-valuenow", String(Math.min(today, goal)));
        }

        // The sentence under the bar. These are the same sentences that the C# file writes first.
        const remaining = goal - today;
        if (remaining <= 0) {
            goalNote.textContent = "Goal reached. Great work!";
        } else if (remaining === 1) {
            goalNote.textContent = "1 more card to reach your goal.";
        } else {
            goalNote.textContent = remaining + " more cards to reach your goal.";
        }

        // The chart. The tallest bar stands for the bigger of the goal and the biggest day, plus 15% of free space on top.
        // This is the same rule as in the C# file.
        const values = bars.map((bar) => Number(bar.dataset.value));
        const scale = Math.max(goal, ...values) * 1.15;

        bars.forEach((bar, index) => {
            const value = values[index];
            // A day with 0 cards has no bar. A day with cards always has at least a small bar.
            const height = value <= 0 ? 0 : Math.max(2, Math.round((value / scale) * 100));
            bar.style.height = height + "%";
        });

        // The dashed goal line moves to the new goal.
        if (goalLine) goalLine.style.bottom = Math.round((goal / scale) * 100) + "%";

        // A button that cannot be used anymore (the goal is at its smallest or biggest) is turned off.
        goalDown.disabled = goal <= minGoal;
        goalUp.disabled = goal >= maxGoal;
    }

    // ===== CHANGE THE GOAL =====
    // "amount" is +5 or -5. The new goal always stays between the smallest and the biggest goal.
    function changeGoal(amount) {
        goal = Math.min(maxGoal, Math.max(minGoal, goal + amount));

        // Keep the goal in this browser. If the browser does not allow it, nothing bad happens.
        try {
            window.localStorage.setItem(STORAGE_KEY, String(goal));
        } catch (error) {
            // We ignore this. The goal still works until the page is closed.
        }

        render();
    }

    // The two buttons.
    goalDown.addEventListener("click", () => changeGoal(-step));
    goalUp.addEventListener("click", () => changeGoal(step));

    // ===== START =====
    // Show the page for the saved goal (or the default goal).
    render();
})();