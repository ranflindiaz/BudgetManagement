using Microsoft.AspNetCore.Mvc;

namespace BudgetManagement.Controllers
{
    public class UsersControllers : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
