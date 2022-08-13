namespace Splines.Infrastructure.Converters;

[ValueConversion(typeof(FiniteElement), typeof(string))]
public class FiniteElementToString : IValueConverter
{
    public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not FiniteElement element) return null;

        return $"Element [{element.LeftBorder}; {element.RightBorder}]";
    }

    public object? ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not string str) return null;

        var parameters = str.Split(";");

        return new FiniteElement(double.Parse(parameters[0].Split('[')[1]), 
            double.Parse(parameters[1].Split(']')[0]));
    }
}