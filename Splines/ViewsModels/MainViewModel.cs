namespace Splines.ViewsModels;

public class MainViewModel : ViewModel
{
    private PlotModel _graphic;
    private double _alpha = 1E-07;
    private double _beta = 1E-07;
    private int _partitions = 1;

    public double Alpha
    {
        get => _alpha;
        set => Set(ref _alpha, value);
    }

    public double Beta
    {
        get => _beta;
        set => Set(ref _beta, value);
    }

    public int Partitions
    {
        get => _partitions;
        set => Set(ref _partitions, value);
    }

    public PlotModel Graphic
    {
        get => _graphic;
        set => Set(ref _graphic, value);
    }

    public PointListingViewModel PointListingViewModel { get; }
    public ICommand BuildSpline { get; }
    public ICommand DrawPoints { get; }
    public ICommand ClearPlane { get; }

    public MainViewModel()
    {
        PointListingViewModel = new();
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

        #region Commands

        BuildSpline = new LambdaCommand(OnBuildSplineCommandExecuted, CanBuildSplineCommandExecute);
        DrawPoints = new LambdaCommand(OnDrawPointsCommandExecuted, CanDrawPointsCommandExecute);
        ClearPlane = new LambdaCommand(OnClearCommandExecuted, CanClearCommandExecute);

        #endregion
    }

    private bool CanBuildSplineCommandExecute(object parameter)
        => PointListingViewModel.Points.Count() >= 4;

    private void OnBuildSplineCommandExecuted(object parameter)
    {
        Spline spline = Spline.CreateBuilder().SetParameters((_alpha, _beta)).SetPartitions(_partitions)
            .SetPoints(PointListingViewModel.Points.Select(p => new Point(p.X, p.Value)).ToArray());
        spline.Compute();
        _graphic.Series.Clear();

        var scatterSeries = new ScatterSeries()
        {
            MarkerType = MarkerType.Circle,
            MarkerSize = 3.0,
            MarkerFill = OxyColors.Black
        };
        var lineSeries = new LineSeries();

        _graphic.Axes[0].Minimum = PointListingViewModel.Points.MinBy(p => p.X)!.X - 20.0;
        _graphic.Axes[0].Maximum = PointListingViewModel.Points.MaxBy(p => p.X)!.X + 20.0;
        _graphic.Axes[1].Minimum = PointListingViewModel.Points.MinBy(p => p.Value)!.Value - 20.0;
        _graphic.Axes[1].Maximum = PointListingViewModel.Points.MaxBy(p => p.Value)!.Value + 20.0;

        scatterSeries.Points.AddRange(PointListingViewModel.Points.Select(p => new ScatterPoint(p.X, p.Value)));
        lineSeries.Points.AddRange(spline.Result.Select(p => new DataPoint(p.X, p.Value)));
        _graphic.Series.Add(scatterSeries);
        _graphic.Series.Add(lineSeries);
        _graphic.InvalidatePlot(true);
    }

    private bool CanDrawPointsCommandExecute(object parameter) => true;

    private void OnDrawPointsCommandExecuted(object parameter)
    {
        var series = new ScatterSeries()
        {
            MarkerType = MarkerType.Circle,
            MarkerSize = 3.0,
            MarkerFill = OxyColors.Black
        };

        series.Points.AddRange(PointListingViewModel.Points.Select(p => new ScatterPoint(p.X, p.Value)));
        _graphic.Series.Clear();
        _graphic.Series.Add(series);
        _graphic.InvalidatePlot(true);
    }

    private bool CanClearCommandExecute(object parameter) => true;

    private void OnClearCommandExecuted(object parameter)
    {
        _graphic.Series.Clear();
        _graphic.InvalidatePlot(true);
    }
}