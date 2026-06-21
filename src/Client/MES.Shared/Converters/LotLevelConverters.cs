using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using MES.Domain.Production;

namespace MES.Shared.Converters
{

    /// <summary>
    /// 批次层级转 Brush 转换器
    /// 用于根据批次层级返回对应的颜色 Brush
    /// </summary>
    public class LotLevelToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is LotLevel level)
            {
                return GetBrushForLevel(level);
            }

            if (value is int levelInt)
            {
                return GetBrushForLevel((LotLevel)levelInt);
            }

            if (value is string levelStr)
            {
                if (Enum.TryParse<LotLevel>(levelStr, true, out var parsedLevel))
                {
                    return GetBrushForLevel(parsedLevel);
                }
            }

            return new SolidColorBrush(Color.FromRgb(0x2A, 0x35, 0x45));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        private static Brush GetBrushForLevel(LotLevel level)
        {
            return level switch
            {
                LotLevel.WaferLot => new SolidColorBrush(Color.FromRgb(0x8B, 0x5C, 0xF6)),
                LotLevel.MotherLot => new SolidColorBrush(Color.FromRgb(0x06, 0xB6, 0xD4)),
                LotLevel.SubLot => new SolidColorBrush(Color.FromRgb(0x10, 0xB9, 0x81)),
                LotLevel.GradeLot => new SolidColorBrush(Color.FromRgb(0xF5, 0x9E, 0x0B)),
                LotLevel.MfgId => new SolidColorBrush(Color.FromRgb(0xEC, 0x48, 0x99)),
                _ => new SolidColorBrush(Color.FromRgb(0x2A, 0x35, 0x45))
            };
        }
    }

    /// <summary>
    /// 批次层级转文字转换器
    /// 用于显示批次层级的友好名称
    /// </summary>
    public class LotLevelToTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is LotLevel level)
            {
                return GetTextForLevel(level);
            }

            if (value is int levelInt)
            {
                return GetTextForLevel((LotLevel)levelInt);
            }

            if (value is string levelStr)
            {
                if (Enum.TryParse<LotLevel>(levelStr, true, out var parsedLevel))
                {
                    return GetTextForLevel(parsedLevel);
                }
            }

            return "未知";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        private static string GetTextForLevel(LotLevel level)
        {
            return level switch
            {
                LotLevel.WaferLot => "Wafer",
                LotLevel.MotherLot => "Mother",
                LotLevel.SubLot => "Sub",
                LotLevel.GradeLot => "Grade",
                LotLevel.MfgId => "MFG",
                _ => "未知"
            };
        }
    }

    /// <summary>
    /// 批次层级转颜色值转换器（用于色块背景）
    /// </summary>
    public class LotLevelToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is LotLevel level)
            {
                return GetColorForLevel(level);
            }

            if (value is int levelInt)
            {
                return GetColorForLevel((LotLevel)levelInt);
            }

            return Color.FromRgb(0x2A, 0x35, 0x45);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        private static Color GetColorForLevel(LotLevel level)
        {
            return level switch
            {
                LotLevel.WaferLot => Color.FromRgb(0x8B, 0x5C, 0xF6),
                LotLevel.MotherLot => Color.FromRgb(0x06, 0xB6, 0xD4),
                LotLevel.SubLot => Color.FromRgb(0x10, 0xB9, 0x81),
                LotLevel.GradeLot => Color.FromRgb(0xF5, 0x9E, 0x0B),
                LotLevel.MfgId => Color.FromRgb(0xEC, 0x48, 0x99),
                _ => Color.FromRgb(0x2A, 0x35, 0x45)
            };
        }
    }

    /// <summary>
    /// 批次层级文字颜色转换器
    /// 根据层级背景色返回合适的文字颜色（深色背景用浅色文字）
    /// </summary>
    public class LotLevelToForegroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is LotLevel level)
            {
                return GetForegroundForLevel(level);
            }

            if (value is int levelInt)
            {
                return GetForegroundForLevel((LotLevel)levelInt);
            }

            return Brushes.White;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        private static Brush GetForegroundForLevel(LotLevel level)
        {
            return level switch
            {
                LotLevel.WaferLot => Brushes.White,
                LotLevel.MotherLot => Brushes.White,
                LotLevel.SubLot => Brushes.White,
                LotLevel.GradeLot => new SolidColorBrush(Color.FromRgb(0x0F, 0x15, 0x21)),
                LotLevel.MfgId => new SolidColorBrush(Color.FromRgb(0x0F, 0x15, 0x21)),
                _ => Brushes.White
            };
        }
    }

    /// <summary>
    /// 批次层级图标转换器
    /// 根据层级返回对应的图标符号
    /// </summary>
    public class LotLevelToIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is LotLevel level)
            {
                return GetIconForLevel(level);
            }

            if (value is int levelInt)
            {
                return GetIconForLevel((LotLevel)levelInt);
            }

            return "📦";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        private static string GetIconForLevel(LotLevel level)
        {
            return level switch
            {
                LotLevel.WaferLot => "◇",
                LotLevel.MotherLot => "○",
                LotLevel.SubLot => "◉",
                LotLevel.GradeLot => "◐",
                LotLevel.MfgId => "●",
                _ => "📦"
            };
        }
    }

    /// <summary>
    /// 批次状态转颜色转换器
    /// </summary>
    public class LotStatusToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string status)
            {
                return status.ToUpper() switch
                {
                    "在制" or "PROCESSING" or "WIP" => new SolidColorBrush(Color.FromRgb(0x3B, 0x82, 0xF6)),
                    "等待" or "WAITING" => new SolidColorBrush(Color.FromRgb(0xF5, 0x9E, 0x0B)),
                    "暂扣" or "HOLD" => new SolidColorBrush(Color.FromRgb(0xEF, 0x44, 0x44)),
                    "完成" or "COMPLETED" or "FINISHED" => new SolidColorBrush(Color.FromRgb(0x10, 0xB9, 0x81)),
                    "报废" or "SCRAPPED" => new SolidColorBrush(Color.FromRgb(0x6B, 0x72, 0x80)),
                    "待进站" or "WAIT_TRACK_IN" => new SolidColorBrush(Color.FromRgb(0x94, 0xA3, 0xB8)),
                    "待出站" or "WAIT_TRACK_OUT" => new SolidColorBrush(Color.FromRgb(0x94, 0xA3, 0xB8)),
                    _ => new SolidColorBrush(Color.FromRgb(0x94, 0xA3, 0xB8))
                };
            }

            return new SolidColorBrush(Color.FromRgb(0x94, 0xA3, 0xB8));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
