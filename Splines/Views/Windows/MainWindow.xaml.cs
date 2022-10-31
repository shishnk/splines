using ReactiveUI;

namespace Splines.Views.Windows;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : IViewFor<MainViewModel>
{
    public MainWindow()
    {
        InitializeComponent();
        ViewModel = App.Current.Services.GetRequiredService<MainViewModel>();
        this.WhenAnyValue(x => x.ViewModel.Alpha)
            .BindTo(this, view => view.Box.Text);
    }

    object? IViewFor.ViewModel
    {
        get => ViewModel;
        set => throw new NotImplementedException();
    }

    public MainViewModel ViewModel { get; set; }
}