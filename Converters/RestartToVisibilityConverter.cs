using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows;

namespace Win11Optimizer.Converters
{
    public class RestartToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // 値が true のときだけ表示（再起動必要項目を示す想定）
            if (value is bool b)
                return b ? Visibility.Visible : Visibility.Collapsed;
            return DependencyProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
