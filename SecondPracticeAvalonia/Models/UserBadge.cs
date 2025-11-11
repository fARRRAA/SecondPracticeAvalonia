using System;
using System.Collections.Generic;

namespace SecondPracticeAvalonia.Models;

public partial class UserBadge
{
    public int Id { get; set; }

    public int? UserId { get; set; }

    public int? BadgeId { get; set; }

    public DateTime? EarnedAt { get; set; }

    public virtual Badge? Badge { get; set; }

    public virtual User? User { get; set; }
}
