window.addEventListener("DOMContentLoaded", () => {
	const revealTargets = document.querySelectorAll(".reveal");

	if ("IntersectionObserver" in window) {
		const observer = new IntersectionObserver(
			(entries) => {
				entries.forEach((entry) => {
					if (entry.isIntersecting) {
						entry.target.classList.add("visible");
						observer.unobserve(entry.target);
					}
				});
			},
			{ threshold: 0.12 }
		);

		revealTargets.forEach((target) => observer.observe(target));
	} else {
		revealTargets.forEach((target) => target.classList.add("visible"));
	}

	const navLinks = document.querySelectorAll(".top-links a[href^='#']");
	const sections = [...navLinks]
		.map((link) => document.querySelector(link.getAttribute("href")))
		.filter(Boolean);

	if (sections.length > 0 && "IntersectionObserver" in window) {
		const sectionObserver = new IntersectionObserver(
			(entries) => {
				entries.forEach((entry) => {
					if (!entry.isIntersecting) {
						return;
					}

					navLinks.forEach((link) => {
						const isActive = link.getAttribute("href") === `#${entry.target.id}`;
						link.classList.toggle("is-active", isActive);
					});
				});
			},
			{ threshold: 0.38 }
		);

		sections.forEach((section) => sectionObserver.observe(section));
	}
});
