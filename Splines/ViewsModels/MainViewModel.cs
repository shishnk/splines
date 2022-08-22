using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using Splines.Views.Windows;

namespace Splines.ViewsModels;

public class MainViewModel : ViewModel
{
    private IDataService _dataService;
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

    public MainViewModel(IDataService dataService)
    {
        PointListingViewModel = new();
        _dataService = dataService;
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

        #endregion
    }

    private bool CanBuildSplineCommandExecute(object parameter)
        => PointListingViewModel.Points.Count() >= 4;

    private void OnBuildSplineCommandExecuted(object parameter)
    {
        // Spline spline = Spline.CreateBuilder().SetElements(Elements.ToArray()).SetParameters((_alpha, _beta));
        // spline.Compute();
        // var series = new LineSeries();
        // series.Points.AddRange(spline.GetData().Select(p => new DataPoint(p.X, p.Value)));
        // _graphic.Series.Add(series);
    }
}