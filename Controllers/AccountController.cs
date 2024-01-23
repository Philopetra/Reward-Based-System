using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RYT.Models.Entities;
using RYT.Models.ViewModels;
using RYT.Services.Emailing;



namespace RYT.Controllers
{
    public class AccountController : Controller
    {

        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IEmailService _emailService;

        public AccountController(UserManager<AppUser> userManager, IEmailService emailService,
            SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _emailService = emailService;
            _signInManager = signInManager;

        }

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

        [HttpPost]
        public IActionResult TeacherSignUpStep2()
        {
            return View();
        }

        [HttpGet]
        public IActionResult ResetPassword(string Email, String Token)
        {
            var resetPasswordModel = new ResetPasswordViewModel { Email = Email, Token = Token };
            return View(resetPasswordModel);
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {

              var user =   await _userManager.FindByEmailAsync(model.Email);
                if (user != null)
                {
                   var resetPasswordResult = await _userManager.ResetPasswordAsync(user, model.Token, model.NewPassword);
                    if (resetPasswordResult.Succeeded)
                    {
                        return RedirectToAction("Login", "Account");
                    }
                    else
                    {
                        foreach (var error in resetPasswordResult.Errors)
                        {
                            ModelState.AddModelError(error.Code, error.Description);
                        }
                        return View(model);
                    }
                }
                ModelState.AddModelError("", "Email Not Recognized");
            }
            return View();
        }

        public IActionResult Login()
        {
            return View();
        }
    }
}
