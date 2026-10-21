// ===== SCROLL REVEAL =====
// Fades each [data-reveal] element in the FIRST time it scrolls into view.
// After that it stays visible permanently: the observer stops watching it,
// and the "is-visible" class is never removed.

// Finds every element on the page that has the data-reveal attribute
const revealTargets = document.querySelectorAll("[data-reveal]");

// Shows one element by adding the class that the CSS turns into "visible"
function revealElement(element) {
    element.classList.add("is-visible");
}

if (revealTargets.length > 0) {

    // Very old browsers do not have IntersectionObserver: just show everything
    if (!("IntersectionObserver" in window)) {
        revealTargets.forEach(revealElement);
    } else {

        // The observer watches elements and calls this function when their visibility changes.
        // "entries" is the list of elements whose visibility changed.
        const revealObserver = new IntersectionObserver((entries) => {
            entries.forEach((entry) => {

                // Reveal if the element is on screen, OR if it is already ABOVE the screen
                // (for example after reloading halfway down the page, or after a very fast scroll).
                // Without this, skipped elements would stay hidden.
                if (entry.isIntersecting || entry.boundingClientRect.top < 0) {
                    revealElement(entry.target);

                    // Stop watching this element: this is what makes it happen only once
                    revealObserver.unobserve(entry.target);
                }
            });
        }, {
            threshold: 0,                   // fire as soon as 1 pixel is inside the watched area
            rootMargin: "0px 0px -8% 0px"   // shrink the area by 8% at the bottom, so elements
                                            // start fading in a little after they appear
        });

        // Start watching every marked element
        revealTargets.forEach((element) => revealObserver.observe(element));
    }
}