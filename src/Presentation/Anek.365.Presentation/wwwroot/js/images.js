async function getPhotos() {
  let data = await fetch("https://jsonplaceholder.typicode.com/photos");
  let photos = await data.json();
  let filteredPhotos = photos.slice(100, 110).map((photo) => photo.url);
  let imageContainers = document.getElementsByClassName("image-card");
  [...imageContainers].forEach((el, idx) => {
    let image = document.createElement("img");
    image.setAttribute("class", "card__image");
    image.setAttribute("src", filteredPhotos[idx]);
    el.innerHTML = "";
    el.append(image);
  });
}
