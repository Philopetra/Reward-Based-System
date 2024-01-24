using Microsoft.AspNetCore.Mvc;
using RYT.Commons;
using RYT.Data;
using RYT.Models.Entities;
using RYT.Models.ViewModels;
using RYT.Services.Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace RYT.Controllers
{
    public class DashboardController : Controller
    {
        private readonly IRepository _repository;
        public DashboardController(IRepository repository)
        {
            _repository = repository;
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
        public async Task<IActionResult> EditProfile(string id) //pass in you VM
        {
            var user = await _repository.GetAsync<AppUser>(id);

            return View(user);


        }

        [HttpPost]
        public async Task<IActionResult> EditProfile(string id, EditProfileVM editProfileVM)
        {

            var user = await _repository.GetAsync<AppUser>(id);
            if (ModelState.IsValid)
            {
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
