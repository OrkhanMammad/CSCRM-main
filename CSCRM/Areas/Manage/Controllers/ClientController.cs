using CSCRM.Abstractions;
using CSCRM.Concretes;
using CSCRM.Models;
using CSCRM.ViewModels.ClientVMs;
using CSCRM.ViewModels.HotelVMs;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CSCRM.Areas.Manage.Controllers
{
    [Area("Manage")]
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
    }
}
