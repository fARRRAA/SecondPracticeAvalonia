using System;
using System.Collections.Generic;

namespace SecondPracticeAvalonia.Models;

public partial class TestAnswer
{
    public int Id { get; set; }

    public int? SubmissionId { get; set; }

    public int? QuestionId { get; set; }

    public int? SelectedOptionId { get; set; }

    public string? TextAnswer { get; set; }

    public bool? IsCorrect { get; set; }

    public int? PointsEarned { get; set; }

    public virtual TestQuestion? Question { get; set; }

    public virtual AnswerOption? SelectedOption { get; set; }

    public virtual AssignmentSubmission? Submission { get; set; }
}
