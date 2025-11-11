using System;
using System.Collections.Generic;

namespace SecondPracticeAvalonia.Models;

public partial class CourseStatistic
{
    public int Id { get; set; }

    public int? CourseId { get; set; }

    public int? TotalEnrollments { get; set; }

    public int? TotalCompletions { get; set; }

    public decimal? CompletionRate { get; set; }

    public decimal? AverageRating { get; set; }

    public decimal? TotalRevenue { get; set; }

    public DateTime? LastUpdated { get; set; }

    public virtual Course? Course { get; set; }
}
