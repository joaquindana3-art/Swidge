// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

document.addEventListener('DOMContentLoaded', () => {
	const backLink = document.querySelector('.back-link');

	if (backLink) {
		backLink.addEventListener('click', (event) => {
			event.preventDefault();
			window.history.back();
		});
	}
});
