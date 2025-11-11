using System;
using System.Collections.Generic;

namespace SecondPracticeAvalonia.Models;

public partial class Webinar
{
    public int Id { get; set; }

    public int? CourseId { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public int? TeacherId { get; set; }

    public DateTime ScheduledAt { get; set; }

    public int? DurationMinutes { get; set; }

    public string? MeetingUrl { get; set; }

    public string? RecordingUrl { get; set; }

    public bool? IsLive { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Course? Course { get; set; }

    public virtual User? Teacher { get; set; }

    public virtual ICollection<WebinarParticipant> WebinarParticipants { get; set; } = new List<WebinarParticipant>();
}
