using System;
using System.Collections.Generic;

namespace SecondPracticeAvalonia.Data;

public partial class Bookmark
{
    public int Id { get; set; }

    public int? StudentId { get; set; }

    public int? LessonId { get; set; }

    public int? PositionSeconds { get; set; }

    public string? Note { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Lesson? Lesson { get; set; }

    public virtual User? Student { get; set; }
}
