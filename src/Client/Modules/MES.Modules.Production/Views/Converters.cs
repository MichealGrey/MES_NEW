using System;
using System.Globalization;
using System.Windows.Data;

namespace MES.Modules.Production.Views;

public class BoolToStatusConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool isAck)
            return isAck ? "已确认" : "未处理";
        return "未知";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

public class RiskLevelToColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value switch
        {
            "Critical" => "#F38BA8",
            "Red" => "#EBA0AC",
            "Yellow" => "#F9E2AF",
            "Green" => "#A6E3A1",
            _ => "#CDD6F4"
        };
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

public class HealthStatusToColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value switch
        {
            "Critical" => "#F38BA8",
            "Degraded" => "#F9E2AF",
            "Healthy" => "#A6E3A1",
            _ => "#CDD6F4"
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
        if (value is bool isAtRisk)
            return isAtRisk ? "#F38BA8" : "#A6E3A1";
        return "#CDD6F4";
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
        if (value is bool isAtRisk)
            return isAtRisk ? "有风险" : "正常";
        return "未知";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
