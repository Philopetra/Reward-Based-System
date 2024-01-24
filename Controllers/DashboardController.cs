using Microsoft.AspNetCore.Mvc;
using RYT.Commons;
using RYT.Data;
using RYT.Models.Entities;
using RYT.Models.ViewModels;
using RYT.Services.Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

        public IActionResult EditProfile() 
        {
            return View();
        }

        public IActionResult ChangePassword()
        {
            return View();  
        }


        public async Task<IActionResult> Teachers (TeacherListViewModel model, int page = 1 )
        {
            int pageSize = 7;

            //string schoolName
            //var listOfSchoolTaught = await _repository.GetAsync<SchoolsTaught>();
            //if(listOfSchoolTaught != null && listOfSchoolTaught.Any())
            //{
            //    var schoolTeachers = listOfSchoolTaught.Where(x => x.School == schoolName).ToList();
            //}

            // Hardcore some details for Teachers
            var teachers = new List<Teacher>
            {
                new Teacher
                {
                    User = new AppUser { FirstName = "John", LastName = "Doe" },
                    Position = "Head Teacher",
                    YearsOfTeaching = "15 years"
                },
                new Teacher
                {
                    User = new AppUser { FirstName = "Neo", LastName = "Smart" },
                    Position = "Head Teacher",
                    YearsOfTeaching = "5 years"
                },
                new Teacher
                {
                    User = new AppUser { FirstName = "Bite", LastName = "Man" },
                    Position = "Teacher",
                    YearsOfTeaching = "10 years"
                },
                new Teacher
                {
                    User = new AppUser { FirstName = "Favour", LastName = "Philopetra" },
                    Position = "Head Teacher",
                    YearsOfTeaching = "17 years"
                },
                new Teacher
                {
                    User = new AppUser { FirstName = "Vain", LastName = "GoldSmith" },
                    Position = "Teacher",
                    YearsOfTeaching = "10 years"
                },
                new Teacher
                {
                    User = new AppUser { FirstName = "Water", LastName = "Fire" },
                    Position = "Teacher",
                    YearsOfTeaching = "10 years"
                },
                new Teacher
                {
                    User = new AppUser { FirstName = "Pepper", LastName = "Rice" },
                    Position = "Teacher",
                    YearsOfTeaching = "10 years"
                },
                new Teacher
                {
                    User = new AppUser { FirstName = "Joy", LastName = "Ozo" },
                    Position = "Teacher",
                    YearsOfTeaching = "10 years"
                },
                new Teacher
                {
                    User = new AppUser { FirstName = "Joe", LastName = "Mark" },
                    Position = "Teacher",
                    YearsOfTeaching = "10 years"
                },
                new Teacher
                {
                    User = new AppUser { FirstName = "Babs", LastName = "Meme" },
                    Position = "Head Teacher",
                    YearsOfTeaching = "15 years"
                }
                
            };



            var query =  teachers.AsQueryable();

            //query = query.Where(t => t.SchoolId == model.SchoolId);
            //var query = await _repository.GetAsync<Teacher>();

            // Apply search and filter logic
            if (!string.IsNullOrEmpty(model.SearchKeyword))
            {
                string searchCriteria = model.SearchCriteria?.ToLower();

                switch (searchCriteria)
                {
                    case "all":
                        query = query.Where(t => (t.User.FirstName.Contains(model.SearchKeyword, StringComparison.OrdinalIgnoreCase) ||
                                                  t.User.LastName.Contains(model.SearchKeyword, StringComparison.OrdinalIgnoreCase) ||
                                                  (t.User.FirstName + " " + t.User.LastName).Contains(model.SearchKeyword, StringComparison.OrdinalIgnoreCase) ||
                                                  t.Position.Contains(model.SearchKeyword, StringComparison.OrdinalIgnoreCase) ||
                                                  t.YearsOfTeaching.Contains(model.SearchKeyword, StringComparison.OrdinalIgnoreCase)));

                        //query = query.Where(t => t.User.FirstName.Contains(model.SearchKeyword, StringComparison.OrdinalIgnoreCase) ||
                        //                          t.User.LastName.Contains(model.SearchKeyword, StringComparison.OrdinalIgnoreCase) ||
                        //                          t.User.FirstName.Contains(model.SearchKeyword, StringComparison.OrdinalIgnoreCase) &&
                        //                          t.User.LastName.Contains(model.SearchKeyword, StringComparison.OrdinalIgnoreCase) ||
                        //                          t.YearsOfTeaching.Contains(model.SearchKeyword, StringComparison.OrdinalIgnoreCase));
                        break;

                    case "name":
                        query = query.Where(t => (t.User.FirstName.Contains(model.SearchKeyword, StringComparison.OrdinalIgnoreCase) ||
                                                  t.User.LastName.Contains(model.SearchKeyword, StringComparison.OrdinalIgnoreCase) ||
                                                  (t.User.FirstName + " " + t.User.LastName).Contains(model.SearchKeyword, StringComparison.OrdinalIgnoreCase) ||
                                                  t.YearsOfTeaching.Contains(model.SearchKeyword, StringComparison.OrdinalIgnoreCase)));
                        break;

                    case "position":
                        query = query.Where(t => t.Position.Contains(model.SearchKeyword, StringComparison.OrdinalIgnoreCase));
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
