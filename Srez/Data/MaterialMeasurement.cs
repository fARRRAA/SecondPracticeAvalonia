using System;
using System.Collections.Generic;

namespace Srez.Data;

public partial class MaterialMeasurement
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Material> Materials { get; set; } = new List<Material>();
}
