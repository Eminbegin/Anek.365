
anekId = Number.parseInt(document.querySelector('.anek__views').dataset.anek);

mark_element = document.querySelector('.anek-single__info-mark')
views_element = document.querySelector('.anek__views')
button_up = document.querySelector('.anek__mark-up');
button_down = document.querySelector('.anek__mark-down');

const connection = new signalR.HubConnectionBuilder()
    .withUrl("/update-data")
    .build();


const rateConnection = new signalR.HubConnectionBuilder()
    .withUrl("/rate-anek")
    .build();

connection.on("UpdateData", function (mark, views) {
    mark_element.textContent = mark;
    views_element.textContent = views;
    setColor(mark_element)
});

button_up.addEventListener('click', async function () {
    await rateConnection.invoke("RateAnek", Number.parseInt(anekId), 1);
    connection.invoke("UpdateData", anekId);
});

button_down.addEventListener('click', async function () {
    await rateConnection.invoke("RateAnek", Number.parseInt(anekId), -1);
    connection.invoke("UpdateData", anekId);
});

rateConnection.start().catch(err => console.error(err.toString()));
connection.start().then(function () {
    setInterval(() => {
        connection.invoke("UpdateData", anekId)
            .catch(function (err) {
                return console.error(err.toString());
            });
    }, 20000);
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
