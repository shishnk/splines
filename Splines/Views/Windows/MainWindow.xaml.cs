namespace Splines.Views.Windows;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow
{
    public MainWindow() => InitializeComponent();

    private void ElementsCollectionOnFilter(object sender, FilterEventArgs e)
    {
        if (e.Item is not FiniteElement elements) return;

        var filterText = ElementFilterText.Text;

        if (filterText.Length == 0) return;

        if (elements.ToString().Contains(filterText, StringComparison.OrdinalIgnoreCase)) return;

        e.Accepted = false;
    }

    private void OnElementsFilterChanged(object sender, TextChangedEventArgs e)
    {
        var textBox = (TextBox)sender;
        var collection = (CollectionViewSource)textBox.FindResource("ElementsCollection");
        collection.View.Refresh();
    }
}