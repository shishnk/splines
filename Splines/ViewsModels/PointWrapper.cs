global using Point = Splines.Models.Spline.Point;

namespace Splines.ViewsModels;

public class PointWrapper : ObservableObject
{
    private readonly Point _point;

    public double X
    {
        get => _point.X;
        set => SetProperty(_point.X, value, _point, (point, x) => point.X = x);
    }

    public double Value
    {
        get => _point.Value;
        set => SetProperty(_point.Value, value, _point, (point, fx) => point.Value = fx);
    }

    public PointWrapper(Point point) => _point = point;
}