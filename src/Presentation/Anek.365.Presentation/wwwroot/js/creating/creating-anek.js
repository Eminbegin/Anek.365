function selectTag(element) {
    const selectedTags = document.getElementById('selectedTags');
    selectedTags.appendChild(element);
    element.onclick = function () {
        deselectTag(this);
    };
    sortList(selectedTags)
}

function deselectTag(element) {
    const availableTags = document.getElementById('availableTags');
    availableTags.appendChild(element);
    element.onclick = function () {
        selectTag(this);
    };
    sortList(availableTags)
}

document.querySelector('.submit-button').addEventListener('click', function () {
    document.getElementById('title').value = document.querySelector('.anek-form__input-title').value;
    document.getElementById('content').value = document.querySelector('.anek-form__input-text').value;
    document.getElementById('tags').value = Array
        .from(document
            .getElementById('selectedTags')
            .getElementsByTagName('li'))
        .map(tag => tag.dataset.id)
        .join(' ')
    document.getElementById('dataForm').submit();
});


function sortList(ul) {
    Array.from(ul.getElementsByTagName('li'))
        .sort((a, b) => a.textContent.localeCompare(b.textContent))
        .forEach(li => ul.appendChild(li));
}

sortList(document.getElementById('availableTags'));