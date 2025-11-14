using System;
using System.Collections.Generic;

namespace SecondPracticeAvalonia.Data;

public partial class StudentPoint
{
    public int Id { get; set; }

    public int? StudentId { get; set; }

    public int? TotalPoints { get; set; }

    public int? Level { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual User? Student { get; set; }
}
