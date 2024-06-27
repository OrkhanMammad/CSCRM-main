using CSCRM.Abstractions;
using CSCRM.DataAccessLayers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CSCRM.ViewModels.TourCarVMs;
using CSCRM.Models;

namespace CSCRM.Areas.Manage.Controllers
{
    [Area("Manage")]
    public class TourCarController : Controller
    {
        readonly ITourByCarTypeService _service;
        readonly AppDbContext _context;
        public TourCarController(ITourByCarTypeService service, AppDbContext context)
        {
                _service = service;
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            return View();      
        }

        [HttpPost]
        public async Task<IActionResult> AddTourCarType([FromBody]AddTourCarVM tourCarVM)
        {
            var result = await _service.AddTrCrTypAsync(tourCarVM);
            return View("Index", result);
        }



    }

    
}
