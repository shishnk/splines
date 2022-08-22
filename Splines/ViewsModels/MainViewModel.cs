using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using Splines.Views.Windows;

namespace Splines.ViewsModels;

public class MainViewModel : ViewModel
{
    private IDataService _dataService;
    private FiniteElement? _selectedElement;
    private PlotModel _graphic;
    private Point? _selectedPoint;
    private double _alpha = 1E-07;
    private double _beta = 1E-07;
    public Point? SelectedPoint
    {
        get => _selectedPoint;
        set => Set(ref _selectedPoint, value);
    }
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
    public ICommand InsertPoint { get; }
    public ICommand DeletePoint { get; }
    public PlotModel Graphic
    {
        get => _graphic;
        set => Set(ref _graphic, value);
    }

    public MainViewModel(IDataService dataService)
    {
        _dataService = dataService;
        _graphic = new()
        {
            PlotType = PlotType.XY,
            Background = OxyColors.WhiteSmoke
        };
        
        _graphic.Axes.Add(new LinearAxis { Position = AxisPosition.Bottom, MinorTickSize = 0, MajorGridlineStyle = LineStyle.Solid, MinorGridlineStyle = LineStyle.Solid });
        _graphic.Axes.Add(new LinearAxis { Position = AxisPosition.Left, MinorTickSize = 0, MajorGridlineStyle = LineStyle.Solid, MinorGridlineStyle = LineStyle.Solid });

        #region Commands

        InsertPoint = new LambdaCommand(OnInsertPointCommandExecuted, CanInsertPointCommandExecute);
        DeletePoint = new LambdaCommand(OnDeletePointCommandExecuted, CanDeletePointCommandExecute);

        #endregion
    }
    
    private bool CanInsertPointCommandExecute(object parameter) => true;

    private void OnInsertPointCommandExecuted(object parameter)
    {
        _selectedElement?.Points?.Add(new(0.0, 0.0));
    }

    private bool CanDeletePointCommandExecute(object parameter) =>
        parameter is Point point && _selectedElement?.Points?.Contains(point) == true;

    private void OnDeletePointCommandExecuted(object parameter)
    {
        if (parameter is not Point point) return;

        if (_selectedElement?.Points?.Count >= 2)
        {
            _selectedElement?.Points?.Remove(point);
        }
    }

    private bool CanBuildSplineCommandExecute(object parameter)
        => true;

    private void OnBuildSplineCommandExecuted(object parameter)
    {
        // Spline spline = Spline.CreateBuilder().SetElements(Elements.ToArray()).SetParameters((_alpha, _beta));
        // spline.Compute();
        // var series = new LineSeries();
        // series.Points.AddRange(spline.GetData().Select(p => new DataPoint(p.X, p.Value)));
        // _graphic.Series.Add(series);
    }
}