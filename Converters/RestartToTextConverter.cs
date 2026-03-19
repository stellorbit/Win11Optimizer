using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Win11Optimizer.Converters
{
    public class RestartToTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool b)
                return b ? "再起動が必要" : string.Empty;
            return DependencyProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
