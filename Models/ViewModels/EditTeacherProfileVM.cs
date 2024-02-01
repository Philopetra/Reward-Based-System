using Microsoft.AspNetCore.Mvc;
using RYT.Data;
using RYT.Models.Entities;
using System.ComponentModel.DataAnnotations;

namespace RYT.Models.ViewModels
{
    public class EditTeacherProfileVM
    {
        [Required]
        public string TeacherId { get; set; } = "";

        public string? UserId { get; set; }

        [Required]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        public string LastName { get; set; } = string.Empty;

        [Required]
        public string Email { get; set; } = string.Empty;

        public string? PhoneNumber { get; set; }

        [Required]
        public string YearsOfTeaching { get; set; } = string.Empty;

        [Required]
        public string SchoolType { get; set; } = string.Empty;

        public List<string> TeachersSubject { get; set; } = SeedData.Subjects.ToList();
        public List<string> TeachersSchool { get; set; } = SeedData.Schools.ToList();

        [Required]
        public string SelectedSchool { get; set;} = string.Empty;

        [Required]
        public string SelectedSubject { get; set;} = string.Empty;
    }
}
