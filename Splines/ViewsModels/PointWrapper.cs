global using Point = Splines.Models.Spline.Point;
using System.Reactive;
using ReactiveUI;

namespace Splines.ViewsModels;

public class PointWrapper : ReactiveObject
{
    private readonly Point _point;

    public double X
    {
        get => _point.X;
        set
        {
            _point.X = value;
            this.RaisePropertyChanged();
        }
    }

    public double Value
    {
        get => _point.Value;
        set
        {
            _point.Value = value;
            this.RaisePropertyChanged();
        }
    }

    public ReactiveCommand<double, Unit>? DeletePoint { get; init; }
    public ReactiveCommand<double, Unit>? InsertPoint { get; init; }

    public PointWrapper(Point point) => _point = point;
}