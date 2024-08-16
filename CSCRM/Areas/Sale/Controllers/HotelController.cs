using CSCRM.Abstractions;
using CSCRM.Models;
using CSCRM.ViewModels.HotelVMs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CSCRM.Areas.Sale.Controllers
{


    [Area("Sale")]
    [Authorize(Roles = "Sale")]

    //[Authorize(Roles = "Developer")]
    public class HotelController : Controller
    {
        readonly IHotelService _hotelService;
        readonly UserManager<AppUser> _userManager;
        public HotelController(IHotelService hotelService, UserManager<AppUser> userManager)
        {
            _hotelService = hotelService;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> Index(short pageIndex=1)
        {

            var result = await _hotelService.GetAllHotelsAsync(pageIndex);           
                return View(result);
        }

        [HttpGet]
        public async Task<IActionResult> EditHotel(int hotelId)
        {
            var result = await _hotelService.GetHotelByIdAsync(hotelId);
            return View(result);
        }

        [HttpPost]
        public async Task<IActionResult> EditHotel([FromBody]EditHotelVM editHotelVM)
        {
            AppUser appUser = await _userManager.Users.FirstOrDefaultAsync(u => u.UserName == User.Identity.Name);

            var result = await _hotelService.EditHotelAsync(editHotelVM, appUser);
            return PartialView("_EditHotelPartialView",result);
        }


        [HttpPost]
        public async Task<IActionResult> AddNewHotel([FromBody] AddHotelVM addHotelVM)
        {
            AppUser appUser = await _userManager.Users.FirstOrDefaultAsync(u => u.UserName == User.Identity.Name);

            var result = await _hotelService.AddHotelAsync(addHotelVM, appUser);
            return PartialView("_HotelPartialView", result);


        }

        [HttpPost]
        public async Task<IActionResult> DeleteHotel(int hotelId)
        {
            AppUser appUser = await _userManager.Users.FirstOrDefaultAsync(u => u.UserName == User.Identity.Name);

            var result = await _hotelService.RemoveHotelAsync(hotelId, appUser);
                return PartialView("_HotelPartialView", result);
           
        }

       



    }
}
