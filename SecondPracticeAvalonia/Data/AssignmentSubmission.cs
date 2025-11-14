using System;
using System.Collections.Generic;

namespace SecondPracticeAvalonia.Data;

/// <summary>
/// Выполненные задания студентов
/// </summary>
public partial class AssignmentSubmission
{
    public int Id { get; set; }

    public int? AssignmentId { get; set; }

    public int? StudentId { get; set; }

    public int? StatusId { get; set; }

    public string? Content { get; set; }

    public string? FileUrl { get; set; }

    public int? Score { get; set; }

    public string? TeacherComment { get; set; }

    public DateTime? SubmittedAt { get; set; }

    public DateTime? CheckedAt { get; set; }

    public int? CheckedBy { get; set; }

    public virtual Assignment? Assignment { get; set; }

    public virtual User? CheckedByNavigation { get; set; }

    public virtual AssignmentStatus? Status { get; set; }

    public virtual User? Student { get; set; }

    public virtual ICollection<TestAnswer> TestAnswers { get; set; } = new List<TestAnswer>();
}
