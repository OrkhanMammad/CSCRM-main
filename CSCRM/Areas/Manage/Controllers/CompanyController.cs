using Microsoft.AspNetCore.Mvc;

namespace CSCRM.Areas.Manage.Controllers
{
    public class CompanyController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
