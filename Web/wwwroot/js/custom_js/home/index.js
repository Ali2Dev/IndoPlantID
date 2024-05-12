//JavaScript of responsive navigation menu

const menuBtn = document.querySelector(".menu-btn");
const navegation = document.querySelector(".navegation");

menuBtn.addEventListener("click", () => {
    //si la clase "active" está presente la elimina, de lo contrario la añade
    menuBtn.classList.toggle("active");
    navegation.classList.toggle("active");
});

//JavaScript for video slider navegation
const btns = document.querySelectorAll(".nav-btn");
const slides = document.querySelectorAll(".video-slide");
const contents = document.querySelectorAll(".content");

var sliderNav = function (manual) {
    //button
    btns.forEach((btn) => {
        btn.classList.remove("active");
    });

    //video
    slides.forEach((slide) => {
        slide.classList.remove("active");
    });

    //content
    contents.forEach((content) => {
        content.classList.remove("active");
    });

    //button
    btns[manual].classList.add("active");
    //video
    slides[manual].classList.add("active");
    //content
    contents[manual].classList.add("active");
};

btns.forEach((btn, i) => {
    btn.addEventListener("click", () => {
        sliderNav(i);
    });
});
