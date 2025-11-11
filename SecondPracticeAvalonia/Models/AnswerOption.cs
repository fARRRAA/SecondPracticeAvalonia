using System;
using System.Collections.Generic;

namespace SecondPracticeAvalonia.Models;

public partial class AnswerOption
{
    public int Id { get; set; }

    public int? QuestionId { get; set; }

    public string OptionText { get; set; } = null!;

    public bool? IsCorrect { get; set; }

    public int OrderIndex { get; set; }

    public virtual TestQuestion? Question { get; set; }

    public virtual ICollection<TestAnswer> TestAnswers { get; set; } = new List<TestAnswer>();
}
