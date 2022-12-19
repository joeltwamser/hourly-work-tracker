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
            //this only rounds to 2 decimal points.  need to add commas every 3 digits.
            return $"{d:C2}";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string s = (string)value;
            _ = double.TryParse(s[1..], out double result);
            return result;
        }
    }
}
