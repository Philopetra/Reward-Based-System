using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RYT.Data;
using RYT.Models.Entities;
using RYT.Models.ViewModels;
using RYT.Services.Repositories;

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


        public async Task<IActionResult> Teachers (TeacherListViewModel model, int page = 1)
        {
            int pageSize = 7;
            

            // Hardcore some details for Teachers
            var teachers = new List<Teacher>
            {
                new Teacher
                {
                    User = new AppUser { FirstName = "John", LastName = "Doe" },
                    YearsOfTeaching = "15 years"
                },
                new Teacher
                {
                    User = new AppUser { FirstName = "Jane", LastName = "Smith" },
                    YearsOfTeaching = "10 years"
                },
                new Teacher
                {
                    User = new AppUser { FirstName = "Jane", LastName = "Smith" },
                    YearsOfTeaching = "10 years"
                },
                new Teacher
                {
                    User = new AppUser { FirstName = "Jane", LastName = "Smith" },
                    YearsOfTeaching = "10 years"
                },
                new Teacher
                {
                    User = new AppUser { FirstName = "Jane", LastName = "Smith" },
                    YearsOfTeaching = "10 years"
                },
                new Teacher
                {
                    User = new AppUser { FirstName = "Jane", LastName = "Smith" },
                    YearsOfTeaching = "10 years"
                },
                new Teacher
                {
                    User = new AppUser { FirstName = "Jane", LastName = "Smith" },
                    YearsOfTeaching = "10 years"
                },
                new Teacher
                {
                    User = new AppUser { FirstName = "Jane", LastName = "Smith" },
                    YearsOfTeaching = "10 years"
                },
                new Teacher
                {
                    User = new AppUser { FirstName = "Jane", LastName = "Smith" },
                    YearsOfTeaching = "10 years"
                },
                new Teacher
                {
                    User = new AppUser { FirstName = "Jane", LastName = "Smith" },
                    YearsOfTeaching = "10 years"
                }
                
            };



            var query =  teachers.AsQueryable();

            //var query = await _repository.GetAsync<Teacher>();

            // Apply search and filter logic
            if (!string.IsNullOrEmpty(model.SearchKeyword))
            {
                string searchCriteria = model.SearchCriteria?.ToLower();

                switch (searchCriteria)
                {
                    case "all":
                        query = query.Where(t => t.User.FirstName.Contains(model.SearchKeyword, StringComparison.OrdinalIgnoreCase) ||
                                                  t.User.LastName.Contains(model.SearchKeyword, StringComparison.OrdinalIgnoreCase) ||
                                                  t.YearsOfTeaching.Contains(model.SearchKeyword, StringComparison.OrdinalIgnoreCase));
                        break;
                    case "name":
                        query = query.Where(t => t.User.FirstName.Contains(model.SearchKeyword, StringComparison.OrdinalIgnoreCase) ||
                                                  t.User.LastName.Contains(model.SearchKeyword, StringComparison.OrdinalIgnoreCase));
                        break;
                    case "period":
                        query = query.Where(t => t.YearsOfTeaching.Contains(model.SearchKeyword, StringComparison.OrdinalIgnoreCase));
                        break;
                }
            }

            // Paginatination logic
            var paginatedTeachers = query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            model.TeacherList = paginatedTeachers.AsQueryable();
            model.CurrentPage = page;
            model.Count = query.Count();
            model.TotalPages = (int)Math.Ceiling(model.Count / (double)pageSize);

            return View(model);
        }
    


        public IActionResult Transfer()
        {
            return View();
        }
    }

}
