const images = document.querySelectorAll("inside img");
const closeButton = document.querySelector(".close");
const lightboxImage = document.querySelector(".lightbox img");
const lightboxCaption = document.querySelector(".lightbox .caption");
const lightbox = document.querySelector(".lightbox");

images.forEach(img => {
    img.addEventListener("click", function() {
        lightbox.classList.remove("hide");
        lightboxImage.src = this.src;
        lightboxCaption.textContent = this.alt || "No caption available";
    });
});

closeButton.addEventListener("click", function() {
    lightbox.classList.add("hide");
});

lightbox.addEventListener("click", function(event) {
    if (event.target === lightbox) {
        lightbox.classList.add("hide");
    }
});