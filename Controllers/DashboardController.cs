using Microsoft.AspNetCore.Mvc;
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
            const int pageSize = 5;

            IQueryable<string> schools = SeedData.Schools.AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                schools = schools.Where(s => s.IndexOf(searchString, StringComparison.OrdinalIgnoreCase) == 0);
                //schools = schools.Where(s => s.Contains(searchString));
                
            }

            int totalItems = schools.Count();
            int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

            List<string> schoolsOnPage = schools
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

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
    }

}
