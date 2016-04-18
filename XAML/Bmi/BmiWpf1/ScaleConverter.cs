using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace BmiWpf1
{
    [ValueConversion(typeof(double), typeof(double))]
    public class ScaleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var valueD = ToValue(value);
            var scale = ToScale(parameter);

            return valueD.HasValue ? valueD * scale : DependencyProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var valueD = ToValue(value);
            var scale = ToScale(parameter);

            return valueD.HasValue && scale != 0.0 ? valueD / scale : DependencyProperty.UnsetValue;
        }

        static double? ToValue(object o)
        {
            try
            {
                return System.Convert.ToDouble(o);
            }
            catch (Exception)
            {
                return null;
            }
        }

        static double ToScale(object o)
        {
            try
            {
                return System.Convert.ToDouble(o);
            }
            catch (Exception)
            {
                return 1.0;
            }
        }
    }
}
