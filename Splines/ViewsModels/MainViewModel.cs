using System.Reactive;
using System.Reactive.Linq;
using ReactiveUI;

namespace Splines.ViewsModels;

public class MainViewModel : ReactiveObject
{
    private PlotModel _graphic;
    private readonly Spline _spline;
    private double _alpha = 1E-07;
    private double _beta = 1E-07;
    private int _partitions = 1;

    public double Alpha
    {
        get => _alpha;
        set => this.RaiseAndSetIfChanged(ref _alpha, value);
    }

    public double Beta
    {
        get => _beta;
        set => this.RaiseAndSetIfChanged(ref _beta, value);
    }

    public int Partitions
    {
        get => _partitions;
        set => this.RaiseAndSetIfChanged(ref _partitions, value);
    }

    public PlotModel Graphic
    {
        get => _graphic;
        set => this.RaiseAndSetIfChanged(ref _graphic, value);
    }

    public PointListingViewModel PointListingViewModel { get; }
    public ReactiveCommand<Unit, Unit> BuildSpline { get; }
    public ReactiveCommand<Unit, Unit> DrawPoints { get; }
    public ReactiveCommand<Unit, Unit> ClearPlane { get; }

    public MainViewModel(PointListingViewModel pointListingViewModel)
    {
        PointListingViewModel = pointListingViewModel;
        _spline = new();
        _graphic = new()
        {
            PlotType = PlotType.XY,
            Background = OxyColors.WhiteSmoke
        };

        _graphic.Axes.Add(new LinearAxis
        {
            Position = AxisPosition.Bottom, MinorTickSize = 0, MajorGridlineStyle = LineStyle.Solid,
            MinorGridlineStyle = LineStyle.Solid, Minimum = -50, Maximum = 50
        });
        _graphic.Axes.Add(new LinearAxis
        {
            Position = AxisPosition.Left, MinorTickSize = 0, MajorGridlineStyle = LineStyle.Solid,
            MinorGridlineStyle = LineStyle.Solid, Minimum = -50, Maximum = 50
        });

        #region Commands and subscriptions

        DrawPoints = ReactiveCommand.Create<Unit>(_ =>
        {
            var series = new ScatterSeries
            {
                MarkerType = MarkerType.Circle,
                MarkerSize = 3.0,
                MarkerFill = OxyColors.Black
            };

            series.Points.AddRange
                (PointListingViewModel.Points.Select(p => new ScatterPoint(p.X, p.Value)));
            _graphic.Series.Clear();
            _graphic.Series.Add(series);
            _graphic.InvalidatePlot(true);
        });
        ClearPlane = ReactiveCommand.Create<Unit>(_ =>
        {
            _graphic.Series.Clear();
            _graphic.InvalidatePlot(true);
        });
        var canExecute = PointListingViewModel.PointsAsSourceCache.CountChanged.Select(c => c >= 4);
        BuildSpline = ReactiveCommand.Create(BuildSplineImpl, canExecute);

        #endregion
    }

    private void BuildSplineImpl()
    {
        _spline.Parameters = (_alpha, _beta);
        _spline.Partitions = _partitions;
        _spline.Points = PointListingViewModel.Points.Select(p => new Point(p.X, p.Value)).ToArray();

        _spline.Compute();
        _graphic.Series.Clear();

        var scatterSeries = new ScatterSeries
        {
            MarkerType = MarkerType.Circle,
            MarkerSize = 3.0,
            MarkerFill = OxyColors.Black
        };
        var lineSeries = new LineSeries();

        _graphic.Axes[0].Minimum = PointListingViewModel.Points.MinBy(p => p.X)!.X - 15.0;
        _graphic.Axes[0].Maximum = PointListingViewModel.Points.MaxBy(p => p.X)!.X + 15.0;
        _graphic.Axes[1].Minimum = PointListingViewModel.Points.MinBy(p => p.Value)!.Value - 15.0;
        _graphic.Axes[1].Maximum = PointListingViewModel.Points.MaxBy(p => p.Value)!.Value + 15.0;

        scatterSeries.Points.AddRange(PointListingViewModel.Points.Select(p => new ScatterPoint(p.X, p.Value)));
        lineSeries.Points.AddRange(_spline.Result.Select(p => new DataPoint(p.X, p.Value)));
        _graphic.Series.Add(scatterSeries);
        _graphic.Series.Add(lineSeries);
        _graphic.InvalidatePlot(true);
    }
}