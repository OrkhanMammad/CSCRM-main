using CSCRM.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace CSCRM.Areas.Reservation.Controllers
{
    [Area("Reservation")]
    public class ReservationController : Controller
    {
        readonly IReservationService _service;
        public ReservationController(IReservationService service)
        {
                _service = service;
        }
        public async Task<IActionResult> Index()
        {
            var result = await _service.GetHotelOrdersAsync();

            return View(result);
        }
    }
}
