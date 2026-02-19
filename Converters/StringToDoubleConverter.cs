using Avalonia.Data.Converters;
using System;
using System.Globalization;

namespace HealthOptimizer.Converters
{
    public class StringToDoubleConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is double d)
            {
                return d == 0 ? string.Empty : d.ToString();
            }
            return string.Empty;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is string str)
            {
                if (string.IsNullOrWhiteSpace(str))
                    return 0.0;

                if (double.TryParse(str, out double result))
                    return result;
            }
            return 0.0;
        }
    }

    public class StringToIntConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is int i)
            {
                return i == 0 ? string.Empty : i.ToString();
            }
            return string.Empty;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is string str)
            {
                if (string.IsNullOrWhiteSpace(str))
                    return 0;

                if (int.TryParse(str, out int result))
                    return result;
            }
            return 0;
        }
    }
}