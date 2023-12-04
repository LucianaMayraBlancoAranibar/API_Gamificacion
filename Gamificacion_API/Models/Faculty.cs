using System;
using System.Collections.Generic;

namespace Gamificacion_API.Models;

public partial class Faculty
{
    public int IdFaculty { get; set; }

    public string FacultyName { get; set; } = null!;

    public virtual ICollection<Department> Departments { get; set; } = new List<Department>();
}
