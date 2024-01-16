using Microsoft.AspNetCore.Mvc;

namespace RYT.Controllers
{
    public class StudentDashboardPopUpController : Controller
    {
        public IActionResult ProfileModal()
        {
            return View();
        }


        public IActionResult StudentDashboardPopUp()
        {
            return View();
        }
    }
}
