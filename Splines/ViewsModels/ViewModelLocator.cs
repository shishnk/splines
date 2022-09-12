namespace Splines.ViewsModels;

public class ViewModelLocator
{
    public MainViewModel MainViewModel => App.Current.Services.GetRequiredService<MainViewModel>();
}