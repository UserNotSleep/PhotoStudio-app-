using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Media;

namespace PhotoStudio.Desktop.Converters;

public class StatusColorConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is string status)
        {
            return status.ToLower() switch
            {
                "scheduled" or "запланирован" or "запланирована" or "запланировано" 
                    or "запланированная" or "запланированный" => new SolidColorBrush(Color.Parse("#3498db")),
                
                "completed" or "завершен" or "завершена" or "завершено" 
                    or "завершенная" or "завершенный" => new SolidColorBrush(Color.Parse("#27ae60")),
                
                "cancelled" or "отменен" or "отменена" or "отменено" 
                    or "отмененная" or "отмененный" => new SolidColorBrush(Color.Parse("#e74c3c")),
                
                _ => new SolidColorBrush(Color.Parse("#6c757d"))
            };
        }
        
        return new SolidColorBrush(Color.Parse("#6c757d"));
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
} 