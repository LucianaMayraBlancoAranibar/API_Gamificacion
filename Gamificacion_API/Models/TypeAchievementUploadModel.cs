namespace Gamificacion_API.Models
{
    public class TypeAchievementUploadModel
    {
        public string NameTypeAchievement { get; set; } = null!;

        public int IdAdministrator { get; set; }

        public IFormFile Image { get; set; }
    }
}
