using System;
using System.Collections.Generic;

namespace SecondPracticeAvalonia.Data;

public partial class CourseReview
{
    public int Id { get; set; }

    public int? CourseId { get; set; }

    public int? StudentId { get; set; }

    public int Rating { get; set; }

    public string? Comment { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Course? Course { get; set; }

    public virtual User? Student { get; set; }
}
