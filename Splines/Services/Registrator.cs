namespace Splines.Services;

public static class Registrator
{
    public static IServiceCollection AddViewModelsAndServices(this IServiceCollection services)
        => services.AddSingleton<MainViewModel>().AddSingleton<IDataService, Spline>();
}