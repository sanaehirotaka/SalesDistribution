// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
document.addEventListener("DOMContentLoaded", e => {
    document.querySelectorAll("form").forEach(form => {
        const rute = form.querySelector(`input[name="Rute"]`);
        if (rute) {
            form.addEventListener("submit", e => {
                rute.value = e.submitter.name ?? "";
            });
        }
    });
});
