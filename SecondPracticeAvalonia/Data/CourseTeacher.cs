using System;
using System.Collections.Generic;

namespace SecondPracticeAvalonia.Data;

public partial class CourseTeacher
{
    public int Id { get; set; }

    public int? CourseId { get; set; }

    public int? TeacherId { get; set; }

    public DateTime? AssignedAt { get; set; }

    public virtual Course? Course { get; set; }

    public virtual User? Teacher { get; set; }
}
