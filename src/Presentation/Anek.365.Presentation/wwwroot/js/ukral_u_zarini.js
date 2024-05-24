document.addEventListener("DOMContentLoaded", function () {
    let currentNavId = document.location.pathname
        .split('/')
        .pop()
        .split('.')
        .at(0);
    currentNavId = 'nav-' + currentNavId;

    const navButton = document.getElementById(currentNavId);
    navButton.classList.add('current');
});