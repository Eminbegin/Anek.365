document.addEventListener('DOMContentLoaded', function () {
    const sections = document.querySelectorAll('.anek-single');

    sections.forEach(section => {
        section.addEventListener('mouseover', function () {
            this.querySelector('.anek-single__text').textContent = this.dataset.title;
        });

        section.addEventListener('mouseout', function () {
            this.querySelector('.anek-single__text').textContent = this.dataset.text;
        });
    });
});
