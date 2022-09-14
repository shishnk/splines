namespace Splines.Infrastructure.Commands.Base;

#nullable disable
public abstract class Command : ICommand
{
    public event EventHandler CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }

    private bool _executable = true;

    public bool Executable
    {
        get => _executable;
        set
        {
            if (_executable == value) return;
            _executable = value;
            CommandManager.InvalidateRequerySuggested();
        }
    }

    bool ICommand.CanExecute(object parameter) => _executable && CanExecute(parameter);

    void ICommand.Execute(object parameter)
    {
        if (CanExecute(parameter)) Execute(parameter);
    }

    protected virtual bool CanExecute(object parameter) => true;

    protected abstract void Execute(object parameter);
}

#nullable restore