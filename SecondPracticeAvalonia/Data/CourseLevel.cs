using System;
using System.Collections.Generic;

namespace SecondPracticeAvalonia.Data;

/// <summary>
/// Справочник уровней сложности курсов
/// </summary>
public partial class CourseLevel
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public virtual ICollection<Course> Courses { get; set; } = new List<Course>();
}
