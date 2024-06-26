using CSCRM.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace CSCRM.Areas.Manage.Controllers
{
    [Area("Manage")]
    public class TourCarController : Controller
    {
        readonly ITourByCarTypeService _service;
        public TourCarController(ITourByCarTypeService service)
        {
                _service = service;
        }
        public async Task<IActionResult> Index()
        {
            var result = await _service.GetAllTrCrTyps();
            var a = 10;
            return View();
        }
    }
}
