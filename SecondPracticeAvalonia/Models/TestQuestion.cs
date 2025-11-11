using System;
using System.Collections.Generic;

namespace SecondPracticeAvalonia.Models;

public partial class TestQuestion
{
    public int Id { get; set; }

    public int? AssignmentId { get; set; }

    public string QuestionText { get; set; } = null!;

    public string QuestionType { get; set; } = null!;

    public int OrderIndex { get; set; }

    public int? Points { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<AnswerOption> AnswerOptions { get; set; } = new List<AnswerOption>();

    public virtual Assignment? Assignment { get; set; }

    public virtual ICollection<TestAnswer> TestAnswers { get; set; } = new List<TestAnswer>();
}
