using System;
using System.Collections.Generic;

namespace SecondPracticeAvalonia.Models;

public partial class DiscussionReply
{
    public int Id { get; set; }

    public int? DiscussionId { get; set; }

    public int? UserId { get; set; }

    public string Content { get; set; } = null!;

    public bool? IsTeacher { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Discussion? Discussion { get; set; }

    public virtual User? User { get; set; }
}
