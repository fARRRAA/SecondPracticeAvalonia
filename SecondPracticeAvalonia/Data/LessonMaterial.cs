using System;
using System.Collections.Generic;

namespace SecondPracticeAvalonia.Data;

public partial class LessonMaterial
{
    public int Id { get; set; }

    public int? LessonId { get; set; }

    public string Title { get; set; } = null!;

    public string? FileUrl { get; set; }

    public string? FileType { get; set; }

    public int? FileSizeKb { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Lesson? Lesson { get; set; }
}
