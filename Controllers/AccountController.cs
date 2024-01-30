using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RYT.Models.Entities;
using RYT.Models.Enums;
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


        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager,
            IEmailService emailService)
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

        [HttpPost]
        public async Task<IActionResult> SignUp(StudentSignUpViewModel model)
        {
            if (ModelState.IsValid)
            {
                //var user = _mapper.Map<AppUser>(model);
                var user = new AppUser
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Email = model.Email,
                    UserName = model.Email,
                };

                user.Wallet = new Wallet
                {
                    UserId = user.Id,
                    Balance = 0,
                    Status = WalletStatus.Active
                };

                var emailToCheck = await _userManager.FindByEmailAsync(model.Email);
                if (emailToCheck == null)
                {
                    var createUser = await _userManager.CreateAsync(user, model.Password);
                    if (createUser.Succeeded)
                    {
                        var addRole = await _userManager.AddToRoleAsync(user, "student");
                        if (addRole.Succeeded)
                        {
                            // send email confirmation link
                            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                            var link = Url.Action("ConfirmEmail", "Account", new { user.Email, token }, Request.Scheme);
                            var body = @$"Hi{user.FirstName},
Please click the link <a href='{link}'>here</a> to confirm your account's email";
                            await _emailService.SendEmailAsync(user.Email, "Confirm Email", body);

                            return RedirectToAction("RegisterCongrats", "Account", new { name = user.FirstName });
                        }

                        foreach (var err in addRole.Errors)
                        {
                            ModelState.AddModelError(err.Code, err.Description);

                        }
                    }
                    foreach (var err in createUser.Errors)
                    {
                        ModelState.AddModelError(err.Code, err.Description);
                    }
                }
                ModelState.AddModelError("", "email already exists");


            }
            return View(model);
        }

        [HttpGet]
        public IActionResult RegisterCongrats(string name)
        {
            ViewBag.Name = name;
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
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                //if(user == null)
                //{
                //    ModelState.AddModelError("", "Email not found");
                //}
                if (user != null)
                {
                    if (await _userManager.IsEmailConfirmedAsync(user))
                    {
                        var loginResult = await _signInManager.PasswordSignInAsync(user, model.Password, false, false);
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
            return RedirectToAction("Index", "Home");
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