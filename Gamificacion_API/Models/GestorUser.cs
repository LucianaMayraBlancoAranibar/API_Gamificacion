namespace Gamificacion_API.Models
{
    public class GestorUser
    {
        public int IdGestor { get; set; }

        public string Email { get; set; } = null!;

        public byte Rol { get; set; }

        public string? Password { get; set; } = null!;

        public string FirstName { get; set; } = null!;

        public string LastName { get; set; } = null!;

        public int IdAcademicUnity { get; set; }

        public int IdCareer { get; set; }
    }
}
