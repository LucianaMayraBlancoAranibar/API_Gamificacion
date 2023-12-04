using System;
using System.Collections.Generic;

namespace Gamificacion_API.Models;

public partial class TypeAchievement
{
    public int IdTypeAchievement { get; set; }

    public string NameTypeAchievement { get; set; } = null!;

    public int IdAdministrator { get; set; }

    public string Route { get; set; } = null!;

    public virtual ICollection<Achievement> Achievements { get; set; } = new List<Achievement>();

    public virtual ICollection<Badge> Badges { get; set; } = new List<Badge>();

    public virtual Admnistrator IdAdministratorNavigation { get; set; } = null!;
}
