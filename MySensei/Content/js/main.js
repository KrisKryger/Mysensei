var navToggle = document.querySelector("#navToggle"),
	navDiv = document.querySelector(".navDiv"),
    navBG = document.querySelector(".navBG"),
	profileToggle = document.querySelector(".profileToggle"),
    profileDiv = document.querySelector(".profileDiv"),
	profileBG = document.querySelector(".profileBG"),
    userIcon = document.querySelector("user-icon"),
	userIconOrange = document.querySelector("user-icon-orange");

navToggle.addEventListener("click", function () {
    navToggle.classList.toggle("active");
    navDiv.classList.toggle("navToggled");
    navBG.classList.toggle("navToggledBG");
    profileDiv.classList.remove("profileToggled");
    profileBG.classList.remove("profileToggledBG");
});

profileToggle.addEventListener("click", function () {
    profileDiv.classList.toggle("profileToggled");
    profileBG.classList.toggle("profileToggledBG");
    navToggle.classList.remove("active");
    navDiv.classList.remove("navToggled");
    navBG.classList.remove("navToggledBG");
});