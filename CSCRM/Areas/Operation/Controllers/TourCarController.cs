using CSCRM.Abstractions;
using CSCRM.dataAccessLayers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CSCRM.ViewModels.TourCarVMs;
using CSCRM.Models;
using Microsoft.AspNetCore.Authorization;

namespace CSCRM.Areas.Operation.Controllers
{
    [Area("Operation")]
    [Authorize(Roles = "Operation")]

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






    }

    
}
