namespace Gamificacion_API.Models
{
    public class AchievementUpdateDto
    {
        public int? IdAchievement { get; set; }
        public string NameAchievemt { get; set; }
        public int Punctuation { get; set; }
        public string ProjectName { get; set; }
        public int IdTypeAchievement { get; set; }
        public string? TypeName { get; set; }
    }
}
