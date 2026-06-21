using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace MES.Shared.Converters;

/// <summary>
/// Converts empty string to Collapsed, non-empty string to Visible.
/// Used for showing/hiding error messages.
/// </summary>
public class StringToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
        value is string s && !string.IsNullOrEmpty(s) ? Visibility.Visible : Visibility.Collapsed;

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
        throw new NotImplementedException();
}
