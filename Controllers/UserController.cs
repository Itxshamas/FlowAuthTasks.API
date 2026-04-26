using Microsoft.AspNetCore.Mvc;

namespace FlowAuthTasks.API.Controllers
{
    public class UserController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
