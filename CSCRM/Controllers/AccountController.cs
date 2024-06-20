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
                if (roles.Any(r => r == "Developer"))
                {
                    return RedirectToAction("Index", "Hotel", new { area = "Manage" });
                }

                return View(signInVM);
            }
            catch (Exception ex) {

                return View(signInVM);

            }


               



        }
        
        //[HttpGet]
        //public async Task CreateNewAccount() 
        //{
        //    //await _roleManager.CreateAsync(new IdentityRole("Developer"));
        //    AppUser user = new AppUser { Email="Orkhanvm@gmail.com", UserName="Orkhan123", Name = "Orkhan", SurName="Mammadli" };
        //   await _userManager.CreateAsync(user,"Orkhan6991");
        //    await _userManager.AddToRoleAsync(user, "Developer");
        
        
        //}
    }
}
