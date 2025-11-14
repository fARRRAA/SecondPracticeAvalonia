using Avalonia.Data.Converters;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecondPracticeAvalonia.Converters
{
    public class DateTimeOffsetToDateTimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DateTimeOffset dto)
                return dto.DateTime;

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DateTime dt)
                return new DateTimeOffset(dt);

            return null;
        }
    }
}
