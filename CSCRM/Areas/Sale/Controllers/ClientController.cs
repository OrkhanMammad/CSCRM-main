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

namespace CSCRM.Areas.Sale.Controllers
{
    [Area("Sale")]
    [Authorize(Roles = "Sale")]

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

        [HttpPost]

        public async Task<IActionResult> AddClient([FromBody] AddClientVM clientVM)
        {

            AppUser appUser = await _userManager.Users.FirstOrDefaultAsync(u => u.UserName == User.Identity.Name);
            var result = await _clientService.AddClientAsync(clientVM, appUser);
            return PartialView("_ClientPartialView", result);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteClient(int clientId)
        {
            AppUser appUser = await _userManager.Users.FirstOrDefaultAsync(u => u.UserName == User.Identity.Name);
            var result = await _clientService.DeleteClientAsync(clientId, appUser);
            return PartialView("_ClientPartialView", result);

        }

        [HttpGet]
        public async Task<IActionResult> EditClientInfo(int clientId)
        {
            var result = await _clientService.GetClientForEditInfo(clientId);
            return View(result);
        }

        [HttpPost]

        public async Task<IActionResult> EditClientInfo([FromBody] EditClientInfoVM clientInfoVM)
        {
            AppUser appUser = await _userManager.Users.FirstOrDefaultAsync(u => u.UserName == User.Identity.Name);
            var result = await _clientService.EditClientInfoAsync(clientInfoVM, appUser);
            return PartialView("_EditClientInfoPartialView", result);
        }

        [HttpGet]
        public async Task<IActionResult> GetClientOrders(int clientId)
        {
            var result = await _clientService.GetClientServicesAsync(clientId);
            return View(result);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteHotelOrderOfClient(int clientId, int hotelOrderId)
        {
            AppUser appUser = await _userManager.Users.FirstOrDefaultAsync(u => u.UserName == User.Identity.Name);
            var result = await _clientService.DeleteHotelOrderAsync(clientId, hotelOrderId, appUser);
            return PartialView("_HotelOrdersSectionPartialView", result);

        }

        [HttpPost]
        public async Task<IActionResult> AddNewHotelOrder([FromBody] AddNewHotelOrderVM newOrder)
        {
            AppUser appUser = await _userManager.Users.FirstOrDefaultAsync(u => u.UserName == User.Identity.Name);
            var result = await _clientService.AddNewHotelOrderAsync(newOrder, appUser);
            return PartialView("_HotelOrdersSectionPartialView", result);
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

        //[HttpPost]
        //public async Task<IActionResult> DeleteRestaurantOrderOfClient(int clientId, int restaurantOrderId)
        //{
        //    AppUser appUser = await _userManager.Users.FirstOrDefaultAsync(u => u.UserName == User.Identity.Name);
        //    var result = await _clientService.DeleteRestaurantOrderAsync(clientId, restaurantOrderId, appUser);
        //    return PartialView("_RestaurantOrdersSectionPartialView", result);

        //}

        //[HttpPost]
        //public async Task<IActionResult> AddNewRestaurantOrder([FromBody] AddNewRestaurantOrderVM newOrder)
        //{
        //    AppUser appUser = await _userManager.Users.FirstOrDefaultAsync(u => u.UserName == User.Identity.Name);
        //    var result = await _clientService.AddNewRestaurantOrderAsync(newOrder, appUser);
        //    return PartialView("_RestaurantOrdersSectionPartialView", result);
        //}

        [HttpPost]
        public async Task<IActionResult> DeleteInclusiveOrderOfClient(int clientId, int inclusiveOrderId)
        {
            AppUser appUser = await _userManager.Users.FirstOrDefaultAsync(u => u.UserName == User.Identity.Name);
            var result = await _clientService.DeleteInclusiveOrderAsync(clientId, inclusiveOrderId, appUser);
            return PartialView("_InclusiveOrdersSectionPartialView", result);

        }

        [HttpPost]
        public async Task<IActionResult> AddNewInclusiveOrder([FromBody] AddNewInclusiveOrderVM newOrder)
        {
            AppUser appUser = await _userManager.Users.FirstOrDefaultAsync(u => u.UserName == User.Identity.Name);
            var result = await _clientService.AddNewInclusiveOrderAsync(newOrder, appUser);
            return PartialView("_InclusiveOrdersSectionPartialView", result);
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
