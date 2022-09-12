global using Point = Splines.Models.Spline.Point;

namespace Splines.ViewsModels;

public class PointWrapper : ViewModel
{
    private Point _point;

    public double X
    {
        get => _point.X;
        set
        {
            _point.X = value;
            OnPropertyChanged();
        }
    }

    public double Value
    {
        get => _point.Value;
        set
        {
            _point.Value = value;
            OnPropertyChanged();
        }
    }

    public PointWrapper(Point point) => _point = point;
}