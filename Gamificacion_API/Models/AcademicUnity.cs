using System;
using System.Collections.Generic;

namespace Gamificacion_API.Models;

public partial class AcademicUnity
{
    public int IdAcademicUnity { get; set; }

    public string AcademicUnityName { get; set; } = null!;

    public virtual ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
}
