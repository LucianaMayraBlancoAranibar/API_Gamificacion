namespace Gamificacion_API.Models
{
    public class StudentProfileDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; } 
        public string Career { get; set; }
        public string Faculty { get; set; }
        public string Department { get; set; }
        public string AcademicUnit { get; set; }
        public string RankName { get; set; }
        public string RankLevel { get; set; } // Asumiendo que tienes un nivel de rango
        public string ImagePath { get; set; }
    }
}
