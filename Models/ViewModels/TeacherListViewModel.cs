using RYT.Models.Entities;
using System.Linq;

namespace RYT.Models.ViewModels
{
    public class TeacherListViewModel
    {
        public IQueryable<Teacher>? TeacherList { get; set; }
        public string SchoolName { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public int PageSize { get; set; } = 10;
        public int CurrentPage { get; set; } = 1;
        public int TotalPages { get; set; }
        public int Count { get; set; }


        // Additional properties for search and pagination
        public string SearchKeyword { get; set; } = string.Empty;
        public string SearchCriteria { get; set; } = "All";

    }
}