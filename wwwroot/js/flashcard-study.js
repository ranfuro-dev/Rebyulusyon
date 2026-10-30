// ==========================================================
// This file is flashcard-study.js.
// It makes the Flashcard Study page work: it turns the card, moves between cards,
// counts "I got it" and "I missed it", and shows the results at the end.
// The cards come from the page itself (a plain list that the C# file made).
// NOTE: Your results are only kept while the page is open. Nothing is saved yet.
// Later, the database will save them.
// ==========================================================


// This finds the plain list of cards and the card area. Both are written in FlashcardStudy.cshtml.
const deckList = document.getElementById("deck");
const studyApp = document.getElementById("studyApp");

// Only continue if this is the Flashcard Study page.
if (deckList && studyApp) {

    // This reads every card from the plain list. Each card has a question and an answer.
    // "textContent" reads only the words (it never runs any code), so it is safe.
    const cards = Array.from(deckList.querySelectorAll("li")).map((item) => ({
        question: item.querySelector("[data-question]").textContent.trim(),
        answer: item.querySelector("[data-answer]").textContent.trim()
    }));

    // Only continue if there is at least one card.
    if (cards.length > 0) {

        // ===== THE PARTS OF THE PAGE =====
        // These lines find the boxes and buttons by their "id" in FlashcardStudy.cshtml.
        const deckArea = document.getElementById("deckArea");       // the box with the progress, the card, and the buttons
        const flipCard = document.getElementById("flipCard");       // the whole card
        const flipInner = document.getElementById("flipInner");     // the part that turns around
        const faceFront = document.getElementById("faceFront");     // the front side (question)
        const faceBack = document.getElementById("faceBack");       // the back side (answer)
        const frontText = document.getElementById("frontText");
        const backText = document.getElementById("backText");
        const progressText = document.getElementById("progressText");
        const scoreText = document.getElementById("scoreText");
        const progressBar = document.getElementById("progressBar");
        const progressFill = document.getElementById("progressFill");
        const ratingRow = document.getElementById("ratingRow");     // the row with "I missed it" and "I got it"
        const btnMissed = document.getElementById("btnMissed");
        const btnGot = document.getElementById("btnGot");
        const btnPrev = document.getElementById("btnPrev");
        const btnNext = document.getElementById("btnNext");
        const nextLabel = document.getElementById("nextLabel");
        const results = document.getElementById("results");         // the box shown at the end
        const resultTitle = document.getElementById("resultTitle");
        const resultLine = document.getElementById("resultLine");
        const statGot = document.getElementById("statGot");
        const statMissed = document.getElementById("statMissed");
        const statSkipped = document.getElementById("statSkipped");
        const btnRestart = document.getElementById("btnRestart");
        const announce = document.getElementById("announce");       // a hidden line that screen readers read aloud

        // ===== WHAT THE PAGE REMEMBERS =====
        let index = 0;          // which card is shown now (0 is the first card)
        let flipped = false;    // is the card turned to show the answer?
        // For each card: null = not rated yet, true = "I got it", false = "I missed it".
        const ratings = new Array(cards.length).fill(null);

        // This tells screen readers a short message. It does not show anything on the screen.
        function say(message) {
            announce.textContent = message;
        }

        // This turns the card to the answer side (true) or the question side (false).
        function setFlipped(value) {
            flipped = value;
            flipCard.classList.toggle("is-flipped", value);

            // The side that is turned away is hidden from screen readers.
            faceFront.setAttribute("aria-hidden", String(value));
            faceBack.setAttribute("aria-hidden", String(!value));

            // The "I missed it" and "I got it" buttons show up only after the answer is shown.
            ratingRow.classList.toggle("is-hidden", !value);
        }

        // This shows the card number "newIndex". It also updates the bar, the score, and the buttons.
        function showCard(newIndex, shouldSay = true) {
            index = newIndex;
            const card = cards[index];

            // We turn the animation off while we change the words and turn the card back.
            // If we did not, the new answer would show for a moment while the card turns.
            flipInner.classList.add("no-anim");
            setFlipped(false);
            frontText.textContent = card.question;
            backText.textContent = card.answer;
            void flipInner.offsetWidth;                // this forces the browser to apply the change right now
            flipInner.classList.remove("no-anim");

            // The progress line, the score, and the bar.
            const got = ratings.filter((rating) => rating === true).length;
            const missed = ratings.filter((rating) => rating === false).length;
            progressText.textContent = "Card " + (index + 1) + " of " + cards.length;
            scoreText.textContent = got + " right · " + missed + " missed";
            progressFill.style.width = ((index + 1) / cards.length) * 100 + "%";
            progressBar.setAttribute("aria-valuenow", String(index + 1));

            // "Previous" cannot be used on the first card. On the last card, "Next" says "Finish".
            btnPrev.disabled = index === 0;
            nextLabel.textContent = index === cards.length - 1 ? "Finish" : "Next";

            if (shouldSay) {
                say("Card " + (index + 1) + " of " + cards.length + ". Question: " + card.question);
            }
        }

        // This turns the card over. It is used when the card is clicked, or when Enter or Space is pressed.
        function toggleFlip() {
            setFlipped(!flipped);
            say(flipped ? "Answer: " + cards[index].answer : "Question: " + cards[index].question);
        }

        // This goes to the next card (delta = 1) or the one before (delta = -1).
        // Going past the last card shows the results.
        function go(delta) {
            const target = index + delta;
            if (target < 0) return;
            if (target >= cards.length) {
                showResults();
                return;
            }
            showCard(target);
        }

        // This saves "I got it" (true) or "I missed it" (false) for the card, then moves on.
        function rate(value) {
            ratings[index] = value;
            go(1);
        }

        // This shows the results box at the end.
        function showResults() {
            const got = ratings.filter((rating) => rating === true).length;
            const missed = ratings.filter((rating) => rating === false).length;
            const skipped = cards.length - got - missed;

            statGot.textContent = got;
            statMissed.textContent = missed;
            statSkipped.textContent = skipped;

            // The title and the line depend on how many were right.
            if (got + missed === 0) {
                resultTitle.textContent = "You went through the cards.";
                resultLine.textContent = "You did not mark any card. Study again and mark each one.";
            } else {
                const share = got / cards.length;
                resultTitle.textContent = share >= 0.8 ? "Great job!" : share >= 0.5 ? "Good effort!" : "Keep practicing!";
                resultLine.textContent = "You got " + got + " of " + cards.length + " right.";
            }

            deckArea.hidden = true;
            results.hidden = false;
            resultTitle.focus();   // this moves the keyboard and screen reader to the results
        }

        // This starts again from the first card and clears the score.
        function restart() {
            ratings.fill(null);
            results.hidden = true;
            deckArea.hidden = false;
            showCard(0);
            flipCard.focus({ preventScroll: true });
        }

        // ===== CONNECTING THE CLICKS AND KEYS =====

        // Clicking the card turns it over.
        flipCard.addEventListener("click", toggleFlip);

        // Pressing Enter or Space on the card turns it over too.
        // preventDefault stops the page from scrolling down when Space is pressed.
        flipCard.addEventListener("keydown", (event) => {
            if (event.key === "Enter" || event.key === " ") {
                event.preventDefault();
                toggleFlip();
            }
        });

        // The four buttons.
        btnGot.addEventListener("click", () => rate(true));
        btnMissed.addEventListener("click", () => rate(false));
        btnPrev.addEventListener("click", () => go(-1));
        btnNext.addEventListener("click", () => go(1));
        btnRestart.addEventListener("click", restart);

        // The arrow keys move between cards: right goes to the next one, left to the one before.
        // We do nothing if Alt, Ctrl, Shift, or Command is held down, so the browser's own shortcuts still work
        // (for example Alt + Left Arrow goes back to the last page).
        document.addEventListener("keydown", (event) => {
            if (event.altKey || event.ctrlKey || event.metaKey || event.shiftKey) return;
            if (deckArea.hidden) return;

            if (event.key === "ArrowRight") {
                go(1);
            } else if (event.key === "ArrowLeft") {
                go(-1);
            }
        });

        // ===== START =====
        // Show the first card. We pass "false" so the page does not read it aloud as soon as it opens.
        showCard(0, false);
    }
}