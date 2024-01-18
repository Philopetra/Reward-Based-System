using Microsoft.AspNetCore.Mvc;

namespace RYT.Controllers
{
    public class DashboardController : Controller
    {
        public IActionResult Overview()
        {
            return View();
        }

        public IActionResult Messages()

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

        public IActionResult ChangePassword()
        {
            return View();  
        }

        public IActionResult Teachers()
        {
            return View();
        }

        public IActionResult StudentDashboardTransfer()
        {
            return View();
        }
    }

}
