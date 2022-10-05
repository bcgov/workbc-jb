/*
* Global javascript
*/
$(document).ready(function () {

    function setupMenuControl() {
        const useCapture = false;
        const show = elem => elem.style.display = 'block';
        const hide = elem => elem.style.display = 'none';
        const toggle = (elem) => {
            if (window.getComputedStyle(elem).display === 'block') {
                hide(elem);
                return;
            }
            show(elem);
        };
        // Listen for click events
        const menuButton = document.getElementById('menu');
        menuButton.addEventListener('click', (event) => {
            // Filter events
            if (event.target.id !== 'menu') return;
            // Prevent default link behavior
            event.preventDefault();
            // Check for a valid target
            const nav = document.getElementById('navbar');
            if (!nav) return;
            // Toggle
            toggle(nav);
        }, useCapture);
    }

    setupMenuControl();

    $("#back-to-top").click(function () {
        document.body.scrollTop = 0;
        document.documentElement.scrollTop = 0;
    });

    $('body').on('click', 'input.show-spinner, a.show-spinner, button.show-spinner', function () {
        $('div#progress-overlay').show();
    });

    // Clear DataTables state from localstorage every time the top nav is clicked
    // The user is going to a different section and state needs to be reset when they return
    $("#navbar a").click(function() {
        localStorage.removeItem("DataTables_dtJobSeekers");
        localStorage.removeItem("DataTables_dtJobs");
        localStorage.removeItem("DataTables_dtSettings");
        localStorage.removeItem("DataTables_dtAdminUsers");
        localStorage.removeItem("DataTables_dtAdminUsersShadow");
    });
});
