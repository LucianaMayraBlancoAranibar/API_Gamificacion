using System;
using System.Collections.Generic;

namespace Gamificacion_API.Models;

public partial class Rank
{
    public int IdRank { get; set; }

    public string NameRank { get; set; } = null!;

    public string NameSubrank { get; set; } = null!;

    public string? ImagePath { get; set; }

    public virtual ICollection<Student> Students { get; set; } = new List<Student>();
}
