anekIds = Array.from(document
    .querySelectorAll('.anek-single'))
    .map(elem => Number.parseInt(elem.dataset.id));

const update_all = new signalR.HubConnectionBuilder()
    .withUrl("/page-data")
    .build();

mark_elemetns = document.querySelectorAll('.anek-single__info-mark');

view_elements = document.querySelectorAll('.anek-single__info-views');

update_all.on("UpdateAllData", function (marks, views) {
    mark_elemetns.forEach(el => {
        el.textContent = marks[Number.parseInt(el.dataset.id)];
        setColor(el);
    });
    view_elements.forEach(el => {
            el.textContent = views[Number.parseInt(el.dataset.id)];
        }
    );
});

update_all.start().then(function () {
    setInterval(() => {
        update_all.invoke("UpdateAllData", anekIds)
            .catch(function (err) {
                return console.error(err.toString());
            });
    }, 5000);
}).catch(function (err) {
    return console.error(err.toString());
});

function setColor(element) {
    let value = Number.parseInt(element.textContent);

    if (value > 0) {
        element.style.color = 'lightgreen';
    }

    if (value < 0) {
        element.style.color = 'red';
    }

    if (value === 0) {
        element.style.color = 'white';
    }
}