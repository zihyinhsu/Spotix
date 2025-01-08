using Microsoft.AspNetCore.Mvc;

namespace Spotix.MVC.Controllers
{
    public class PlaceController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
