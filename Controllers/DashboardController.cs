using Microsoft.AspNetCore.Mvc;
using RYT.Commons;
using RYT.Data;
using RYT.Models.Entities;
using RYT.Models.ViewModels;
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
    }
}

