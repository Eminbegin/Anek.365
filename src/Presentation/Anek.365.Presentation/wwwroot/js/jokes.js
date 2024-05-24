function addLocalJokes() {
    const lsData = JSON.parse(localStorage.getItem("jokes")) || {};
    let tableBody = document.querySelector("tbody");
    Object.keys(lsData).forEach((idx) => {
        let localJoke = lsData[idx];
        let newRow = document.createElement("tr");
        newRow.setAttribute("class", "jokes__row");
        newRow.innerHTML += `
      <td class="jokes__data">${
            document.getElementsByClassName("jokes__row").length
        }</td>
      <td class="jokes__data">${localJoke.title}</td>
      <td class="jokes__data">${localJoke.content}</td>
    `;
        tableBody.append(newRow);
    });
}

function addJoke(event) {
    event.preventDefault();
    let formInputs = document.getElementsByClassName("jokes-form__input-title");
    let formInputs2 = document.getElementsByClassName("jokes-form__input-text");
    let tableBody = document.querySelector("tbody");
    let newRow = document.createElement("tr");
    newRow.setAttribute("class", "jokes__row");
    let idx = document.getElementsByClassName("jokes__row").length;
    let title = formInputs[0].value;
    let content = formInputs2[0].value;
    newRow.innerHTML += `
    <td class="jokes__data" <td class="jokes__data-cell">${idx}</td> </td>
    <td class="jokes__data-title" <td class="jokes__data-cell">${title}</td> </td>
    <td class="jokes__data" <td class="jokes__data-cell">${content}</td> </td>
  `;
    tableBody.append(newRow);
    const lsData = JSON.parse(localStorage.getItem("jokes")) || {};
    lsData[idx] = {
        title,
        content,
    };
    localStorage.setItem("jokes", JSON.stringify(lsData));
}

let form = document.querySelector("form");
form.addEventListener("submit", addJoke);
addLocalJokes();
