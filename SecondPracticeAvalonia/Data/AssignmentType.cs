using System;
using System.Collections.Generic;

namespace SecondPracticeAvalonia.Data;

/// <summary>
/// Справочник типов заданий
/// </summary>
public partial class AssignmentType
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public virtual ICollection<Assignment> Assignments { get; set; } = new List<Assignment>();
}
