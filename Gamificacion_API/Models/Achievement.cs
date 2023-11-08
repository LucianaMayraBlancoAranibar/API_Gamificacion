using System;
using System.Collections.Generic;

namespace Gamificacion_API.Models;

public partial class Achievement
{
    public int IdAchievement { get; set; }

    public string NameAchievemt { get; set; } = null!;

    public int Punctuation { get; set; }

    public string ProjectName { get; set; } = null!;

    public int IdTypeAchievement { get; set; }

    public int? IdGestor { get; set; }

    public int? IdAdministrator { get; set; }

    //public virtual ICollection<BadgeStudent> BadgeStudents { get; set; } = new List<BadgeStudent>();

    public virtual Admnistrator? IdAdministratorNavigation { get; set; }

    public virtual Gestor? IdGestorNavigation { get; set; }

    public virtual TypeAchievement IdTypeAchievementNavigation { get; set; } = null!;

    public virtual ICollection<StudentAchievement> StudentAchievements { get; set; } = new List<StudentAchievement>();
}
