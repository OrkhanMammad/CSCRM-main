using CSCRM.DataAccessLayers;
using CSCRM.Models;
using CSCRM.ViewModels.AccountVMs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CSCRM.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<AppUser> _signInManager;
        public AccountController(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager, SignInManager<AppUser> signInManager, AppDbContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _context = context;

        }
        [HttpGet]
        public IActionResult SignIn()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SignIn(SignInVM signInVM)
        {
            try
            {
                if (!ModelState.IsValid)
                    return View(signInVM);

                AppUser appUser = await _userManager.FindByNameAsync(signInVM.Username);

                if (appUser == null)
                {
                    ModelState.AddModelError("", "Email or Password is not correct");
                    return View(signInVM);
                }

                Microsoft.AspNetCore.Identity.SignInResult signInResult = await _signInManager.PasswordSignInAsync(appUser, signInVM.Password, true, false);



                if (!signInResult.Succeeded)
                {
                    ModelState.AddModelError("", "Username or Password is not correct");
                    return View(signInVM);
                }
                var roles = await _userManager.GetRolesAsync(appUser);
                if (roles.Any(r => r == "Admin"))
                {
                    return RedirectToAction("Index", "Hotel", new { area = "Manage" });
                }
                if (roles.Any(r => r == "Operation"))
                {
                    return RedirectToAction("Index", "Client", new { area = "Operation" });
                }

                if (roles.Any(r => r == "Sale"))
                {
                    return RedirectToAction("Index", "Client", new { area = "Sale" });
                }
                if (roles.Any(r => r == "Reservation"))
                {
                    return RedirectToAction("Index", "Reservation", new { area = "Reservation" });
                }

                return View(signInVM);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error Occured");
                return View(signInVM);

            }
        }

        [HttpGet]
        public async Task<IActionResult> LogOut()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("SignIn");
        }


    }
}
