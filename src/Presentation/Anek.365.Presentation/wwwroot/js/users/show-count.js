document.addEventListener('DOMContentLoaded', function () {
    const sections = document.querySelectorAll('.user__single');

    sections.forEach(section => {
        section.addEventListener('mouseover', function () {
            this.querySelector('.user__element').textContent = this.dataset.count;
        });

        section.addEventListener('mouseout', function () {
            this.querySelector('.user__element').textContent = this.dataset.name;
        });
    });
});