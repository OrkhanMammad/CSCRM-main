﻿using CSCRM.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CSCRM.Areas.Sale.Controllers
{
    [Area("Sale")]
    [Authorize(Roles = "Sale")]

    public class ConfirmationController : Controller
    {
        readonly IClientService _service;
        public ConfirmationController(IClientService service)
        {
            _service = service;   
        }
        [HttpGet]
        public async Task<IActionResult> Index(short pageIndex = 1)
        {
            var result = await _service.GetConfirmationAsync(pageIndex);
            return View(result);
        }
    }
}
