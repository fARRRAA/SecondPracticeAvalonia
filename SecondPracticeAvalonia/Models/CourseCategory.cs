using System;
using System.Collections.Generic;

namespace SecondPracticeAvalonia.Models;

public partial class CourseCategory
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public string? IconUrl { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<Course> Courses { get; set; } = new List<Course>();
}
