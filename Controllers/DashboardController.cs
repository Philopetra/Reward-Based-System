using Microsoft.AspNetCore.Identity;
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
