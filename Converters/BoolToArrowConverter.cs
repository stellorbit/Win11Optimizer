using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows;

namespace Win11Optimizer.Converters
{
    public class BoolToArrowConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool b)
                return b ? "▼" : "▶";   // true: 下向き、false: 右向き
            return DependencyProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // 双方向バインディングが必要なければ NotImplemented で可
            throw new NotImplementedException();
        }
    }
}
