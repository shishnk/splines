namespace Splines.ViewsModels;

public class PointListingViewModel : ObservableObject
{
    private readonly ObservableCollection<PointWrapper> _points;
    public IEnumerable<PointWrapper> Points => _points;
    public ICommand InsertPoint { get; }
    public ICommand DeletePoint { get; }

    public PointListingViewModel()
    {
        _points = new() { new(new(0.0, 0.0)) };

        #region Commands

        InsertPoint = new RelayCommand(OnInsertPointCommandExecuted);
        DeletePoint = new RelayCommand<PointWrapper>(OnDeletePointCommandExecuted!, CanDeletePointCommandExecute!);

        #endregion
    }

    private void OnInsertPointCommandExecuted()
        => _points.Add(new(new(0.0, 0.0)));

    private bool CanDeletePointCommandExecute(PointWrapper point)
        => _points.Count > 1;

    private void OnDeletePointCommandExecuted(PointWrapper point) => _points.Remove(point);
}