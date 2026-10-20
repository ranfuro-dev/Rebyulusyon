// Mobile menu: show/hide the nav links when the hamburger button is clicked.
const header = document.getElementById("siteHeader");
const toggle = document.getElementById("navToggle");

if (header && toggle) {
    toggle.addEventListener("click", () => {
        const isOpen = header.classList.toggle("open"); // add or remove the "open" class
        toggle.setAttribute("aria-expanded", String(isOpen)); // keeps screen readers informed
    });
}