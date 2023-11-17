using System;
using System.Collections.Generic;

namespace Gamificacion_API.Models;

public partial class Department
{
    public int IdDepartment { get; set; }

    public string DepartmentName { get; set; } = null!;

    public int IdFaculty { get; set; }

    public virtual ICollection<Career> Careers { get; set; } = new List<Career>();

    public virtual Faculty? IdFacultyNavigation { get; set; } = null!;
}
