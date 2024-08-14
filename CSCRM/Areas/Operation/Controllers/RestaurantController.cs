using CSCRM.Abstractions;
using CSCRM.Models;
using CSCRM.ViewModels.RestaurantVMs;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CSCRM.Areas.Operation.Controllers
{
    [Area("Operation")]
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

    }

}
