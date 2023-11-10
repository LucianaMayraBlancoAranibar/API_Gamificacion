namespace Gamificacion_API.Models
{
    public class EditAssignmentDto
    {
        public int IdStudent { get; set; } // ID del estudiante, si necesitas cambiarlo
        public int IdAchievement { get; set; } // ID del logro, si necesitas cambiarlo
        public int Points { get; set; }
    }
}
