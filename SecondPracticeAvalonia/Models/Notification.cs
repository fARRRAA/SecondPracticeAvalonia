using System;
using System.Collections.Generic;

namespace SecondPracticeAvalonia.Models;

/// <summary>
/// Уведомления пользователей
/// </summary>
public partial class Notification
{
    public int Id { get; set; }

    public int? UserId { get; set; }

    public string Title { get; set; } = null!;

    public string Message { get; set; } = null!;

    public string? LinkUrl { get; set; }

    public bool? IsRead { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual User? User { get; set; }
}
