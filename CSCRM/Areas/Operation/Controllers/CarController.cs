using CSCRM.Abstractions;
using CSCRM.Concretes;
using CSCRM.Models;
using CSCRM.ViewModels.CarVMs;
using CSCRM.ViewModels.CompanyVMs;
using CSCRM.ViewModels.TourVMs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CSCRM.Areas.Operation.Controllers
{
    [Area("Operation")]
    [Authorize(Roles = "Operation")]
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
        public async Task<IActionResult> Index(int pageIndex = 1)
        {
            var result = await _carService.GetAllCarsAsync(pageIndex);
            return View(result);
        }



    }
}
