using System;
using System.Globalization;
using System.Windows.Data;

namespace MES.Shared.Converters;

/// <summary>
/// 枚举值匹配转换器：用于 RadioButton 绑定枚举属性
/// 用法: IsChecked="{Binding MyEnum, Converter={StaticResource EnumMatch}, ConverterParameter=SomeValue}"
/// </summary>
public class EnumMatchConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (parameter is not string paramStr) return false;

        // ALL 代表"全部"：value 为 null 时才被选中
        if (paramStr == "ALL")
            return value == null;

        if (value == null) return false;
        return value.ToString()!.Equals(paramStr, StringComparison.OrdinalIgnoreCase);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not bool isChecked || !isChecked || parameter == null)
            return Binding.DoNothing;

        var paramStr = parameter.ToString()!;

        // ALL 代表"全部"：返回 null 清除筛选
        if (string.Equals(paramStr, "ALL", StringComparison.OrdinalIgnoreCase))
            return null;

        var enumType = Nullable.GetUnderlyingType(targetType) ?? targetType;
        return Enum.Parse(enumType, paramStr);
    }
}
