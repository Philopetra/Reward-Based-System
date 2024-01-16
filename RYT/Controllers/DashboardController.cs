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

        public IActionResult AccountCreatedSuccessfullyModalUnderStudentDashboard()
        {
            return View();
        }

        public IActionResult NotificationModalUnderStudentDashboard()
        {
            return View();
        }


    }
}
