using System;
using System.Collections.Generic;

namespace SecondPracticeAvalonia.Models;

public partial class NotificationSetting
{
    public int Id { get; set; }

    public int? UserId { get; set; }

    public bool? EmailNotifications { get; set; }

    public bool? PushNotifications { get; set; }

    public bool? DeadlineReminders { get; set; }

    public bool? NewMaterials { get; set; }

    public bool? GradeNotifications { get; set; }

    public bool? Announcements { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual User? User { get; set; }
}
