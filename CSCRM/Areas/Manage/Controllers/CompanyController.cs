using CSCRM.Abstractions;
using CSCRM.Concretes;
using CSCRM.Models.ResponseTypes;
using CSCRM.ViewModels.CompanyVMs;
using CSCRM.ViewModels.HotelVMs;
using Microsoft.AspNetCore.Mvc;

namespace CSCRM.Areas.Manage.Controllers
{
    [Area("Manage")]
    public class CompanyController : Controller
    {
        readonly ICompanyService _companyService;

        public CompanyController(ICompanyService companyService)
        {
                _companyService = companyService;
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
            var result = await _companyService.AddCompanyAsync(companyVM);
            return PartialView("_CompanyPartialView", result);
        }


        [HttpPost]
        public async Task<IActionResult> DeleteCompany(int companyId)
        {
            var result = await _companyService.RemoveCompanyAsync(companyId);
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
            var result = await _companyService.EditCompanyAsync(editCompanyVM);
            return PartialView("_EditCompanyPartialView", result);
        }




    }

    


}
