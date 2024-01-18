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

        public IActionResult Messages()

        {
            return View();
        }

        public IActionResult SettingLogoutAppreciationModal()
        {
            return View();
        }
        public IActionResult SendReward()
        { 
            return View();
        }
        
        public IActionResult EditProfile() 
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

        public IActionResult ChangePassword()
        {
            return View();  
        }

        public IActionResult Teachers()
        {
            return View();
        }

    }

}
