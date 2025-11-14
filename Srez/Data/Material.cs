using System;
using System.Collections.Generic;

namespace Srez.Data;

public partial class Material
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public int MaterialTypeId { get; set; }

    public decimal UnitPrice { get; set; }

    public decimal StockQuantity { get; set; }

    public decimal MinQuantity { get; set; }

    public decimal PackageQuantity { get; set; }

    public int MeasurementId { get; set; }

    public virtual MaterialType MaterialType { get; set; } = null!;

    public virtual MaterialMeasurement Measurement { get; set; } = null!;

    public virtual ICollection<SupplierMaterial> SupplierMaterials { get; set; } = new List<SupplierMaterial>();
}
