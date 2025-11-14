using System;
using System.Collections.Generic;

namespace Srez.Data;

public partial class SupplierMaterial
{
    public int Id { get; set; }

    public int MaterialId { get; set; }

    public int SupplierId { get; set; }

    public virtual Material Material { get; set; } = null!;

    public virtual Supplier Supplier { get; set; } = null!;
}
