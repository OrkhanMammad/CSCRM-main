﻿using CSCRM.Abstractions;
using CSCRM.ViewModels.HotelVMs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CSCRM.Areas.Manage.Controllers
{


    [Area("Manage")]
    //[Authorize(Roles = "Developer")]
    public class HotelController : Controller
    {
        readonly IHotelService _hotelService;
        public HotelController(IHotelService hotelService)
        {
            _hotelService = hotelService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {

            var result = await _hotelService.GetAllHotelsAsync();           
                return View(result);
        }

        [HttpGet]
        public async Task<IActionResult> EditHotel(int hotelId)//int hotelId)
        {
            var result = await _hotelService.GetHotelByIdAsync(hotelId);
            return View(result);
        }

        [HttpPost]
        public async Task<IActionResult> EditHotel([FromBody]EditHotelVM editHotelVM)
        {
            var result = await _hotelService.EditHotelAsync(editHotelVM);
            return PartialView("_EditHotelPartialView",result);
        }


        [HttpPost]
        public async Task<IActionResult> AddNewHotel([FromBody] AddHotelVM addHotelVM)
        {
            var result = await _hotelService.AddHotelAsync(addHotelVM);
            return PartialView("_HotelPartialView", result);


        }

        [HttpPost]
        public async Task<IActionResult> DeleteHotel(int hotelId)
        {
            var result = await _hotelService.RemoveHotelAsync(hotelId);
                return PartialView("_HotelPartialView", result);
           
        }

       



    }
}
