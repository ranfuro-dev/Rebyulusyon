// ==========================================================
// This file is site.js.
// Right now it does ONE thing: the fade-in animation when you scroll down the page.
// Each part of the page that has the word "data-reveal" in its HTML starts hidden.
// The FIRST time you scroll to it, this code shows it, and it stays visible after that.
// The look of the animation (how it fades and moves) is in site.css, in part 11.
// ==========================================================


// ===== SCROLL ANIMATION (fade in one time) =====

// This finds every part of the page that has "data-reveal" in its HTML.
// For example: the hero text, the feature cards, the About page cards, and the blue banner.
const revealTargets = document.querySelectorAll("[data-reveal]");

// This is a small helper. It shows one part of the page.
// It adds the word "is-visible" to it, and site.css uses that word to fade the part in.
function revealElement(element) {
    element.classList.add("is-visible");
}

// Only continue if the page has at least one part to animate.
if (revealTargets.length > 0) {

    // Very old browsers cannot watch the screen like this ("IntersectionObserver").
    // In that case, we just show everything right away, so nothing stays hidden.
    if (!("IntersectionObserver" in window)) {
        revealTargets.forEach(revealElement);
    } else {

        // This is a "watcher". It watches the parts of the page and runs the code below
        // every time one of them comes into view (or leaves the view).
        // "entries" is the list of parts that just changed.
        const revealObserver = new IntersectionObserver((entries) => {
            entries.forEach((entry) => {

                // Show the part if it is on the screen now,
                // OR if it is already ABOVE the screen (its top is less than 0).
                // The second case is for when you reload the page halfway down, or scroll very fast.
                // Without it, the parts you skipped would stay hidden forever.
                if (entry.isIntersecting || entry.boundingClientRect.top < 0) {
                    revealElement(entry.target);

                    // Stop watching this part. This is why the animation happens only ONE time:
                    // when you scroll up and down again, the part is not watched anymore,
                    // and it stays visible.
                    revealObserver.unobserve(entry.target);
                }
            });
        }, {
            threshold: 0,                   // runs as soon as even 1 pixel of the part is in view
            rootMargin: "0px 0px -8% 0px"   // the watched area is 8% shorter at the bottom of the screen,
            // so a part starts to fade in a little after it appears,
            // and not at the very edge of the screen
        });

        // Tell the watcher to start watching every part that has "data-reveal".
        revealTargets.forEach((element) => revealObserver.observe(element));
    }
}