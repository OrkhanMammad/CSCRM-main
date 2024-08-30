using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CSCRM.ViewModels.AccountVMs;
using CSCRM.Models;
using Microsoft.AspNetCore.Identity;
using CSCRM.dataAccessLayers;

namespace CSCRM.Areas.Manage.Controllers
{
    [Area("Manage")]
    [Authorize(Roles = "Admin")]

    public class UserController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<AppUser> _signInManager;
        public UserController(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager, SignInManager<AppUser> signInManager, AppDbContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _context = context;

        }

        //[HttpGet]
        //public async Task<IActionResult> Index()
        //{

        //}


        [HttpGet]
        public IActionResult CreateNewUser()
        {
            return View(new CreateUserDTO());
        }

        [HttpPost]
        public async Task<IActionResult> CreateNewUser(CreateUserDTO dto)
        {            
                AppUser user = new AppUser { UserName = dto.NewUsername, Name = dto.NewName, SurName = dto.NewSurname };
               IdentityResult identityResult = await _userManager.CreateAsync(user, dto.NewPassword);
            if (identityResult.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, dto.Role);
                return RedirectToAction("Index", "Hotel");
            }
            else
            {
                return RedirectToAction("CreateNewUser", "User");
            }
                           
        }




    }
}
