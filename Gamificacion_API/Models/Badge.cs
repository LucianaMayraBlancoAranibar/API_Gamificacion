using System;
using System.Collections.Generic;

namespace Gamificacion_API.Models;

public partial class Badge
{
    public int IdBadge { get; set; }

    public string BadgeName { get; set; } = null!;

    public int IdAdministrator { get; set; }

    public string? BadgeLevel { get; set; } = null!;

    public int? IdBadgeImage { get; set; }

    public int IdTypeAchivement { get; set; }

    public int Points { get; set; }
    public string? ImagePath { get; set; }
    public int? NextLevelBadgeId { get; set; } // Permite valores nulos

    // Propiedad de navegación
    public virtual Badge? NextLevelBadge { get; set; }

    public virtual ICollection<BadgeStudent> BadgeStudents { get; set; } = new List<BadgeStudent>();

    public virtual Admnistrator IdAdministratorNavigation { get; set; } = null!;

    public virtual BadgeImage IdBadgeImageNavigation { get; set; } = null!;

    public virtual TypeAchievement IdTypeAchivementNavigation { get; set; } = null!;
}
