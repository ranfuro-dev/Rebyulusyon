// Mobile menu: show/hide the nav links when the hamburger button is clicked.
const header = document.getElementById("siteHeader");
const toggle = document.getElementById("navToggle");

if (header && toggle) {
    toggle.addEventListener("click", () => {
        const isOpen = header.classList.toggle("open"); // add or remove the "open" class
        toggle.setAttribute("aria-expanded", String(isOpen)); // keeps screen readers informed
    });
}


/* ===== 8. FEATURES SECTION ===== */

/* The whole section: spacing and a faint dotted background.
   radial-gradient draws a tiny dot; background-size repeats it every 24px */
.features {
    padding: 100px 0;
    background-color: var(--bg);
    background-image: radial-gradient(#d5def5 1px, transparent 1px);
    background-size: 24px 24px;
}

/* The centered heading block above the cards */
.section-head { text-align: center; max-width: 680px; margin: 0 auto 56px; }

/* The big heading "Built around how students actually review" */
.section-head h2 {
    font-family: var(--font-head);
    font-size: clamp(1.9rem, 4vw, 2.6rem);
    font-weight: 700;
    line-height: 1.15;
    margin: 12px 0 14px;
}

/* The paragraph under the heading */
.section-sub { color: var(--muted); font-size: 17px; margin: 0; }

/* The small blue label, centered, with a thin line on each side.
   ::before and ::after draw the two lines without any extra HTML */
.eyebrow-lines { display: inline-flex; align-items: center; gap: 12px; margin: 0; font-size: 12px; }
.eyebrow-lines::before,
.eyebrow-lines::after { content: ""; width: 32px; height: 1px; background: #9fb4ea; }

/* The cards grid: 3 equal columns on desktop */
.feature-grid { display: grid; grid-template-columns: repeat(3, 1fr); gap: 24px; }

/* One card: white box with a rounded border */
.feature-card {
    background: #fff;
    border: 1px solid #d5def5;
    border-radius: 22px;
    padding: 26px;
}

/* The card title (an h3 heading) */
.feature-card h3 { font-family: var(--font-head); font-size: 20px; font-weight: 600; line-height: 1.25; margin: 18px 0 10px; }

/* The card description */
.feature-card p { margin: 0; font-size: 15.5px; color: var(--muted); }

/* The round badge that holds the icon */
.icon-chip { width: 48px; height: 48px; border-radius: 50%; display: grid; place-items: center; font-size: 22px; }

/* Badge color versions. The name matches the "tone" text in the C# list */
.tone-blue   { background: var(--blue-soft);   color: var(--blue); }
.tone-red    { background: var(--red-soft);    color: var(--red); }
.tone-yellow { background: var(--yellow-soft); color: #5a4500; }

/* Tablets (narrower than 960px): 2 cards per row */
@media (max-width: 960px) {
    .features { padding: 72px 0; }
    .feature-grid { grid-template-columns: repeat(2, 1fr); }
}

/* Phones (narrower than 600px): 1 card per row */
@media (max-width: 600px) {
    .feature-grid { grid-template-columns: 1fr; }
}