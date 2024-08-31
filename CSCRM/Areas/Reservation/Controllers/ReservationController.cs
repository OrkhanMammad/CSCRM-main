using CSCRM.Abstractions;
using CSCRM.ViewModels.ReservationVMs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CSCRM.Areas.Reservation.Controllers
{
    [Area("Reservation")]
    [Authorize(Roles = "Reservation")]
    public class ReservationController : Controller
    {
        readonly IReservationService _service;
        public ReservationController(IReservationService service)
        {
                _service = service;
        }
        [HttpGet]
        public async Task<IActionResult> Index(int pageIndex = 1)
        {
            var result = await _service.GetHotelOrdersAsync(pageIndex);

            return View(result);
        }

        [HttpGet]
        public async Task<IActionResult> EditReservation(int reservationId)
        {
            var result = await _service.GetReservationForEditAsync(reservationId);
            return View(result);
        }


        [HttpPost]
        public async Task<IActionResult> EditReservation([FromBody] EditReservationDTO dto)
        {
            var result = await _service.EditReservationAsync(dto);
            return PartialView("_EditReservationPartialView",result);
        }
    }
}
