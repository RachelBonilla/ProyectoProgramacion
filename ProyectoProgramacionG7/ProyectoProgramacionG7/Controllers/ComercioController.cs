using Microsoft.AspNetCore.Mvc;

namespace ProyectoProgramacionG7.Controllers
{
    public class HomeController1 : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
