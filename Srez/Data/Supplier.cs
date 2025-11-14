using System;
using System.Collections.Generic;

namespace Srez.Data;

public partial class Supplier
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public int SupplierTypeId { get; set; }

    public string Inn { get; set; } = null!;

    public int? Rating { get; set; }

    public DateOnly StartDate { get; set; }

    public virtual ICollection<SupplierMaterial> SupplierMaterials { get; set; } = new List<SupplierMaterial>();

    public virtual SupplierType SupplierType { get; set; } = null!;
}
