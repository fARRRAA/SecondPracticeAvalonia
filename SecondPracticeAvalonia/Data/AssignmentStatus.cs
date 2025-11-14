using System;
using System.Collections.Generic;

namespace SecondPracticeAvalonia.Data;

/// <summary>
/// Справочник статусов заданий
/// </summary>
public partial class AssignmentStatus
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public virtual ICollection<AssignmentSubmission> AssignmentSubmissions { get; set; } = new List<AssignmentSubmission>();
}
