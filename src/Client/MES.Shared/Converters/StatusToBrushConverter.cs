using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace MES.Shared.Converters;

public class StatusToBrushConverter : IValueConverter
{
    private static readonly Brush Green = new SolidColorBrush(Color.FromRgb(0x4C, 0xC5, 0x6C));
    private static readonly Brush Red = new SolidColorBrush(Color.FromRgb(0xE7, 0x4C, 0x3C));
    private static readonly Brush Orange = new SolidColorBrush(Color.FromRgb(0xF3, 0x9C, 0x12));
    private static readonly Brush Blue = new SolidColorBrush(Color.FromRgb(0x34, 0x98, 0xDB));
    private static readonly Brush Gray = new SolidColorBrush(Color.FromRgb(0x95, 0xA5, 0xA6));

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var status = value?.ToString() ?? "";
        return status switch
        {
            "Completed" or "Pass" or "Idle" or "Active" or "Approved" or "Closed" => Green,
            "Released" or "Processing" or "InProgress" => Blue,
            "Created" => Gray,
            "Hold" or "Major" or "Rework" or "Warning" => Orange,
            "Down" or "Critical" or "Scrap" or "Fail" or "Cancelled" => Red,
            _ => Gray
        };
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
        throw new NotImplementedException();
}
