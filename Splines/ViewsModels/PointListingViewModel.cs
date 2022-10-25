namespace Splines.ViewsModels;

public class PointListingViewModel : ObservableObject
{
    private readonly ObservableCollection<PointWrapper> _points;
    private ICommand? _deletePointCommand;
    private ICommand? _insertPointCommand;
    public IEnumerable<PointWrapper> Points => _points;

    public ICommand DeletePointCommand => _deletePointCommand ??=
        new LambdaCommand(OnDeletePointCommandExecuted, _ => CanDeletePointCommandExecute());

    public ICommand InsertPointCommand => _insertPointCommand ??=
        new LambdaCommand(OnInsertPointCommandExecuted);

    public PointListingViewModel() => _points = new()
    {
        new(new(0.0, 0.0))
        {
            DeletePointCommand = DeletePointCommand,
            InsertPointCommand = InsertPointCommand
        }
    };

    private void OnInsertPointCommandExecuted() => _points.Add(new(new(0.0, 0.0))
    {
        DeletePointCommand = DeletePointCommand,
        InsertPointCommand = InsertPointCommand
    });

    private bool CanDeletePointCommandExecute() => _points.Count > 1;

    private void OnDeletePointCommandExecuted(object parameter)
    {
        if (parameter is not PointWrapper point) return;
        _points.Remove(point);
    }
}