using CSCRM.DataAccessLayers;
using CSCRM.Models;
using CSCRM.ViewModels.AccountVMs;
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
        
            //if (!ModelState.IsValid)
            //{
            //    return View(signInVM);
            //}
            //return View();

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
            catch (Exception ex) {
                ModelState.AddModelError("", "Error Occured");
                return View(signInVM);

            }
        }

        //[HttpGet]
        //public async Task CreateNewAccount()
        //{
        //    await _roleManager.CreateAsync(new IdentityRole("Developer"));
        //    AppUser user = new AppUser { Email = "Orkhanvm@gmail.com", UserName = "Orkhan123", Name = "Orkhan", SurName = "Mammadli" };
        //    await _userManager.CreateAsync(user, "Orkhan6991");
        //    await _userManager.AddToRoleAsync(user, "Developer");


        //}


        //[HttpGet]
        //public async Task CreateNewAccount()
        //{
        //    await _roleManager.CreateAsync(new IdentityRole("Operation"));
        //    AppUser user = new AppUser { Email = "Operation@gmail.com", UserName = "Operation123", Name = "Operation", SurName = "Operli" };
        //    await _userManager.CreateAsync(user, "Operation123");
        //    await _userManager.AddToRoleAsync(user, "Operation");


        //}

        //[HttpGet]
        //public async Task CreateNewAccount()
        //{
        //    await _roleManager.CreateAsync(new IdentityRole("Sale"));
        //    AppUser user = new AppUser { Email = "Sales@gmail.com", UserName = "Sales123", Name = "Sales", SurName = "Salesli" };
        //    await _userManager.CreateAsync(user, "Sales123");
        //    await _userManager.AddToRoleAsync(user, "Sale");


        //}

        //[HttpGet]
        //public async Task CreateNewAccount()
        //{
        //    await _roleManager.CreateAsync(new IdentityRole("Reservation"));
        //    AppUser user = new AppUser { Email = "Reservation@gmail.com", UserName = "Reservation123", Name = "Reservation", SurName = "Reservationli" };
        //    await _userManager.CreateAsync(user, "Reservation123");
        //    await _userManager.AddToRoleAsync(user, "Reservation");


        //}
    }
}
