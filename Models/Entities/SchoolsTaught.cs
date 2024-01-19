namespace RYT.Models.Entities
{
    public class SchoolsTaught
    {
        public string TeacherId { get; set; } = string.Empty;
        public string School { get; set; } = string.Empty;

        // navigation prop
        public Teacher? Teacher { get; set; }
    }
}
