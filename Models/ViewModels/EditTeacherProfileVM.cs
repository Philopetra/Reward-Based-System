using RYT.Models.Entities;

namespace RYT.Models.ViewModels
{
    public class EditTeacherProfileVM
    {
        public string UserId { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string YearsOfTeaching { get; set; } = string.Empty;
        public IFormFile photo { get; set; }
        public string FolderName { get; set; }
        public string SchoolType { get; set; } = string.Empty;
        public string NINUploadUrl { get; set; } = string.Empty;
        public string NINUploadPublicId { get; set; } = string.Empty;
        public List<SubjectsTaught> TeachersSubject { get; set; } = new List<SubjectsTaught>();
        public List<SchoolsTaught> TeachersSchool { get; set; } = new List<SchoolsTaught>();
        public string PhotoUrl { get; set; } = string.Empty;
    }
}
