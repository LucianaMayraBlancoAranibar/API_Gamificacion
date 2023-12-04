namespace Gamificacion_API.Models
{
    public class BadgeCreationRequestDto
    {
        public int AdministratorId { get; set; }
        public string BadgeName { get; set; }
        public int IdTypeAchievement { get; set; }
    }
}
