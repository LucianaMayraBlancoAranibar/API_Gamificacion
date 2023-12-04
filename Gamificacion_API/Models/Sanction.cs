using System;
using System.Collections.Generic;

namespace Gamificacion_API.Models;

public partial class Sanction
{
    public int IdSanctions { get; set; }

    public string Description { get; set; } = null!;

    public byte Sanction1 { get; set; }

    public int IdStudent { get; set; }

    public int? IdGestor { get; set; }

    public int? IdAdministrator { get; set; }

    public virtual Admnistrator? IdAdministratorNavigation { get; set; }

    public virtual Gestor? IdGestorNavigation { get; set; }

    public virtual Student IdStudentNavigation { get; set; } = null!;
}
