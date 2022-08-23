namespace Splines.Infrastructure.Converters;

[ValueConversion(typeof(bool), typeof(Visibility))]
public class BooleanToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        bool visibility = false;
        if (value is bool)
        {
            visibility = (bool)value;
        }

        return visibility ? Visibility.Visible : Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        Visibility visibility = (Visibility)value;

        return (visibility == Visibility.Visible);
    }
}