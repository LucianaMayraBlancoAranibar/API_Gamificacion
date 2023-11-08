using System;
using System.Collections.Generic;

namespace Gamificacion_API.Models;

public partial class Student
{
    public int IdStudent { get; set; }

    public int Score { get; set; }

    public int IdRank { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public virtual ICollection<BadgeStudent> BadgeStudents { get; set; } = new List<BadgeStudent>();

    public virtual Rank IdRankNavigation { get; set; } = null!;

    public virtual Usuario IdStudentNavigation { get; set; } = null!;

    public virtual ICollection<Sanction> Sanctions { get; set; } = new List<Sanction>();

    public virtual ICollection<StudentAchievement> StudentAchievements { get; set; } = new List<StudentAchievement>();
}
