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
        if (value == null || parameter == null) return false;
        return value.ToString()?.Equals(parameter.ToString(), StringComparison.OrdinalIgnoreCase) ?? false;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not bool isChecked || !isChecked || parameter == null)
            return Binding.DoNothing;
        return Enum.Parse(targetType, parameter.ToString()!);
    }
}
