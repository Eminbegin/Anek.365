function disable(elements) {
    elements.forEach(element => {
        element.classList.add('blur', 'none-events')
    })
}

function enable(elements) {
    elements.forEach(element => {
        element.classList.remove('blur', 'none-events')
    })
}

document.addEventListener("DOMContentLoaded", function () {
    const elements = [
        document.getElementsByTagName('main')[0],
        document.getElementsByTagName('header')[0],
        document.getElementsByTagName('footer')[0]
    ]
    const popup = document.getElementById('popup');
    const closeButton = document.getElementById('close-btn');

    // Проверяем, было ли всплывающее окно уже показано
    if (!localStorage.getItem('popupShown')) {
        popup.style.display = 'block';
        disable(elements)
        localStorage.setItem('popupShown', 'true');
    }

    closeButton.addEventListener('click', function () {
        popup.style.display = 'none';
        enable(elements)
    });

    window.addEventListener('click', function (event) {
        if (event.target === popup) {
            popup.style.display = 'none';
            enable(elements)
        }
    });
});

const clearButton = document.getElementsByClassName('nav__clear-button')[0];
clearButton.addEventListener('click', function () {
    localStorage.clear();
    alert('Local Storage очищен!');
});