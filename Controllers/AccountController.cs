using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RYT.Data;
using RYT.Models.Entities;
using RYT.Models.ViewModels;
using RYT.Services.Emailing;
using System.Diagnostics.Eventing.Reader;

namespace RYT.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IEmailService _emailService;



        public AccountController (UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IEmailService emailService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailService = emailService;
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
        public async Task<IActionResult> ConfirmEmail(string Email, string token)
        {
            var user = await _userManager.FindByEmailAsync(Email);
            if (user != null)
            {
                var confirmEmailResult = await _userManager.ConfirmEmailAsync(user, token);
                if (confirmEmailResult.Succeeded)
                {
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("Index", "Home");
                }

                foreach (var err in confirmEmailResult.Errors)
                {
                    ModelState.AddModelError(err.Code, err.Description);
                }
                return View(ModelState);
            }

            ModelState.AddModelError("", "Email confirmation failed");

            return View(ModelState);

        }

        [HttpGet]
        public IActionResult TeacherSignUp()
        {
            var model = new TeacherSignUpStep1ViewModel();
            model.SchoolsTaught = SeedData.Schools;

            return View(model);
        }

        // To loadUp the Options in the SeedData
/*        [HttpGet]
        public IActionResult TeacherSignUpStep2(TeacherSignUpStep2GetViewModel model)
        {
            var step2ViewModel = new TeacherSignUpStep2GetViewModel
            {
                listOfSubjectsTaught = SeedData.Subjects,
                listOfSchoolTypes = SeedData.SchoolTypes
            };
            return View(step2ViewModel);
        }
*/
        [HttpPost]
        public IActionResult TeacherSignUpStep2(TeacherSignUpStep2ViewModel model)
        {
            var modelToDisplay = new TeacherSignUpStep2ViewModel();
            modelToDisplay.SchoolsTaught = SeedData.Schools;
            modelToDisplay.listOfSubjectsTaught = SeedData.Subjects;
            modelToDisplay.listOfSchoolTypes = SeedData.SchoolTypes;

            if (ModelState.IsValid)
            {

                var stepTwoViewModel = new TeacherSignUpStep2ViewModel
                {
                    Name = model.Name,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Email = model.Email,
                    Password = model.Password,
                    SelectedSchool = model.SelectedSchool,
                    SchoolsTaught = model.SchoolsTaught,
                    YearsOfTeaching = model.YearsOfTeaching,
                    CalculatedYearsOfTeaching= model.CalculatedYearsOfTeaching,
                    SelectedSchoolType= model.SelectedSchoolType,
                    SelectedSubject=model.SelectedSubject,
                    listOfSubjectsTaught = SeedData.Subjects,
                    listOfSchoolTypes = SeedData.SchoolTypes
                };
                return View(stepTwoViewModel);
            }
                return View(model);
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
                              return RedirectToAction("overview", "dashboard");
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

        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user != null)
                {
                    var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                    var link = Url.Action("ResetPassword", "Action", new { user.Email, token, Request.Scheme });
                    var body = @$"Hi{user.FirstName}{user.LastName},
						please, click the link <a href='{link}'>here</a> to reset your password";

                    await _emailService.SendEmailAsync(user.Email, "Forgot Password", body);

                    ViewBag.Message = "Password Reset details has been sent to your email";
                    return View();
                }
                ModelState.AddModelError("", "Invalid Email");
            }
            return View(model);
        }
    }
}
