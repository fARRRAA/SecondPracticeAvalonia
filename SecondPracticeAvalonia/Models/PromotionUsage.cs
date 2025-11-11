using System;
using System.Collections.Generic;

namespace SecondPracticeAvalonia.Models;

public partial class PromotionUsage
{
    public int Id { get; set; }

    public int? PromotionId { get; set; }

    public int? UserId { get; set; }

    public int? CourseId { get; set; }

    public DateTime? UsedAt { get; set; }

    public virtual Course? Course { get; set; }

    public virtual Promotion? Promotion { get; set; }

    public virtual User? User { get; set; }
}
