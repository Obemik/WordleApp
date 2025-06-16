using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using WordleGameEngine.Enums;

namespace WordleApp.Converters;

/// <summary>
/// Конвертер GuessResult у колір
/// </summary>
public class GuessResultToColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is GuessResult result)
        {
            return result switch
            {
                GuessResult.Correct => new SolidColorBrush(Color.FromRgb(106, 170, 100)), // Зелений
                GuessResult.Present => new SolidColorBrush(Color.FromRgb(201, 180, 88)),  // Жовтий
                GuessResult.Absent => new SolidColorBrush(Color.FromRgb(120, 124, 126)),  // Сірий
                _ => new SolidColorBrush(Color.FromRgb(211, 214, 218))                    // Білий/сірий за замовчуванням
            };
        }
        return new SolidColorBrush(Color.FromRgb(211, 214, 218));
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

/// <summary>
/// Конвертер GuessResult у колір тексту
/// </summary>
public class GuessResultToTextColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is GuessResult result)
        {
            return result switch
            {
                GuessResult.Correct => Brushes.White,
                GuessResult.Present => Brushes.White,
                GuessResult.Absent => Brushes.White,
                _ => Brushes.Black
            };
        }
        return Brushes.Black;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

/// <summary>
/// Конвертер bool у команду (для вибору між двома командами)
/// </summary>
public class BoolToCommandConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool boolValue && parameter is string commandPair)
        {
            var commands = commandPair.Split('|');
            if (commands.Length == 2)
            {
                // Return the command name based on boolean value
                return boolValue ? commands[0] : commands[1];
            }
        }
        return null;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

/// <summary>
/// Конвертер для інвертування boolean значення
/// </summary>
public class InverseBooleanConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool boolValue)
        {
            return !boolValue;
        }
        return false;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool boolValue)
        {
            return !boolValue;
        }
        return false;
    }
}

/// <summary>
/// Конвертер DateTime у рядок з коротким форматом
/// </summary>
public class DateTimeToShortStringConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is DateTime dateTime)
        {
            return dateTime.ToString("dd.MM.yyyy HH:mm");
        }
        return string.Empty;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is string stringValue && DateTime.TryParse(stringValue, out DateTime result))
        {
            return result;
        }
        return DateTime.MinValue;
    }
}