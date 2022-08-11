namespace Splines.ViewsModels;

public class ViewModelLocator
{
    public MainViewModel MainViewModel => App.Services.GetRequiredService<MainViewModel>();
}