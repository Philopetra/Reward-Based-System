using Microsoft.AspNetCore.Mvc;
using RYT.Services.Payment;

namespace RYT.Controllers
{
    public class DashboardController : Controller
    {
        private readonly IPaymentService _paymentService;

        public DashboardController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }
        public IActionResult Overview()
        {
            return View();
        }

        public IActionResult Messages()

        {
            return View();
        }
        [HttpGet]
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


        public IActionResult Transfer()
        {
            return View();
        }
    }

}
