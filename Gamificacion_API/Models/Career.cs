using System;
using System.Collections.Generic;

namespace Gamificacion_API.Models;

public partial class Career
{
    public int IdCareer { get; set; }

    public string CareerName { get; set; } = null!;

    public int IdDepartment { get; set; }

    public virtual Department? IdDepartmentNavigation { get; set; } = null!;

    public virtual ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
}
