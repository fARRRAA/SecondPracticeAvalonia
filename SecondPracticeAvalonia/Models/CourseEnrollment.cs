using System;
using System.Collections.Generic;

namespace SecondPracticeAvalonia.Models;

/// <summary>
/// Записи студентов на курсы
/// </summary>
public partial class CourseEnrollment
{
    public int Id { get; set; }

    public int? StudentId { get; set; }

    public int? CourseId { get; set; }

    public DateTime? EnrolledAt { get; set; }

    public DateTime? CompletedAt { get; set; }

    public decimal? ProgressPercentage { get; set; }

    public string? CertificateUrl { get; set; }

    public virtual Course? Course { get; set; }

    public virtual User? Student { get; set; }
}
