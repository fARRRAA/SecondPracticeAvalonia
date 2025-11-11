using System;
using System.Collections.Generic;

namespace SecondPracticeAvalonia.Models;

public partial class Announcement
{
    public int Id { get; set; }

    public int? CourseId { get; set; }

    public int? TeacherId { get; set; }

    public string Title { get; set; } = null!;

    public string Content { get; set; } = null!;

    public bool? IsPublished { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Course? Course { get; set; }

    public virtual User? Teacher { get; set; }
}
