using Microsoft.AspNetCore.Mvc;

namespace RYT.Controllers
{
    public class DashboardController : Controller
    {
        public IActionResult Overview()
        {
            return View();
        }
        public IActionResult MessagingUnderStudentDashboard()
        {
            return View();
        }
    }
}
