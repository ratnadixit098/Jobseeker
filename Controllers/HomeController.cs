using Microsoft.AspNetCore.Mvc;

namespace Jobseeker.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
