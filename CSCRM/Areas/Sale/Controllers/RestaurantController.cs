using CSCRM.Abstractions;
using CSCRM.Models;
using CSCRM.ViewModels.RestaurantVMs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CSCRM.Areas.Sale.Controllers
{
    [Area("Sale")]
    [Authorize(Roles = "Sale")]

    //[Authorize(Roles = "Developer")]
    public class RestaurantController : Controller
    {
        readonly IRestaurantService _restaurantService;
        readonly UserManager<AppUser> _userManager;
        public RestaurantController(IRestaurantService restaurantService, UserManager<AppUser> userManager)
        {
            _restaurantService = restaurantService;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> Index(short pageIndex = 1)
        {
            var result = await _restaurantService.GetAllRestaurantsAsync(pageIndex);
            return View(result);
        }

        [HttpGet]
        public async Task<IActionResult> EditRestaurant(int restaurantId)
        {
            var result = await _restaurantService.GetRestaurantByIdAsync(restaurantId);
            return View(result);
        }

        [HttpPost]
        public async Task<IActionResult> EditRestaurant([FromBody] EditRestaurantVM editRestaurantVM)
        {
            AppUser appUser = await _userManager.Users.FirstOrDefaultAsync(u => u.UserName == User.Identity.Name);

            var result = await _restaurantService.EditRestaurantAsync(editRestaurantVM, appUser);
            return PartialView("_EditRestaurantPartialView", result);
        }

        [HttpPost]
        public async Task<IActionResult> AddNewRestaurant([FromBody] AddRestaurantVM addRestaurantVM)
        {
            AppUser appUser = await _userManager.Users.FirstOrDefaultAsync(u => u.UserName == User.Identity.Name);

            var result = await _restaurantService.AddRestaurantAsync(addRestaurantVM, appUser);
            return PartialView("_RestaurantPartialView", result);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteRestaurant(int restaurantId)
        {
            AppUser appUser = await _userManager.Users.FirstOrDefaultAsync(u => u.UserName == User.Identity.Name);

            var result = await _restaurantService.RemoveRestaurantAsync(restaurantId, appUser);
            return PartialView("_RestaurantPartialView", result);
        }
    }

}
