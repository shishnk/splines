namespace Splines.Views.Windows;

public partial class ElementEditorWindow
{
    public static readonly DependencyProperty LeftBorderProperty = DependencyProperty
        .Register(
            nameof(LeftBorder),
            typeof(double),
            typeof(ElementEditorWindow),
            new PropertyMetadata(default(double)));

    public static readonly DependencyProperty RightBorderProperty = DependencyProperty
        .Register(
            nameof(RightBorder),
            typeof(double),
            typeof(ElementEditorWindow),
            new PropertyMetadata(default(double)));

    public double LeftBorder
    {
        get => (double)GetValue(LeftBorderProperty);
        set => SetValue(LeftBorderProperty, value);
    }

    public double RightBorder
    {
        get => (double)GetValue(RightBorderProperty);
        set => SetValue(RightBorderProperty, value);
    }

    public ElementEditorWindow() => InitializeComponent();
}