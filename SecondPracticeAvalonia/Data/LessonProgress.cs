using System;
using System.Collections.Generic;

namespace SecondPracticeAvalonia.Data;

/// <summary>
/// Прогресс прохождения уроков
/// </summary>
public partial class LessonProgress
{
    public int Id { get; set; }

    public int? StudentId { get; set; }

    public int? LessonId { get; set; }

    public bool? IsCompleted { get; set; }

    public int? LastPositionSeconds { get; set; }

    public DateTime? CompletedAt { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Lesson? Lesson { get; set; }

    public virtual User? Student { get; set; }
}
