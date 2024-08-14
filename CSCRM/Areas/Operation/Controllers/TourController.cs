using CSCRM.Abstractions;
using CSCRM.Concretes;
using CSCRM.Models;
using CSCRM.ViewModels.CompanyVMs;
using CSCRM.ViewModels.TourVMs;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CSCRM.Areas.Operation.Controllers
{
    [Area("Operation")]
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


    }
}
