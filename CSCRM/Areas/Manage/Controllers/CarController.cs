using CSCRM.Abstractions;
using CSCRM.Concretes;
using CSCRM.ViewModels.CarVMs;
using CSCRM.ViewModels.CompanyVMs;
using CSCRM.ViewModels.TourVMs;
using Microsoft.AspNetCore.Mvc;

namespace CSCRM.Areas.Manage.Controllers
{
    [Area("Manage")]
    public class CarController : Controller
    {
        readonly ICarService _carService;

        public CarController(ICarService carService)
        {
            _carService = carService; 
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
            var result = await _carService.AddCarAsync(carVM);
            return PartialView("_CarPartialView", result);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteCar(int carId)
        {
            var result = await _carService.RemoveCarAsync(carId);
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
            var result = await _carService.EditCarAsync(editCarVM);
            return PartialView("_EditCarPartialView", result);
        }


    }
}
