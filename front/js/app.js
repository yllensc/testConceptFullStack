const registerButton = document.getElementById("registerButton");
const loginButton = document.getElementById("loginButton");
const registerForm = document.getElementById("registerForm");
const loginForm = document.getElementById("loginForm");

registerButton.addEventListener("click", () => {
    registerForm.style.display = "block";
    loginForm.style.display = "none";
});

loginButton.addEventListener("click", () => {
    registerForm.style.display = "none";
    loginForm.style.display = "block";
});