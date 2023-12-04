using System;
using System.Collections.Generic;

namespace Gamificacion_API.Models;

public partial class BadgeImage
{
    public int IdBadgeImage { get; set; }

    public string Title { get; set; } = null!;

    public string NameImage { get; set; } = null!;

    public virtual ICollection<Badge> Badges { get; set; } = new List<Badge>();
}
