using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using WordleGameEngine.Enums;
using WordleApp.Helpers;

namespace WordleApp.Converters;

/// <summary>
/// Конвертер bool у Visibility
/// </summary>
public class BoolToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool boolValue)
        {
            // Check for inverted parameter
            bool inverted = parameter?.ToString()?.ToLower() == "inverted";
            
            if (inverted)
                boolValue = !boolValue;
                
            return boolValue ? Visibility.Visible : Visibility.Collapsed;
        }
        return Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is Visibility visibility)
        {
            bool result = visibility == Visibility.Visible;
            
            // Check for inverted parameter
            bool inverted = parameter?.ToString()?.ToLower() == "inverted";
            if (inverted)
                result = !result;
                
            return result;
        }
        return false;
    }
}

/// <summary>
/// Конвертер string у Visibility
/// </summary>
public class StringToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is string stringValue)
        {
            return string.IsNullOrWhiteSpace(stringValue) ? Visibility.Collapsed : Visibility.Visible;
        }
        return Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

/// <summary>
/// Конвертер GuessResult у колір границі
/// </summary>
public class GuessResultToBorderColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is GuessResult result)
        {
            return result switch
            {
                GuessResult.Correct => new SolidColorBrush(ColorHelper.CorrectColor),
                GuessResult.Present => new SolidColorBrush(ColorHelper.PresentColor),
                GuessResult.Absent when parameter?.ToString() == "HasLetter" => new SolidColorBrush(ColorHelper.BorderColor),
                GuessResult.Absent => new SolidColorBrush(ColorHelper.DefaultColor),
                _ => new SolidColorBrush(ColorHelper.BorderColor)
            };
        }
        return new SolidColorBrush(ColorHelper.BorderColor);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

/// <summary>
/// Конвертер для визначення видимості на основі GuessResult
/// </summary>
public class GuessResultToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is GuessResult result)
        {
            string param = parameter?.ToString() ?? "";
            
            return param.ToLower() switch
            {
                "correct" => result == GuessResult.Correct ? Visibility.Visible : Visibility.Collapsed,
                "present" => result == GuessResult.Present ? Visibility.Visible : Visibility.Collapsed,
                "absent" => result == GuessResult.Absent ? Visibility.Visible : Visibility.Collapsed,
                "notabsent" => result != GuessResult.Absent ? Visibility.Visible : Visibility.Collapsed,
                _ => Visibility.Visible
            };
        }
        return Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

/// <summary>
/// Конвертер для перетворення bool у команду
/// </summary>
public class BoolToCommandNameConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool boolValue && parameter is string commandPair)
        {
            var commands = commandPair.Split('|');
            if (commands.Length == 2)
            {
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
/// Конвертер для множення числових значень
/// </summary>
public class MultiplyConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is double doubleValue && parameter is string paramString)
        {
            if (double.TryParse(paramString, out double multiplier))
            {
                return doubleValue * multiplier;
            }
        }
        return value;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is double doubleValue && parameter is string paramString)
        {
            if (double.TryParse(paramString, out double multiplier) && multiplier != 0)
            {
                return doubleValue / multiplier;
            }
        }
        return value;
    }
}

/// <summary>
/// Конвертер для форматування тексту з параметрами
/// </summary>
public class StringFormatConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (parameter is string format && value != null)
        {
            try
            {
                return string.Format(format, value);
            }
            catch
            {
                return value.ToString();
            }
        }
        return value?.ToString() ?? string.Empty;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

/// <summary>
/// Конвертер для перевірки null значень
/// </summary>
public class NullToBoolConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        bool inverted = parameter?.ToString()?.ToLower() == "inverted";
        bool isNull = value == null;
        
        return inverted ? isNull : !isNull;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}