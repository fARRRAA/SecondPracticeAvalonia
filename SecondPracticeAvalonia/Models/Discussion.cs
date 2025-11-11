using System;
using System.Collections.Generic;

namespace SecondPracticeAvalonia.Models;

/// <summary>
/// Обсуждения и вопросы по урокам
/// </summary>
public partial class Discussion
{
    public int Id { get; set; }

    public int? LessonId { get; set; }

    public int? StudentId { get; set; }

    public string Title { get; set; } = null!;

    public string Content { get; set; } = null!;

    public bool? IsAnswered { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<DiscussionReply> DiscussionReplies { get; set; } = new List<DiscussionReply>();

    public virtual Lesson? Lesson { get; set; }

    public virtual User? Student { get; set; }
}
