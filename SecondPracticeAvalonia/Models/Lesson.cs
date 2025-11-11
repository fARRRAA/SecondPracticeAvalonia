using System;
using System.Collections.Generic;

namespace SecondPracticeAvalonia.Models;

/// <summary>
/// Уроки внутри модулей
/// </summary>
public partial class Lesson
{
    public int Id { get; set; }

    public int? ModuleId { get; set; }

    public string Title { get; set; } = null!;

    public string? Content { get; set; }

    public string? VideoUrl { get; set; }

    public int? DurationMinutes { get; set; }

    public int OrderIndex { get; set; }

    public bool? IsPublished { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<Assignment> Assignments { get; set; } = new List<Assignment>();

    public virtual ICollection<Bookmark> Bookmarks { get; set; } = new List<Bookmark>();

    public virtual ICollection<Discussion> Discussions { get; set; } = new List<Discussion>();

    public virtual ICollection<LessonMaterial> LessonMaterials { get; set; } = new List<LessonMaterial>();

    public virtual ICollection<LessonProgress> LessonProgresses { get; set; } = new List<LessonProgress>();

    public virtual CourseModule? Module { get; set; }
}
