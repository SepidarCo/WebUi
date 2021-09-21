using Microsoft.AspNetCore.Mvc;

namespace Sepidar.Admin.Controllers
{
    public class HomeController : AdminController
    {
        [HttpGet]
        public IActionResult List()
        {
            return View();
        }
    }
}
