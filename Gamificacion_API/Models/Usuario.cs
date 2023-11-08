using System;
using System.Collections.Generic;

namespace Gamificacion_API.Models;

public partial class Usuario
{
    public int IdUsuario { get; set; }

    public string Email { get; set; } = null!;

    public byte Rol { get; set; }

    public string Password { get; set; } = null!;

    public int? IdCareer { get; set; }

    public int IdAcademicUnity { get; set; }

    public virtual Admnistrator? Admnistrator { get; set; }

    public virtual Gestor? Gestor { get; set; }

    public virtual AcademicUnity IdAcademicUnityNavigation { get; set; } = null!;

    public virtual Career? IdCareerNavigation { get; set; }

    public virtual Student? Student { get; set; }
}
