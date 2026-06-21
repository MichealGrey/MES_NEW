using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace MES.Modules.Order.Views;

public class RiskLevelToColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value?.ToString() switch
        {
            "High" => new SolidColorBrush(Colors.Red),
            "Medium" => new SolidColorBrush(Colors.Orange),
            "Low" => new SolidColorBrush(Colors.Green),
            _ => new SolidColorBrush(Colors.Gray)
        };
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

public class BoolToRiskColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value is true ? new SolidColorBrush(Colors.Red) : new SolidColorBrush(Colors.Green);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

public class BoolToRiskTextConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value is true ? "有风险" : "正常";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
