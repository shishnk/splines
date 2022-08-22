namespace Splines.Services;

public static class Registrator
{
    public static void AddViewModelsAndServices(this IServiceCollection services)
        => services.AddSingleton<MainViewModel>().AddSingleton<IDataService, Spline>().AddSingleton<PointListingViewModel>();
}