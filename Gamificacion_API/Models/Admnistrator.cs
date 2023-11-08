using System;
using System.Collections.Generic;

namespace Gamificacion_API.Models;

public partial class Admnistrator
{
    public int IdAdministrator { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public virtual ICollection<Achievement> Achievements { get; set; } = new List<Achievement>();

    public virtual ICollection<Badge> Badges { get; set; } = new List<Badge>();

    public virtual Usuario IdAdministratorNavigation { get; set; } = null!;

    public virtual ICollection<Sanction> Sanctions { get; set; } = new List<Sanction>();

    public virtual ICollection<TypeAchievement> TypeAchievements { get; set; } = new List<TypeAchievement>();
}
