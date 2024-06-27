using CSCRM.Abstractions;
using CSCRM.Concretes;
using CSCRM.Models;
using CSCRM.Models.ResponseTypes;
using CSCRM.ViewModels.CompanyVMs;
using CSCRM.ViewModels.HotelVMs;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CSCRM.Areas.Manage.Controllers
{
    [Area("Manage")]
    public class CompanyController : Controller
    {
        readonly ICompanyService _companyService;
        readonly UserManager<AppUser> _userManager;

        public CompanyController(ICompanyService companyService, UserManager<AppUser> userManager)
        {
                _companyService = companyService;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var result = await _companyService.GetAllCompaniesAsync();
            return View(result);
        }

        [HttpPost]
        public async Task<IActionResult> AddNewCompany([FromBody]AddCompanyVM companyVM)
        {
            AppUser appUser = await _userManager.Users.FirstOrDefaultAsync(u => u.UserName == User.Identity.Name);

            var result = await _companyService.AddCompanyAsync(companyVM, appUser);
            return PartialView("_CompanyPartialView", result);
        }


        [HttpPost]
        public async Task<IActionResult> DeleteCompany(int companyId)
        {
            AppUser appUser = await _userManager.Users.FirstOrDefaultAsync(u => u.UserName == User.Identity.Name);

            var result = await _companyService.RemoveCompanyAsync(companyId, appUser);
            return PartialView("_CompanyPartialView", result);
            
        }

        [HttpGet]
        public async Task<IActionResult> EditCompany(int companyId)
        {
            var result = await _companyService.GetCompanyByIdAsync(companyId);
            return View(result);
        }

        [HttpPost]
        public async Task<IActionResult> EditCompany([FromBody] EditCompanyVM editCompanyVM)
        {
            AppUser appUser = await _userManager.Users.FirstOrDefaultAsync(u => u.UserName == User.Identity.Name);

            var result = await _companyService.EditCompanyAsync(editCompanyVM, appUser);
            return PartialView("_EditCompanyPartialView", result);
        }

    }

    


}
