using CSCRM.Abstractions;
using CSCRM.dataAccessLayers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CSCRM.ViewModels.TourCarVMs;
using CSCRM.Models;
using Microsoft.AspNetCore.Authorization;

namespace CSCRM.Areas.Sale.Controllers
{
    [Area("Sale")]
    [Authorize(Roles = "Sale")]

    public class TourCarController : Controller
    {
        readonly ITourByCarTypeService _service;
        readonly AppDbContext _context;
        public TourCarController(ITourByCarTypeService service, AppDbContext context)
        {
                _service = service;
            _context = context;
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
               var result = await _service.GetAllTrCrTypsAsync();
         
            return View(result);
        }

        [HttpGet]
        public async Task<IActionResult> EditTourCar(string tourName)
        {
            var result = await _service.GetTourCarForEditByTourName(tourName);
            return View(result);
        }

        [HttpPost]

        public async Task<IActionResult> EditTourCar([FromBody]EditTourCarVM editTourCarVM)
        {
            var result = await _service.EditTourCarAsync(editTourCarVM);
            return PartialView("_EditTourCarPartialView", result);
        }





    }

    
}
