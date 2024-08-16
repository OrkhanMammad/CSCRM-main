using CSCRM.Abstractions;
using CSCRM.Concretes;
using CSCRM.Models;
using CSCRM.ViewModels.ClientOrdersVM;
using CSCRM.ViewModels.ClientVMs;
using CSCRM.ViewModels.HotelVMs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CSCRM.Areas.Operation.Controllers
{
    [Area("Operation")]
    [Authorize(Roles = "Operation")]

    public class ClientController : Controller
    {
        readonly IClientService _clientService;
        readonly UserManager<AppUser> _userManager;
        public ClientController(IClientService clientService, UserManager<AppUser> userManager)
        {
            _clientService = clientService;
            _userManager = userManager;
        }
        [HttpGet]
        public async Task<IActionResult> Index(short pageIndex = 1)
        {
            var result = await _clientService.GetAllClientsAsync(pageIndex);
            return View(result);
        }


        [HttpGet]
        public async Task<IActionResult> GetClientOrders(int clientId)
        {
            var result = await _clientService.GetClientServicesAsync(clientId);
            return View(result);
        }


        [HttpPost]
        public async Task<IActionResult> DeleteTourOrderOfClient(int clientId, int tourOrderId)
        {
            AppUser appUser = await _userManager.Users.FirstOrDefaultAsync(u => u.UserName == User.Identity.Name);
            var result = await _clientService.DeleteTourOrderAsync(clientId, tourOrderId, appUser);
            return PartialView("_TourOrdersSectionPartialView", result);

        }

        [HttpPost]
        public async Task<IActionResult> AddNewTourOrder([FromBody] AddNewTourOrderVM newOrder)
        {
            AppUser appUser = await _userManager.Users.FirstOrDefaultAsync(u => u.UserName == User.Identity.Name);
            var result = await _clientService.AddNewTourOrderAsync(newOrder, appUser);
            return PartialView("_TourOrdersSectionPartialView", result);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteRestaurantOrderOfClient(int clientId, int restaurantOrderId)
        {
            AppUser appUser = await _userManager.Users.FirstOrDefaultAsync(u => u.UserName == User.Identity.Name);
            var result = await _clientService.DeleteRestaurantOrderAsync(clientId, restaurantOrderId, appUser);
            return PartialView("_RestaurantOrdersSectionPartialView", result);

        }

        [HttpPost]
        public async Task<IActionResult> AddNewRestaurantOrder([FromBody] AddNewRestaurantOrderVM newOrder)
        {
            AppUser appUser = await _userManager.Users.FirstOrDefaultAsync(u => u.UserName == User.Identity.Name);
            var result = await _clientService.AddNewRestaurantOrderAsync(newOrder, appUser);
            return PartialView("_RestaurantOrdersSectionPartialView", result);
        }



        [HttpPost]
        public async Task<IActionResult> GetClientByMailOrInvCode(string code)
        {
            var result = await _clientService.GetClientByMailOrInvCodeAsync(code);
            return PartialView("_ClientPartialView", result);
        }

        [HttpGet]
        public async Task<IActionResult> GetVoucherOfClient(int clientId)
        {
            var result = await _clientService.GetVoucherOfClientAsync(clientId);
            return View(result);
        }

    }
}
