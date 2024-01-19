using Microsoft.AspNetCore.Mvc;

namespace RYT.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult SignUp()
        {
            return View();
        }

        [HttpGet]
        public IActionResult TeacherSignUp()
        {
            return View();
        }

        [HttpGet]
        public IActionResult TeacherSignUpStep2()
        {
            return View();
        }
        public IActionResult TeacherLogin()
        {
            return View();
        }
    }
}
