using System;
using Srez.Data;

namespace Srez.Services
{

    public static class ProductionCalculator
    {

        public static int CalculateProducedQuantity(int productTypeId, int materialTypeId, int usedMaterialAmount, double param1, double param2)
        {
            try
            {
                if (productTypeId <= 0 || materialTypeId <= 0) return -1;
                if (usedMaterialAmount < 0) return -1;
                if (!(param1 > 0.0) || !(param2 > 0.0)) return -1;

                using var db = new AppDbContext();

                var productType = db.ProductTypes.Find(productTypeId);
                if (productType == null) return -1;

                var materialType = db.MaterialTypes.Find(materialTypeId);
                if (materialType == null) return -1;

                decimal p1 = Convert.ToDecimal(param1);
                decimal p2 = Convert.ToDecimal(param2);

                decimal coeff = productType.Coefficient;

                decimal perUnit = p1 * p2 * coeff;

                if (perUnit <= 0m)
                {
                    return -1;
                }

                decimal lossPct = materialType.LossPercentage;

                decimal lossFactor = 1m + (lossPct / 100m);

                decimal requiredPerUnitWithLoss = perUnit * lossFactor;

                if (requiredPerUnitWithLoss <= 0m)
                {
                    return -1;
                }

                decimal available = Convert.ToDecimal(usedMaterialAmount);

                decimal producedDecimal = Math.Floor(available / requiredPerUnitWithLoss);

                if (producedDecimal < 0m) return 0;

                if (producedDecimal > int.MaxValue) return int.MaxValue;

                return (int)producedDecimal;
            }
            catch
            {
                return -1;
            }
        }
    }
}
