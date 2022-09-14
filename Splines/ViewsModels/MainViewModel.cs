namespace Splines.ViewsModels;

public class MainViewModel : ObservableObject
{
    private PlotModel _graphic;
    private double _alpha = 1E-07;
    private double _beta = 1E-07;
    private int _partitions = 1;
    private ICommand? _buildSpline;
    private ICommand? _drawPointsCommand;
    private ICommand? _clearPlaneCommand;

    public double Alpha
    {
        get => _alpha;
        set => SetProperty(ref _alpha, value);
    }

    public double Beta
    {
        get => _beta;
        set => SetProperty(ref _beta, value);
    }

    public int Partitions
    {
        get => _partitions;
        set => SetProperty(ref _partitions, value);
    }

    public PlotModel Graphic
    {
        get => _graphic;
        set => SetProperty(ref _graphic, value);
    }

    public PointListingViewModel PointListingViewModel { get; }

    public ICommand BuildSplineCommand =>
        _buildSpline ??= new LambdaCommand(OnBuildSplineCommandExecuted, CanBuildSplineCommandExecute);

    public ICommand DrawPointsCommand => _drawPointsCommand ??= new LambdaCommand(OnDrawPointsCommandExecuted);

    public ICommand ClearPlaneCommand => _clearPlaneCommand ??= new LambdaCommand(OnClearPlaneCommandExecuted);

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
    }

    private bool CanBuildSplineCommandExecute()
        => PointListingViewModel.Points.Count() >= 4;

    private void OnBuildSplineCommandExecuted()
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

        _graphic.Axes[0].Minimum = PointListingViewModel.Points.MinBy(p => p.X)!.X - 15.0;
        _graphic.Axes[0].Maximum = PointListingViewModel.Points.MaxBy(p => p.X)!.X + 15.0;
        _graphic.Axes[1].Minimum = PointListingViewModel.Points.MinBy(p => p.Value)!.Value - 15.0;
        _graphic.Axes[1].Maximum = PointListingViewModel.Points.MaxBy(p => p.Value)!.Value + 15.0;

        scatterSeries.Points.AddRange(PointListingViewModel.Points.Select(p => new ScatterPoint(p.X, p.Value)));
        lineSeries.Points.AddRange(spline.Result.Select(p => new DataPoint(p.X, p.Value)));
        _graphic.Series.Add(scatterSeries);
        _graphic.Series.Add(lineSeries);
        _graphic.InvalidatePlot(true);
    }

    private void OnDrawPointsCommandExecuted()
    {
        var series = new ScatterSeries
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

    private void OnClearPlaneCommandExecuted()
    {
        _graphic.Series.Clear();
        _graphic.InvalidatePlot(true);
    }
}