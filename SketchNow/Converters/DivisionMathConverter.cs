using System;
using System.Globalization;
using System.Windows.Data;

namespace SketchNow.Converters
{
    public class DivisionMathConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || parameter == null)
                return null;

            double number;
            double divisor;

            if (double.TryParse(value.ToString(), out number) && double.TryParse(parameter.ToString(), out divisor))
            {
                return number / divisor;
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
