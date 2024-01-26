// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

let viewProfileBtn = document.querySelector("#view-profile-btn");
let viewLogoutBtn = document.querySelector("#view-logout-btn");
let noLogoutBtn = document.querySelector("#no-logout-btn");

viewProfileBtn.addEventListener('click', () => {
    $("#account_settings").modal("hide")
    $("#profile").modal("show")
})

viewLogoutBtn.addEventListener('click', () => {
    $("#account_settings").modal("hide")
    $("#logout").modal("show")
})

noLogoutBtn.addEventListener('click', () => {
    $("#logout").modal("hide")

})