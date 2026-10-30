using InvoiceManagement.Models.Data.InvoiceDbContext;
using InvoiceManagement.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using InvoiceManagement.Models;

namespace InvoiceManagement.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;     
        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid) 
            { 
                return View(model);
            }
            var  currentUser = await _userManager.FindByEmailAsync(model.Email);
            if (currentUser == null) 
            {
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                return View(model);
            }
            if (currentUser.CompanyId != null)
            { 
            
                if (currentUser.CompanyId == null)
                    {
                    ModelState.AddModelError(string.Empty, "You are not associated with any company.");
                    return View(model);
                }

            }

            var result = await _signInManager.PasswordSignInAsync(currentUser, model.Password, model.RememberMe, lockoutOnFailure: false);
            if (result.Succeeded) { }
            else
            {
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                return View(model);
            }
            return await RedirectcurrentUser(currentUser);
        }

        private async Task<IActionResult> RedirectcurrentUser(ApplicationUser currentUser)
        {
            if (await _userManager.IsInRoleAsync(currentUser, "SystemAdmin"))
            {
                return RedirectToAction("Index", "SystemDashboard");
            }

            if (await _userManager.IsInRoleAsync(currentUser, "Admin"))
            {
                return RedirectToAction("Dashboard", "Admin");
            }

            if (await _userManager.IsInRoleAsync(currentUser, "Cashier"))
            {
                return RedirectToAction("Create", "Invoice");
            }

            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();

            return RedirectToAction(nameof(Login));
        }

        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
