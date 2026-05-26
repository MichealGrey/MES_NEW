using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace MES.Shared.Converters;

/// <summary>
/// HoldType 枚举 → 颜色画刷（通过 ToString 匹配，避免 Shared 引用 Module）
/// </summary>
public class HoldTypeToBrushConverter : IValueConverter
{
    private static readonly Brush EngBrush = new SolidColorBrush(Color.FromRgb(0x34, 0x98, 0xDB));   // 蓝 - 工程
    private static readonly Brush QABrush = new SolidColorBrush(Color.FromRgb(0xE7, 0x4C, 0x3C));    // 红 - 品质
    private static readonly Brush CustBrush = new SolidColorBrush(Color.FromRgb(0x9B, 0x59, 0xB6));  // 紫 - 客户
    private static readonly Brush MatBrush = new SolidColorBrush(Color.FromRgb(0xF3, 0x9C, 0x12));   // 橙 - 物料
    private static readonly Brush EqBrush = new SolidColorBrush(Color.FromRgb(0x95, 0xA5, 0xA6));    // 灰 - 设备
    private static readonly Brush YieldBrush = new SolidColorBrush(Color.FromRgb(0xE6, 0x7E, 0x22)); // 深橙 - 良率
    private static readonly Brush DataBrush = new SolidColorBrush(Color.FromRgb(0x1A, 0xBC, 0x9C));   // 青绿 - 数据
    private static readonly Brush DefaultBrush = new SolidColorBrush(Color.FromRgb(0x95, 0xA5, 0xA6));

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value?.ToString() switch
        {
            "Engineering" => EngBrush,
            "Quality" => QABrush,
            "Customer" => CustBrush,
            "Material" => MatBrush,
            "Equipment" => EqBrush,
            "YieldHold" => YieldBrush,
            "DataHold" => DataBrush,
            _ => DefaultBrush
        };
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
        throw new NotImplementedException();
}

/// <summary>
/// bool (IsHoldOverdue) → 行背景色：超时标红
/// </summary>
public class OverdueToBrushConverter : IValueConverter
{
    private static readonly Brush OverdueBackground = new SolidColorBrush(Color.FromArgb(0x30, 0xE7, 0x4C, 0x3C)); // 半透明红
    private static readonly Brush NormalBackground = Brushes.Transparent;

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value is true ? OverdueBackground : NormalBackground;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
        throw new NotImplementedException();
}
