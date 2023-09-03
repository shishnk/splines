global using Point = Splines.Models.Spline.Point;
using System.Reactive;
using ReactiveUI;

namespace Splines.ViewsModels;

public class PointWrapper(Point point) : ReactiveObject
{
    public double X
    {
        get => point.X;
        set
        {
            point.X = value;
            this.RaisePropertyChanged();
        }
    }

    public double Value
    {
        get => point.Value;
        set
        {
            point.Value = value;
            this.RaisePropertyChanged();
        }
    }

    public required ReactiveCommand<double, Unit>? DeletePoint { get; init; }
    public required ReactiveCommand<double, Unit>? InsertPoint { get; init; }
}