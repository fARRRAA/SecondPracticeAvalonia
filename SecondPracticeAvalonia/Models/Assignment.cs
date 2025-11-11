using System;
using System.Collections.Generic;

namespace SecondPracticeAvalonia.Models;

/// <summary>
/// Задания и тесты
/// </summary>
public partial class Assignment
{
    public int Id { get; set; }

    public int? LessonId { get; set; }

    public string Title { get; set; } = null!;

    public string Description { get; set; } = null!;

    public int? MaxScore { get; set; }

    public DateTime? Deadline { get; set; }

    public bool? IsRequired { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<AssignmentSubmission> AssignmentSubmissions { get; set; } = new List<AssignmentSubmission>();

    public virtual Lesson? Lesson { get; set; }

    public virtual ICollection<TestQuestion> TestQuestions { get; set; } = new List<TestQuestion>();
}
