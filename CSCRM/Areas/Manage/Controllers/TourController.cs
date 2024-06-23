using CSCRM.Abstractions;
using CSCRM.Concretes;
using CSCRM.ViewModels.CompanyVMs;
using CSCRM.ViewModels.TourVMs;
using Microsoft.AspNetCore.Mvc;

namespace CSCRM.Areas.Manage.Controllers
{
    [Area("Manage")]
    public class TourController : Controller
    {
        readonly ITourService _tourService;
        public TourController(ITourService tourService)
        {
              _tourService = tourService;
        }



        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var result = await _tourService.GetAllToursAsync();
            return View(result);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteTour(int tourId)
        {
            var result = await _tourService.RemoveTourAsync(tourId);
            return PartialView("_TourPartialView", result);

        }

        [HttpPost]
        public async Task<IActionResult> AddNewTour([FromBody] AddTourVM tourVM)
        {
            var result = await _tourService.AddTourAsync(tourVM);
            return PartialView("_TourPartialView", result);
        }

    }
}
