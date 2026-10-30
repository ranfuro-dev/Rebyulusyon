// ==========================================================
// This file is auth.js.
// It is used by the Get Started (Register) page, the Log In page, and the Contact page.
// It does three things:
// 1) Makes the eye button show and hide the password.
// 2) Checks the form and shows red messages under the boxes.
// 3) Shows a small spinning circle on the button while the form is being sent.
// NOTE: These checks are only for the DESIGN for now.
// Later, the server (C#) will do the real checks, because JavaScript checks are easy to skip.
// ==========================================================


// ===== 1. THE SHOW / HIDE PASSWORD BUTTONS =====

// This finds every eye button on the page and gets each one ready for a click.
document.querySelectorAll(".toggle-password").forEach((button) => {
    button.addEventListener("click", () => {

        // Each button knows which box it controls. The box id is written in the HTML as data-target.
        const input = document.getElementById(button.dataset.target);
        if (!input) return; // if the box is not found, do nothing

        // If the box is hidden now (type "password"), we show it. If not, we hide it again.
        const show = input.type === "password";
        input.type = show ? "text" : "password";

        // This adds or removes the word "is-shown", so the CSS can swap the eye picture.
        button.classList.toggle("is-shown", show);

        // These two lines tell screen readers what the button does now.
        button.setAttribute("aria-pressed", String(show));
        button.setAttribute("aria-label", show ? "Hide password" : "Show password");
    });
});


// ===== 2. THE FORM CHECKS =====

// This finds the form. It only continues if the page has a form with the class "auth-form".
const form = document.querySelector("form.auth-form");

if (form) {

    // This turns off the browser's own pop-up messages, so we can show our own messages under each box.
    // (We turn it off here and not in the HTML, so if JavaScript fails, the browser's checks still work.)
    form.noValidate = true;

    // This is a list of checks. Each check gets the text typed in a box and gives back a message.
    // If the message is empty (""), the box is OK.
    const rules = {

        // A box that only must not be empty (like the Log In boxes, and the Name and Subject boxes on the Contact page).
        // The message can be set in the HTML with data-required-message.
        required: (value, input) =>
            value.trim() === "" ? (input.dataset.requiredMessage || "Please fill out this field.") : "",

        // The username box on the Get Started page.
        username: (value) => {
            if (value.trim() === "") return "Please choose a username.";
            if (value.length < 3 || value.length > 20) return "Your username must be 3 to 20 characters.";
            if (!/^[A-Za-z0-9_.]+$/.test(value)) return "Use only letters, numbers, underscores (_) and periods (.).";
            return "";
        },

        // The email box on the Get Started page and the Contact page.
        email: (value) => {
            if (value.trim() === "") return "Please enter your email address.";
            // This checks for the simple shape: something, an @ sign, something, a dot, something.
            if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(value)) return "Please enter a valid email address.";
            return "";
        },

        // The new password box on the Get Started page.
        newpassword: (value) => {
            if (value === "") return "Please create a password.";
            if (value.length < 8) return "Your password must be at least 8 characters.";
            if (!/[A-Z]/.test(value) || !/[a-z]/.test(value) || !/[0-9]/.test(value)) {
                return "Add an uppercase letter, a lowercase letter, and a number.";
            }
            return "";
        },

        // The "Confirm Password" box. It must be the same as the password box.
        // The HTML says which box to compare with, using data-match.
        confirm: (value, input) => {
            if (value === "") return "Please confirm your password.";
            const other = document.getElementById(input.dataset.match);
            if (other && value !== other.value) return "The passwords do not match.";
            return "";
        },

        // NEW: The message box on the Contact page. It must not be empty,
        // it must have at least 10 letters, and it cannot be longer than 1000 letters.
        message: (value) => {
            const text = value.trim();
            if (text === "") return "Please write your message.";
            if (text.length < 10) return "Your message must be at least 10 characters.";
            if (text.length > 1000) return "Your message must be 1000 characters or less.";
            return "";
        }
    };

    // CHANGED: This is the list of all the boxes that have a "data-rule" in the HTML.
    // It now includes text boxes (input), drop-down lists (select), and message boxes (textarea).
    const fields = Array.from(form.querySelectorAll("input[data-rule], select[data-rule], textarea[data-rule]"));

    // This checks ONE box. It writes the message under the box, marks the box red if there is a problem,
    // and gives back true (the box is OK) or false (there is a problem).
    function validateField(input) {
        const rule = rules[input.dataset.rule];
        const message = rule ? rule(input.value, input) : "";

        // The message goes into the empty line under the box. Its id is the box id plus "-error".
        const errorBox = document.getElementById(input.id + "-error");
        if (errorBox) errorBox.textContent = message;

        // This marks the box. The CSS shows a red border when this is "true".
        input.setAttribute("aria-invalid", message ? "true" : "false");

        return message === "";
    }

    // This gets every box ready to be checked at the right time.
    fields.forEach((input) => {
        let touched = false; // becomes true after the person has typed something in this box

        // When the person leaves the box, check it, but only if something was typed.
        // (So we do not show red messages on empty boxes the person has not used yet.)
        input.addEventListener("blur", () => {
            if (input.value !== "") {
                touched = true;
                validateField(input);
            }
        });

        // While typing (or choosing from a list), check again, but only after the box was checked once.
        // This way the message goes away as soon as the problem is fixed.
        input.addEventListener("input", () => {
            if (touched) validateField(input);

            // If the password changes, the "Confirm Password" box must be checked again too.
            if (input.dataset.rule === "newpassword") {
                const confirmBox = fields.find((other) => other.dataset.match === input.id);
                if (confirmBox && confirmBox.value !== "") validateField(confirmBox);
            }
        });
    });


    // ===== 3. WHEN THE BUTTON IS PRESSED =====

    // This finds the button that sends the form, and the words inside it.
    const submitButton = form.querySelector("button[type='submit']");
    const label = submitButton ? submitButton.querySelector(".btn-label") : null;
    const originalText = label ? label.textContent : "";

    form.addEventListener("submit", (event) => {

        // Check ALL the boxes, and remember the first box with a problem.
        let firstInvalid = null;
        fields.forEach((input) => {
            if (!validateField(input) && !firstInvalid) firstInvalid = input;
        });

        // If there is a problem: stop the form from being sent, and put the cursor in the first box with a problem.
        if (firstInvalid) {
            event.preventDefault();
            firstInvalid.focus();
            return;
        }

        // If everything is OK: disable the button and show the spinning circle,
        // so the button cannot be clicked twice.
        if (submitButton && label) {
            submitButton.disabled = true;
            submitButton.classList.add("is-loading");
            label.textContent = submitButton.dataset.loadingText || originalText;
        }
    });

    // If the person comes back with the browser's Back button, put the button back to normal.
    window.addEventListener("pageshow", () => {
        if (submitButton && label) {
            submitButton.disabled = false;
            submitButton.classList.remove("is-loading");
            label.textContent = originalText;
        }
    });
}