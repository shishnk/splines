using OxyPlot;
using OxyPlot.Series;

namespace Splines.ViewsModels;

public class MainViewModel : ViewModel
{
    private IDataService _dataService;
    private FiniteElement _selectedElement;
    private CollectionViewSource _selectedElementPoints = new();
    private double _alpha = 1E-07;
    private double _beta = 1E-07;
    public ObservableCollection<Point> Points { get; }
    public ObservableCollection<FiniteElement> Elements { get; }
    public ICollectionView SelectedElementPoints => _selectedElementPoints.View;

    //public FiniteElement SelectedElement
    //{
    //    get => _selectedElement;
    //    set
    //    {
    //        if(!Set(ref _selectedElement, value)) return;

    //        _selectedElementPoints.Source = value?.Points;
    //        OnPropertyChanged(nameof(SelectedGroupStudents));
    //    }
    //}

    public double Alpha { get => _alpha; set => Set(ref _alpha, value); }
    public double Beta { get => _alpha; set => Set(ref _beta, value); }
    public ICommand CreateElement { get; }
    public ICommand DeleteElement { get; }
    public PlotModel Graphic { get; }

    public MainViewModel(IDataService dataService)
    {
        _dataService = dataService;

        var points = Enumerable.Range(0, 100).Select(i => new Point(i, i));

        Points = new(points);

        var elements = Enumerable.Range(0, 100).Select(i => new FiniteElement(i, i));

        Elements = new(elements);

        Graphic = new();
        Graphic.Series.Add(new FunctionSeries(Math.Cos, 0, 10, 0.1, "cos(x)"));

        #region Commands

        CreateElement = new LambdaCommand(OnCreateElementCommandExecuted, CanCreateElementExecute);
        DeleteElement = new LambdaCommand(OnRemoveElementCommandExecuted, CanRemoveElementCommandExecute);

        #endregion
    }

    private void OnCreateElementCommandExecuted(object parameter)
    {
        FiniteElement newElement = new(1.0, 2.0);
        Elements.Add(newElement);
    }

    private bool CanCreateElementExecute(object parameter) => true;

    private void OnRemoveElementCommandExecuted(object parameter)
    {
        if (parameter is not FiniteElement element) return;

        Elements.Remove(element);
    }

    private bool CanRemoveElementCommandExecute(object parameter) =>
        parameter is FiniteElement element && Elements.Contains(element);
}