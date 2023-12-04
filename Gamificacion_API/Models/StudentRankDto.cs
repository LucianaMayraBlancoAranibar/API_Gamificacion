namespace Gamificacion_API.Models
{
    public class StudentRankDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Score { get; set; }
        public string RankName { get; set; }
        public string SubRankName { get; set; }
        public string ImagePath { get; set; }
        // Nuevos campos agregados
        public int CurrentLevel { get; set; }
        public int Achievements { get; set; }
        public int AchievementsCount { get; set; }
        public int ActivityCompletions { get; set; }
    }
}
