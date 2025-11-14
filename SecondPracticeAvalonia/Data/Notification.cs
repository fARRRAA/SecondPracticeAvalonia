using System;
using System.Collections.Generic;

namespace SecondPracticeAvalonia.Data;

/// <summary>
/// Уведомления пользователей
/// </summary>
public partial class Notification
{
    public int Id { get; set; }

    public int? UserId { get; set; }

    public int TypeId { get; set; }

    public string Title { get; set; } = null!;

    public string Message { get; set; } = null!;

    public string? LinkUrl { get; set; }

    public bool? IsRead { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual NotificationType Type { get; set; } = null!;

    public virtual User? User { get; set; }
}
