(() => {
    const loadTimeSpan = document.querySelector('.load-time');
    window.addEventListener('load', () => {
        const pageEnd = performance.mark('pageEnd');
        const loadTime = pageEnd.startTime / 1000;
        loadTimeSpan.innerHTML += `Page loaded in ${Math.floor(100000 * loadTime) / 100} ms.`;
    });
})();

document.addEventListener("DOMContentLoaded", function() {
    // Добавление обработчика событий для каждой навигационной кнопки
    document.querySelectorAll('.nav-button').forEach(function(button) {
        button.addEventListener('mouseover', function() {
            // Действия при наведении на кнопку
            button.style.boxShadow = '0 0 10px orange';
        });

        button.addEventListener('mouseout', function() {
            // Действия при убирании курсора с кнопки
            button.style.boxShadow = 'none';
        });
    });
});