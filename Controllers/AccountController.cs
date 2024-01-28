using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RYT.Data;
using RYT.Models.Entities;
using RYT.Models.ViewModels;
using RYT.Services.Emailing;
using System.Diagnostics.Eventing.Reader;
using System.Text.Json;
using RYT.Models.Enums;
using RYT.Services.Repositories;

namespace RYT.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IEmailService _emailService;
        private readonly IRepository _repository;

        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager,
            IEmailService emailService, IRepository repository)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailService = emailService;
            _repository = repository;
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

        [HttpPost]
        public async Task<IActionResult> TeacherSignUp1([FromForm] TeacherSignUpStep1ViewModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user is not null)
            {
                ModelState.AddModelError("Email", "Email already exists");

                return View("TeacherSignUp", model);
            }

            HttpContext.Session.SetString("TeacherSignUpStep1ViewModel", JsonSerializer.Serialize(model));

            return RedirectToAction("TeacherSignUpStep2");
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

        [HttpGet]
        public IActionResult TeacherSignUpStep2()
        {
            var model = new TeacherSignUpStep2ViewModel();
            model.ListOfSchoolTypes = SeedData.SchoolTypes;
            model.listOfSubjectsTaught = SeedData.Subjects;

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> TeacherSignUpStep2(TeacherSignUpStep2ViewModel model)
        {
            var step1ViewModel =
                JsonSerializer.Deserialize<TeacherSignUpStep1ViewModel>(
                    HttpContext.Session.GetString("TeacherSignUpStep1ViewModel"));
            HttpContext.Session.Clear();
            if (step1ViewModel is null)
            {
                return RedirectToAction("TeacherSignUp");
            }

            var user = new AppUser
            {
                FirstName = step1ViewModel.FirstName,
                LastName = step1ViewModel.LastName,
                Email = step1ViewModel.Email,
                UserName = step1ViewModel.Email,
                NameofSchool = step1ViewModel.SelectedSchool,
            };

            await _userManager.CreateAsync(user, step1ViewModel.Password);
            await _userManager.AddToRoleAsync(user, "teacher");

            var teacher = new Teacher
            {
                UserId = user.Id,
                YearsOfTeaching = model.YearsOfTeaching,
                SchoolType = model.SelectedSchoolType,
                NINUploadUrl = "", // added url returned from cloudinary
                NINUploadPublicId = "" // added public id returned from cloudinary
            };
            await _repository.AddAsync(teacher);

            var teacherSubject = new SubjectsTaught
            {
                TeacherId = teacher.Id,
                Subject = model.SelectedSubject
            };
            await _repository.AddAsync(teacherSubject);

            var teacherSchool = new SchoolsTaught
            {
                TeacherId = teacher.Id,
                School = step1ViewModel.SelectedSchool
            };
            await _repository.AddAsync(teacherSchool);

            var wallet = new Wallet
            {
                UserId = user.Id,
                Balance = 0,
                Status = WalletStatus.Active
            };
            await _repository.AddAsync(wallet);

            // confirm teacher email

            // redirect to confirm email view
            return RedirectToAction("Overview", "Dashboard");
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