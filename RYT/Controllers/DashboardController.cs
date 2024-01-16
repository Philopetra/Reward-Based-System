using Microsoft.AspNetCore.Mvc;

namespace RYT.Controllers
{
    public class DashboardController : Controller
    {
        public IActionResult Overview()
        {
            return View();
        }


        public IActionResult teacher_dashboard_messaging()
        {
            return View();
        }

        public IActionResult MessagingUnderStudentDashboard()

        {
            return View();
        }

        public IActionResult SettingLogoutAppreciationModal()
        {
            return View();
        }
        public IActionResult StudentDashboardListOfSchools()
        { 
            return View();
        }
    }
}
