using System.Windows.Media;
using WordleGameEngine.Enums;

namespace WordleApp.Helpers;

public static class ColorHelper
{
    public static readonly Color CorrectColor = Color.FromRgb(106, 170, 100);      
    public static readonly Color PresentColor = Color.FromRgb(201, 180, 88);     
    public static readonly Color AbsentColor = Color.FromRgb(120, 124, 126);       
    public static readonly Color DefaultColor = Color.FromRgb(211, 214, 218);      
    public static readonly Color BorderColor = Color.FromRgb(135, 138, 140);       
    
    public static Color GetColorFromGuessResult(GuessResult result)
    {
        return result switch
        {
            GuessResult.Correct => CorrectColor,
            GuessResult.Present => PresentColor,
            GuessResult.Absent => AbsentColor,
            _ => DefaultColor
        };
    }

    public static SolidColorBrush GetBrushFromGuessResult(GuessResult result)
    {
        return new SolidColorBrush(GetColorFromGuessResult(result));
    }

    public static Color GetTextColorFromGuessResult(GuessResult result)
    {
        return result switch
        {
            GuessResult.Correct => Colors.White,
            GuessResult.Present => Colors.White,
            GuessResult.Absent => Colors.White,
            _ => Colors.Black
        };
    }

    public static SolidColorBrush GetTextBrushFromGuessResult(GuessResult result)
    {
        return new SolidColorBrush(GetTextColorFromGuessResult(result));
    }

    public static Color FromRgb(byte red, byte green, byte blue)
    {
        return Color.FromRgb(red, green, blue);
    }

    public static SolidColorBrush FromRgbToBrush(byte red, byte green, byte blue)
    {
        return new SolidColorBrush(Color.FromRgb(red, green, blue));
    }

    public static Color GetTransparentColor(Color color, byte alpha)
    {
        return Color.FromArgb(alpha, color.R, color.G, color.B);
    }

    public static SolidColorBrush GetTransparentBrush(Color color, byte alpha)
    {
        return new SolidColorBrush(GetTransparentColor(color, alpha));
    }
    public static Color BlendColors(Color color1, Color color2, double ratio)
    {
        ratio = Math.Max(0, Math.Min(1, ratio)); 
        
        byte r = (byte)(color1.R * (1 - ratio) + color2.R * ratio);
        byte g = (byte)(color1.G * (1 - ratio) + color2.G * ratio);
        byte b = (byte)(color1.B * (1 - ratio) + color2.B * ratio);
        
        return Color.FromRgb(r, g, b);
    }

    public static bool IsDarkColor(Color color)
    {
        double brightness = (0.299 * color.R + 0.587 * color.G + 0.114 * color.B) / 255;
        return brightness < 0.5;
    }

    public static Color GetContrastColor(Color backgroundColor)
    {
        return IsDarkColor(backgroundColor) ? Colors.White : Colors.Black;
    }
}