using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace MES.Shared.Converters;

public class PriorityToBrushConverter : IValueConverter
{
    private static readonly Brush Urgent = new SolidColorBrush(Color.FromRgb(0xFF, 0x47, 0x57));
    private static readonly Brush High = new SolidColorBrush(Color.FromRgb(0xE7, 0x4C, 0x3C));
    private static readonly Brush Normal = new SolidColorBrush(Color.FromRgb(0x34, 0x98, 0xDB));
    private static readonly Brush Low = new SolidColorBrush(Color.FromRgb(0x95, 0xA5, 0xA6));

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value?.ToString() switch
        {
            "Urgent" => Urgent,
            "High" => High,
            "Normal" => Normal,
            "Low" => Low,
            _ => Low
        };
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
        throw new NotImplementedException();
}
