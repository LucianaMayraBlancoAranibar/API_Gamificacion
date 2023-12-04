namespace Gamificacion_API.Models
{
    public class DefaultBadgeCreationDto
    {
        public string BadgeName { get; set; }
        public string BadgeLevel { get; set; }
        public int Points { get; set; }
        public string ImagePath { get; set; }
        public int IdTypeAchievement { get; set; }
    }

}
