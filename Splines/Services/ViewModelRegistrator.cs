namespace Splines.Services;

public static class ViewModelRegistrator
{
    public static IServiceCollection AddViewModels(this IServiceCollection services)
        => services.AddSingleton<MainViewModel>();
}