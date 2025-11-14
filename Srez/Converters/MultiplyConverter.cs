using Avalonia.Data.Converters;
using System;
using System.Collections.Generic;
using System.Globalization;
namespace Srez.Converters
{
    public class MultiplyConverter : IMultiValueConverter
    {
        public object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
        {
            if (values == null || values.Count < 2)
                return "0.00";
            if (values[0] is decimal minQty && values[1] is decimal unitPrice)
            {
                return (minQty * unitPrice).ToString("F2", culture);
            }
            if (values[0] is double minQtyD && values[1] is double unitPriceD)
            {
                return (minQtyD * unitPriceD).ToString("F2", culture);
            }
            try
            {
                var minQtyValue = System.Convert.ToDecimal(values[0] ?? 0);
                var unitPriceValue = System.Convert.ToDecimal(values[1] ?? 0);
                return (minQtyValue * unitPriceValue).ToString("F2", culture);
            }
            catch
            {
                return "0.00";
            }
        }
        public object[] ConvertBack(object? value, Type[] targetTypes, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
