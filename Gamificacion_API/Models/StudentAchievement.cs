using System;
using System.Collections.Generic;

namespace Gamificacion_API.Models;

public partial class StudentAchievement
{
    public int IdStudentAchievement { get; set; }

    public int IdStudent { get; set; }

    public int IdAchievement { get; set; }

    public int StudentPoints { get; set; }

    public virtual Achievement IdAchievementNavigation { get; set; } = null!;

    public virtual Student IdStudentNavigation { get; set; } = null!;
}
