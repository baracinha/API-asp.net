using Microsoft.AspNetCore.Mvc;

namespace hotel.Controllers
{
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
