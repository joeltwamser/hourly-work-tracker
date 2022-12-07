using System;
using System.Globalization;
using System.Windows.Data;

namespace hourlyWorkTracker.Converters
{
    public class MoneyFormatter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double d = (double)value;
            return $"${d:F2}";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string s = (string)value;
            _ = double.TryParse(s[1..], out double result);
            return result;
        }
    }
}
