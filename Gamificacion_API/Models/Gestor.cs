using System;
using System.Collections.Generic;

namespace Gamificacion_API.Models;

public partial class Gestor
{
    public int IdGestor { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public virtual ICollection<Achievement> Achievements { get; set; } = new List<Achievement>();

    public virtual Usuario IdGestorNavigation { get; set; } = null!;

    public virtual ICollection<Sanction> Sanctions { get; set; } = new List<Sanction>();
}
