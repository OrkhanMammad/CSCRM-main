using CSCRM.Abstractions;
using CSCRM.Concretes;
using CSCRM.Models;
using CSCRM.ViewModels.CompanyVMs;
using CSCRM.ViewModels.TourVMs;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CSCRM.Areas.Manage.Controllers
{
    [Area("Manage")]
    public class TourController : Controller
    {
        readonly ITourService _tourService;
        readonly UserManager<AppUser> _userManager;
        public TourController(ITourService tourService, UserManager<AppUser> userManager)
        {
              _tourService = tourService;
            _userManager = userManager;
        }



        [HttpGet]
        public async Task<IActionResult> Index(int pageIndex = 1)
        {
            

            var result = await _tourService.GetAllToursAsync(pageIndex);
            return View(result);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteTour(int tourId)
        {
            AppUser appUser =  await _userManager.Users.FirstOrDefaultAsync(u => u.UserName == User.Identity.Name);
            var result = await _tourService.RemoveTourAsync(tourId, appUser);
            return PartialView("_TourPartialView", result);
        }

        [HttpPost]
        public async Task<IActionResult> AddNewTour([FromBody] AddTourVM tourVM)
        {
            AppUser appUser = await _userManager.Users.FirstOrDefaultAsync(u => u.UserName == User.Identity.Name);

            var result = await _tourService.AddTourAsync(tourVM, appUser);
            return PartialView("_TourPartialView", result);
        }


        [HttpGet]
        public async Task<IActionResult> EditTour(int tourId)
        {
            

            var result = await _tourService.GetTourByIdAsync(tourId);
            return View(result);
        }

        [HttpPost]
        public async Task<IActionResult> EditTour([FromBody] EditTourVM tourVM)
        {
            AppUser appUser = await _userManager.Users.FirstOrDefaultAsync(u => u.UserName == User.Identity.Name);
            var result = await _tourService.EditTourAsync(tourVM, appUser);
            return PartialView("_EditTourPartialView", result);
        }

    }
}
