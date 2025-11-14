using Srez.Data;
using System;
namespace Srez.ExtendedClasses
{
    public class MaterialExtended
    {
        public Material Material { get; set; }
        public MaterialExtended(Material material)
        {
            Material = material;
        }
        public int Id => Material.Id;
        public string Name => Material.Name;
        public string MaterialTypeName => Material.MaterialType?.Name ?? "-";
        public decimal StockQuantity => Material.StockQuantity;
        public decimal MinQuantity => Material.MinQuantity;
        public decimal PackageQuantity => Material.PackageQuantity;
        public decimal UnitPrice => Material.UnitPrice;
        public string MeasurementName => Material.Measurement?.Name ?? "-";
        public bool NeedsRestock => StockQuantity < MinQuantity;
        public bool HasSufficientStock => !NeedsRestock;
        public decimal Deficit => NeedsRestock ? MinQuantity - StockQuantity : 0m;
        public decimal PackagesNeeded
        {
            get
            {
                if (!NeedsRestock || PackageQuantity <= 0)
                {
                    return 0;
                }
                var quotient = Deficit / PackageQuantity;
                return Math.Ceiling(quotient);
            }
        }
        public decimal MinimalPurchaseQuantity
        {
            get
            {
                if (!NeedsRestock)
                {
                    return 0;
                }
                if (PackageQuantity <= 0)
                {
                    return Deficit;
                }
                return PackagesNeeded * PackageQuantity;
            }
        }
        public decimal BatchCost
        {
            get
            {
                if (!NeedsRestock)
                {
                    return 0;
                }
                var cost = MinimalPurchaseQuantity * UnitPrice;
                return Math.Max(0, cost);
            }
        }
        public string FormattedBatchCost => BatchCost > 0 ? BatchCost.ToString("F2") : "0.00";
        public string FormattedPurchaseQuantity => MinimalPurchaseQuantity.ToString("F2");
        public string FormattedDeficit => Deficit.ToString("F2");
        public string FormattedPackagesNeeded => PackagesNeeded.ToString("0");
        public string PurchaseSummary
        {
            get
            {
                if (!NeedsRestock)
                {
                    return string.Empty;
                }
                return $"Закупить: {FormattedPurchaseQuantity} {MeasurementName} ({FormattedPackagesNeeded} упак.) = {FormattedBatchCost} р";
            }
        }
    }
}
