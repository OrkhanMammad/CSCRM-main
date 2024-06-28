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
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var tours = await _context.TourByCarTypes
        .Include(t => t.Tour)
        .Include(t => t.CarType)
        .ToListAsync();
            var a = 100;
            return View();      
        }

        //[HttpPost]
        //public async Task<IActionResult> AddTourCarType([FromBody]AddTourCarVM tourCarVM)
        //{
        //    var result = await _service.AddTrCrTypAsync(tourCarVM);
        //    return View("Index", result);
        //}



    }

    
}
