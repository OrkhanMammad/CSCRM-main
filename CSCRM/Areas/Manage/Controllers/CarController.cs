using CSCRM.Abstractions;
using CSCRM.Concretes;
using CSCRM.ViewModels.CarVMs;
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



    }
}
