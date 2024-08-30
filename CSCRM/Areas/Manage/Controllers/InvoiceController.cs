using CSCRM.Abstractions;
using CSCRM.dataAccessLayers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CSCRM.Areas.Manage.Controllers
{
    [Area("Manage")]
    [Authorize(Roles = "Admin")]
    public class InvoiceController : Controller
    {
        readonly IInvoiceService _service;
        public InvoiceController(IInvoiceService service)
        {
            _service = service;  
        }
        [HttpGet]
        public async Task<IActionResult>Index(int clientId)
        {
            var result = await _service.GetInvoiceAsync(clientId);
            return View(result);
        }
    }
}
