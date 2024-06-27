using CSCRM.Abstractions;
using CSCRM.Concretes;
using CSCRM.Models;
using CSCRM.ViewModels.CarVMs;
using CSCRM.ViewModels.CompanyVMs;
using CSCRM.ViewModels.TourVMs;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CSCRM.Areas.Manage.Controllers
{
    [Area("Manage")]
    public class CarController : Controller
    {
        readonly ICarService _carService;
        readonly UserManager<AppUser> _userManager;

        public CarController(ICarService carService, UserManager<AppUser> userManager)
        {
            _carService = carService;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var result = await _carService.GetAllCarsAsync();
            return View(result);
        }

        [HttpPost]
        public async Task<IActionResult> AddNewCar([FromBody] AddCarVM carVM)
        {
            AppUser appUser = await _userManager.Users.FirstOrDefaultAsync(u => u.UserName == User.Identity.Name);

            var result = await _carService.AddCarAsync(carVM, appUser);
            return PartialView("_CarPartialView", result);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteCar(int carId)
        {
            AppUser appUser = await _userManager.Users.FirstOrDefaultAsync(u => u.UserName == User.Identity.Name);

            var result = await _carService.RemoveCarAsync(carId, appUser);
            return PartialView("_CarPartialView", result);

        }



        [HttpGet]
        public async Task<IActionResult> EditCar(int carId)
        {
            var result = await _carService.GetCarByIdAsync(carId);
            return View(result);
        }

        [HttpPost]
        public async Task<IActionResult> EditCar([FromBody] EditCarVM editCarVM)
        {
            AppUser appUser = await _userManager.Users.FirstOrDefaultAsync(u => u.UserName == User.Identity.Name);

            var result = await _carService.EditCarAsync(editCarVM, appUser);
            return PartialView("_EditCarPartialView", result);
        }


    }
}
