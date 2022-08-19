﻿using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using Splines.Views.Windows;

namespace Splines.ViewsModels;

public class MainViewModel : ViewModel
{
    private IDataService _dataService;
    private FiniteElement? _selectedElement;
    private Point? _selectedPoint;
    private PlotModel _graphic;
    private readonly CollectionViewSource _selectedElementPoints = new();
    private double _alpha = 1E-07;
    private double _beta = 1E-07;
    public ObservableCollection<FiniteElement> Elements { get; }
    public ICollectionView SelectedElementPoints => _selectedElementPoints.View;

    public Point? SelectedPoint
    {
        get => _selectedPoint;
        set => Set(ref _selectedPoint, value);
    }

    public FiniteElement? SelectedElement
    {
        get => _selectedElement;
        set
        {
            if (!Set(ref _selectedElement, value)) return;

            _selectedElementPoints.Source = value?.Points;
            OnPropertyChanged(nameof(SelectedElementPoints));
        }
    }

    public double Alpha
    {
        get => _alpha;
        set => Set(ref _alpha, value);
    }

    public double Beta
    {
        get => _alpha;
        set => Set(ref _beta, value);
    }

    public PlotModel Graphic
    {
        get => _graphic;
        set => Set(ref _graphic, value);
    }

    public ICommand CreateElement { get; }
    public ICommand DeleteElement { get; }
    public ICommand InsertPoint { get; }
    public ICommand DeletePoint { get; }
    public ICommand BuildSpline { get; }

    public MainViewModel(IDataService dataService)
    {
        _dataService = dataService;
        Elements = new();

        _graphic = new()
        {
            PlotType = PlotType.XY,
            Background = OxyColors.WhiteSmoke
        };
        
        _graphic.Axes.Add(new LinearAxis { Position = AxisPosition.Bottom, MinorTickSize = 0, MajorGridlineStyle = LineStyle.Solid, MinorGridlineStyle = LineStyle.Solid });
        _graphic.Axes.Add(new LinearAxis { Position = AxisPosition.Left, MinorTickSize = 0, MajorGridlineStyle = LineStyle.Solid, MinorGridlineStyle = LineStyle.Solid });

        #region Commands

        CreateElement = new LambdaCommand(OnCreateElementCommandExecuted, CanCreateElementExecute);
        DeleteElement = new LambdaCommand(OnRemoveElementCommandExecuted, CanRemoveElementCommandExecute);

        InsertPoint = new LambdaCommand(OnInsertPointCommandExecuted, CanInsertPointCommandExecute);
        DeletePoint = new LambdaCommand(OnDeletePointCommandExecuted, CanDeletePointCommandExecute);

        BuildSpline = new LambdaCommand(OnBuildSplineCommandExecuted, CanBuildSplineCommandExecute);

        #endregion
    }

    private void OnCreateElementCommandExecuted(object parameter)
    {
        var dlg = new ElementEditorWindow();
        dlg.ShowDialog();

        if (dlg.DialogResult == true)
        {
            Elements.Add(new(dlg.LeftBorder, dlg.RightBorder)
            {
                Points = new ObservableCollection<Point> { new(0.0, 0.0) }
            });
            MessageBox.Show("Элемент добавлен");
        }
    }

    private bool CanCreateElementExecute(object parameter) => true;

    private void OnRemoveElementCommandExecuted(object parameter)
    {
        if (parameter is not FiniteElement element) return;

        int index = Elements.IndexOf(element);
        Elements.Remove(element);
        SelectedElement = index > 0 ? Elements[index - 1] : Elements.FirstOrDefault();
    }

    private bool CanRemoveElementCommandExecute(object parameter) =>
        parameter is FiniteElement element && Elements.Contains(element);

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
        => Elements.Any(elem => elem.Points?.Count >= 4);

    private void OnBuildSplineCommandExecuted(object parameter)
    {
        Spline spline = Spline.CreateBuilder().SetElements(Elements.ToArray()).SetParameters((_alpha, _beta));
        spline.Compute();
        var series = new LineSeries();
        series.Points.AddRange(spline.GetData().Select(p => new DataPoint(p.X, p.Value)));
        _graphic.Series.Add(series);
    }
}