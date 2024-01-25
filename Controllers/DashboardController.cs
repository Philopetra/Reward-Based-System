﻿using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RYT.Commons;
using RYT.Data;
using RYT.Models.Entities;
using RYT.Models.ViewModels;
using RYT.Services.CloudinaryService;
using RYT.Services.Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;


namespace RYT.Controllers
{
    public class DashboardController : Controller
    {
        private readonly IRepository _repository;
        private readonly IPhotoService _photoService;
        private readonly UserManager<AppUser> _userManager;

        public DashboardController(IRepository repository, UserManager<AppUser> userManager, IPhotoService photoService)
        {
            _repository = repository;
            _userManager = userManager;
            _photoService = photoService;
        }

        public IActionResult Overview()
        {
            return View();
        }

        public IActionResult Messages()

        {
            return View();
        }

        public IActionResult SendReward(ListOfSchoolViewModel model, string searchString, int page = 1)
        {
            int pageSize = 5;

            IQueryable<string> schools = SeedData.Schools.AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                schools = schools.Where(s => s.IndexOf(searchString, StringComparison.OrdinalIgnoreCase) == 0);
            }
            List<string> schoolsOnPage;
            int totalItems, totalPages;

            Pagination.UsePagination(schools, page, pageSize, out schoolsOnPage, out totalItems, out totalPages);

            model.Schools = schoolsOnPage;
            model.CurrentPage = page;
            model.TotalPages = totalPages;
            model.Count = totalItems;
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> EditProfile() //pass in you VM
        {
            var user = await _userManager.GetUserAsync(User); ;

            var editProfileViewModel = new EditProfileVM()
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email   = user.Email,
                PhoneNumber = user.PhoneNumber,
                NameofSchool = user.NameofSchool
            };

            return View(editProfileViewModel);


        }

        [HttpPost]
        public async Task<IActionResult> EditProfile(EditProfileVM editProfileVM)
        public IActionResult EditProfile()
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                user.FirstName = editProfileVM.FirstName;
                user.LastName = editProfileVM.LastName;
                user.Email = editProfileVM.Email;
                user.PhoneNumber = editProfileVM.PhoneNumber;
                user.NameofSchool = editProfileVM.NameofSchool;

                await _repository.UpdateAsync<AppUser>(user);

            }
            return RedirectToAction("Overview");
        }

        public IActionResult ChangePassword()
        {
            return View();
        }

        public async Task<IActionResult> Teachers(TeacherListViewModel model, string schoolName, int page = 1)
        {
            int pageSize = 5;

            var listOfSchoolTaught = await _repository.GetAsync<SchoolsTaught>();
            if (listOfSchoolTaught != null && listOfSchoolTaught.Any())
            {
                var schoolTeachers = listOfSchoolTaught
                    .Where(x => x.School == schoolName)
                    .Select(x => x.Teacher)
                    .AsQueryable();

                // Apply search and filter logic
                if (!string.IsNullOrEmpty(model.SearchKeyword))
                {
                    string searchCriteria = model.SearchCriteria?.ToLower();
                    switch (searchCriteria)
                    {
                        case "all":
                            schoolTeachers = schoolTeachers.Where(t =>
                                (t.User.FirstName.Contains(model.SearchKeyword, StringComparison.OrdinalIgnoreCase) ||
                                t.User.LastName.Contains(model.SearchKeyword, StringComparison.OrdinalIgnoreCase) ||
                                (t.User.FirstName + " " + t.User.LastName).Contains(model.SearchKeyword, StringComparison.OrdinalIgnoreCase) ||
                                t.Position.Contains(model.SearchKeyword, StringComparison.OrdinalIgnoreCase) ||
                                t.YearsOfTeaching.Contains(model.SearchKeyword, StringComparison.OrdinalIgnoreCase)));
                            break;
                        case "name":
                            schoolTeachers = schoolTeachers.Where(t =>
                                (t.User.FirstName.Contains(model.SearchKeyword, StringComparison.OrdinalIgnoreCase) ||
                                t.User.LastName.Contains(model.SearchKeyword, StringComparison.OrdinalIgnoreCase) ||
                                (t.User.FirstName + " " + t.User.LastName).Contains(model.SearchKeyword, StringComparison.OrdinalIgnoreCase) ||
                                t.YearsOfTeaching.Contains(model.SearchKeyword, StringComparison.OrdinalIgnoreCase)));
                            break;
                        case "position":
                            schoolTeachers = schoolTeachers.Where(t => t.Position.Contains(model.SearchKeyword, StringComparison.OrdinalIgnoreCase));
                            break;
                        case "period":
                            schoolTeachers = schoolTeachers.Where(t => t.YearsOfTeaching.Contains(model.SearchKeyword, StringComparison.OrdinalIgnoreCase));
                            break;
                    }
                }

                List<Teacher> paginatedTeachers;
                int totalItems, totalPages;

                Pagination.UsePagination(schoolTeachers, page, pageSize, out paginatedTeachers, out totalItems, out totalPages);

                model.TeacherList = paginatedTeachers.AsQueryable();
                model.CurrentPage = page;
                model.Count = totalItems;
                model.TotalPages = totalPages;

                model.SchoolName = schoolName; // Set the school name in the model

                return View(model);
            }

            return View(model);
        }

        public IActionResult Transfer()
        {
            return View();
        }

        public IActionResult UpdateImage()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> UpdateImage(UploadImageVM model)
        {
            if (ModelState.IsValid)
            {
                //To get User
                var user = await _userManager.GetUserAsync(User);

                //To go to the database and get the AppUser properties so that we can confirm if the picture is  if (user.PhotoUrl == null)
                if (user != null)
                {
                    Dictionary<string, string> cloudinaryResponse = await _photoService.UploadImage(model.Image, $"{user.LastName} {user.FirstName}");
                    if (cloudinaryResponse["Code"] == "200")
                    {
                        string photoUrl = cloudinaryResponse["Url"];
                        string publicId = cloudinaryResponse["PublicId"];
                        user.PhotoUrl = photoUrl;
                        user.PublicId = publicId;
                        var result = await _userManager.UpdateAsync(user);

                        if (result.Succeeded)
                        {
                            return RedirectToAction("UpdateImage");
                        }
                        else
                        {
                            foreach (var err in result.Errors)
                            {
                                ModelState.AddModelError(err.Code, err.Description);
                            }
                        }
                    }

                    ModelState.AddModelError("", cloudinaryResponse["Message"]);
                }
            }
            return View();


        }
    }
}

