const dropdown_buttons = document.querySelectorAll('.dropdown__button')

for (var el of dropdown_buttons) {
    const element = el;
    el.addEventListener('click', function (event) {
        element.setAttribute('open', !(element.getAttribute('open') === 'true'))
    });

    // el.addEventListener('focusout', function (event) {
    //     element.setAttribute('open', false)
    //     event.stopPropagation();
    // });
}

const content_popular = document.querySelector('.selection__popular-container')
const content_more_viewed = document.querySelector('.selection__more-viewed-container')

const container_popular = document.querySelector('.selection__popular-values-container')
const container_more_viewed = document.querySelector('.selection__more-viewed-values-container')

const button_popular = document.querySelector('.selection__popular-button')
const button_viewed = document.querySelector('.selection__more-viewed-button')
window.addEventListener('click', (event) => {
    const isDropdownClicked = (event.target === container_popular || event.target === button_popular);
    button_popular.setAttribute('open', String(isDropdownClicked));
})

window.addEventListener('click', (event) => {
    const isDropdownClicked = (event.target === content_more_viewed || event.target === button_viewed);
    button_viewed.setAttribute('open', String(isDropdownClicked));
})