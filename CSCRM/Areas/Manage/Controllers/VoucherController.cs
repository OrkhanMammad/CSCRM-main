using Microsoft.AspNetCore.Mvc;

namespace CSCRM.Areas.Manage.Controllers
{
    public class VoucherController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
