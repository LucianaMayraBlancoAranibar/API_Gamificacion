using System;
using System.Collections.Generic;

namespace Gamificacion_API.Models;

public partial class BadgeStudent
{
    public int IdBadgeStudent { get; set; }

    public int IdStudent { get; set; }

    public int IdBadge { get; set; }
    //public string? BadgeName { get; set; } = null;
    //public string? BadgeLevel { get; set; } = null;
    //public string? ImagePath { get; set; } = null;
    public long AccumulatedPoints { get; set; }

    public virtual Badge IdBadgeNavigation { get; set; } = null!;

    public virtual Student IdStudentNavigation { get; set; } = null!;
}
