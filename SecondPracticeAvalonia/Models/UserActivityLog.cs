using System;
using System.Collections.Generic;

namespace SecondPracticeAvalonia.Models;

public partial class UserActivityLog
{
    public int Id { get; set; }

    public int? UserId { get; set; }

    public string ActivityType { get; set; } = null!;

    public string? Description { get; set; }

    public string? Metadata { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual User? User { get; set; }
}
