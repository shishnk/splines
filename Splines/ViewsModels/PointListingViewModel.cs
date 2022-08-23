using OxyPlot;
using OxyPlot.Series;

namespace Splines.ViewsModels;

public class PointListingViewModel : ViewModel
{
    private readonly ObservableCollection<PointWrapper> _points;
    public IEnumerable<PointWrapper> Points => _points;
    public ICommand InsertPoint { get; }
    public ICommand DeletePoint { get; }
    
    public PointListingViewModel()
    {
        _points = new() { new(new(0.0, 0.0)) };

        #region Commands

        InsertPoint = new LambdaCommand(OnInsertPointCommandExecuted, CanInsertPointCommandExecute);
        DeletePoint = new LambdaCommand(OnDeletePointCommandExecuted, CanDeletePointCommandExecute);

        #endregion
    }

    private bool CanInsertPointCommandExecute(object parameter) => true;

    private void OnInsertPointCommandExecuted(object parameter) => _points.Add(new PointWrapper(new Point(0.0, 0.0)));

    private bool CanDeletePointCommandExecute(object parameter)
        => _points.Count > 1 && parameter is PointWrapper;

    private void OnDeletePointCommandExecuted(object parameter)
    {
        if (parameter is not PointWrapper point) return;

        _points.Remove(point);
    }
}