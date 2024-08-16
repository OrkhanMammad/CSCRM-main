using CSCRM.Abstractions;
using CSCRM.Concretes;
using CSCRM.Models;
using CSCRM.ViewModels.HotelVMs;
using CSCRM.ViewModels.InclusivesVMs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CSCRM.Areas.Manage.Controllers
{
    [Area("Manage")]
    [Authorize(Roles = "Admin")]

    public class InclusiveController : Controller
    {
        readonly IIncludedService _includedService;
        readonly UserManager<AppUser> _userManager;
        public InclusiveController(IIncludedService includedService, UserManager<AppUser> userManager)
        {
                _includedService = includedService;
            _userManager = userManager;
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var result = await _includedService.GetAllInclusivesAsync();
            return View(result);
        }
        [HttpPost]

        public async Task<IActionResult> AddNewInclusive([FromBody]AddNewInclusiveVM inclusVm)
        {
            AppUser appUser = await _userManager.Users.FirstOrDefaultAsync(u => u.UserName == User.Identity.Name);
            var result = await _includedService.AddInclusiveAsync(inclusVm, appUser);
            return PartialView("_InclusivePartialView", result);

        }


        [HttpPost]
        public async Task<IActionResult> DeleteInclusive(int inclusiveId)
        {
            AppUser appUser = await _userManager.Users.FirstOrDefaultAsync(u => u.UserName == User.Identity.Name);

            var result = await _includedService.RemoveInclusiveAsync(inclusiveId, appUser);
            return PartialView("_InclusivePartialView", result);

        }


        [HttpGet]
        public async Task<IActionResult> EditInclusive(int inclusiveId)
        {
            var result = await _includedService.GetInclusiveByIdAsync(inclusiveId);
            return View(result);
        }

        [HttpPost]
        public async Task<IActionResult> EditInclusive([FromBody] EditInclusiveVM editInclusiveVM)
        {
            AppUser appUser = await _userManager.Users.FirstOrDefaultAsync(u => u.UserName == User.Identity.Name);

            var result = await _includedService.EditInclusiveAsync(editInclusiveVM, appUser);
            return PartialView("_EditInclusivePartialView", result);
        }


    }
}
