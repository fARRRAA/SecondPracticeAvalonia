using System;
using System.Collections.Generic;

namespace SecondPracticeAvalonia.Models;

public partial class WebinarParticipant
{
    public int Id { get; set; }

    public int? WebinarId { get; set; }

    public int? StudentId { get; set; }

    public DateTime? JoinedAt { get; set; }

    public DateTime? LeftAt { get; set; }

    public virtual User? Student { get; set; }

    public virtual Webinar? Webinar { get; set; }
}
