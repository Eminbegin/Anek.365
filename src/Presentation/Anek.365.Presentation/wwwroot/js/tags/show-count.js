document.addEventListener('DOMContentLoaded', function () {
    const sections = document.querySelectorAll('.tag__single');

    sections.forEach(section => {
        section.addEventListener('mouseover', function () {
            this.querySelector('.tag__element').textContent = this.dataset.count;
        });

        section.addEventListener('mouseout', function () {
            this.querySelector('.tag__element').textContent = this.dataset.name;
        });
    });
});