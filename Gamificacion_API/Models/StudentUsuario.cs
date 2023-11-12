namespace Gamificacion_API.Models
{
    public class StudentUsuario
    {
        public int IdStudent { get; set; }

        public int Score { get; set; }

        public int IdRank { get; set; }

        public string FirstName { get; set; } = null!;

        public string LastName { get; set; } = null!;
        public string Email { get; set; } = null!;

        public byte Rol { get; set; }

        public string? Password { get; set; } = null!;

        public int IdCareer { get; set; }

        public int IdAcademicUnity { get; set; }
    }
}
