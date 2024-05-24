document.addEventListener('DOMContentLoaded', function () {
    var selectElements = document.querySelectorAll('.anek-single__info-mark');

    selectElements.forEach(function (select) {
        setColor(select);
    });

    function setColor(element) {
        if (element.textContent > 0) {
            element.style.color = 'lightgreen';
        }

        if (element.textContent < 0) {
            element.style.color = 'red';
        }

        if (element.textContent === 0) {
            element.style.color = 'white';
        }
    }
});