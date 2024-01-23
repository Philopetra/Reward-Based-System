using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RYT.Models.Entities;
using RYT.Models.ViewModels;
using System.Diagnostics.Eventing.Reader;

namespace RYT.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;


        public AccountController (UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
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
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if(ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                //if(user == null)
                //{
                //    ModelState.AddModelError("", "Email not found");
                //}
                if (user != null)
                {
                    if(await _userManager.IsEmailConfirmedAsync(user))
                        {
                        var loginResult = await _signInManager.PasswordSignInAsync(user, model.Password, false,false);
                        if (loginResult.Succeeded)
                           {
                              return RedirectToAction("Index", "Home");
                           }
                        else
                           {
                            ModelState.AddModelError("", "Email or Password is incorrect");
                           }
                        }
                    else
                    {
                        ModelState.AddModelError("", "Email is not yet Confirmed");
                    }
          
                }
                else
                {
                    ModelState.AddModelError("", "Invalid Credentials");
                }
            }
            return View(model);
        }
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index","Home");
        }

    }
}
