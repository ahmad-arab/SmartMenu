window.addEventListener("DOMContentLoaded", () => {
	const sections = document.querySelectorAll(".reveal");
	sections.forEach((section, i) => {
		setTimeout(() => {
			section.classList.add("visible");
		}, 120 + i * 120);
	});
});
