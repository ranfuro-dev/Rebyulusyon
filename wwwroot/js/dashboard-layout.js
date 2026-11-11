// ==========================================================
// This file is dashboard-layout.js.
// It makes the sidebar menu work on PHONES: the menu button opens the sidebar like a drawer, and it can be closed again.
// On computers the sidebar is always visible, so this file does nothing there.
// The menu can be closed with the "X" button, the Escape key, or a click on the dark layer behind it.
// While it is open, the Tab key stays inside the menu, so keyboard users do not get lost in the page behind it.
// ==========================================================

(function () {

    // This finds the parts of the page by their "id" in _DashboardLayout.cshtml.
    const sidebar = document.getElementById("sidebar");
    const menuBtn = document.getElementById("menuBtn");      // the three-line button in the top bar
    const closeBtn = document.getElementById("sidebarClose"); // the "X" button inside the sidebar
    const backdrop = document.getElementById("sidebarBackdrop"); // the dark layer behind the open menu

    // If something is missing, we stop here, so the page still works.
    if (!sidebar || !menuBtn || !backdrop) return;

    // This asks the browser if the screen is a phone size (the same size as in dashboard-layout.css).
    const phoneSize = window.matchMedia("(max-width: 900px)");

    // This remembers if the menu is open now.
    let isOpen = false;

    // ===== OPEN THE MENU =====
    function openMenu() {
        if (isOpen) return;
        isOpen = true;

        sidebar.classList.add("is-open");                    // the CSS slides the sidebar in
        backdrop.hidden = false;                             // show the dark layer
        menuBtn.setAttribute("aria-expanded", "true");       // tell screen readers that the menu is open
        document.body.classList.add("no-scroll");            // the page behind does not scroll

        // Put the keyboard on the close button, so it is inside the menu.
        if (closeBtn) closeBtn.focus();
    }

    // ===== CLOSE THE MENU =====
    // "returnFocus" is true when a person closed it: the keyboard goes back to the menu button.
    function closeMenu(returnFocus) {
        if (!isOpen) return;
        isOpen = false;

        sidebar.classList.remove("is-open");
        backdrop.hidden = true;
        menuBtn.setAttribute("aria-expanded", "false");
        document.body.classList.remove("no-scroll");

        if (returnFocus) menuBtn.focus();
    }

    // ===== THE CLICKS =====
    menuBtn.addEventListener("click", () => (isOpen ? closeMenu(true) : openMenu()));
    if (closeBtn) closeBtn.addEventListener("click", () => closeMenu(true));
    backdrop.addEventListener("click", () => closeMenu(true));

    // ===== THE KEYS =====
    document.addEventListener("keydown", (event) => {
        if (!isOpen) return;

        // Escape closes the menu.
        if (event.key === "Escape") {
            event.preventDefault();
            closeMenu(true);
            return;
        }

        // Tab stays inside the menu: after the last link it goes back to the first, and the other way round with Shift + Tab.
        if (event.key === "Tab") {
            const items = Array.from(sidebar.querySelectorAll("a[href], button:not([disabled])"));
            if (items.length === 0) return;

            const first = items[0];
            const last = items[items.length - 1];
            const active = document.activeElement;

            if (!sidebar.contains(active)) {
                event.preventDefault();
                first.focus();
            } else if (event.shiftKey && active === first) {
                event.preventDefault();
                last.focus();
            } else if (!event.shiftKey && active === last) {
                event.preventDefault();
                first.focus();
            }
        }
    });

    // ===== IF THE SCREEN BECOMES BIG =====
    // For example, a tablet is turned sideways. The sidebar is always visible on a big screen, so we close the drawer.
    const onSizeChange = (event) => {
        if (!event.matches) closeMenu(false);
    };

    if (phoneSize.addEventListener) {
        phoneSize.addEventListener("change", onSizeChange);
    } else if (phoneSize.addListener) {
        // Very old browsers.
        phoneSize.addListener(onSizeChange);
    }
})();