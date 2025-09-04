using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SmartMenu.Data.Entities;
using SmartMenu.Models;
using System.Security.Claims;

namespace SmartMenu.Controllers
{
    public class AccountController : Controller
    {
        private readonly ILogger<AccountController> _logger;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public AccountController(
            ILogger<AccountController> logger,
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager)
        {
            _logger = logger;
            _signInManager = signInManager;
            _userManager = userManager;
        }

        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            var model = new LoginViewModel
            {
                ReturnUrl = returnUrl
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.FindByNameAsync(model.Username);
            if (user == null)
            {
                ModelState.AddModelError(nameof(model.Username), "Invalid username or password.");
                ModelState.AddModelError(nameof(model.Password), "Invalid username or password.");
                return View(model);
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);
            if (!result.Succeeded)
            {
                ModelState.AddModelError(nameof(model.Username), "Invalid username or password.");
                ModelState.AddModelError(nameof(model.Password), "Invalid username or password.");
                return View(model);
            }

            var claims = new List<Claim>
            {
                new Claim("TenantId", user.TenantId.ToString())
            };

            await _signInManager.SignInWithClaimsAsync(user, model.RememberMe, claims);

            if (!string.IsNullOrEmpty(model.ReturnUrl))
                return Redirect(model.ReturnUrl);

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
