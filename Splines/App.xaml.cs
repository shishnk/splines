namespace Splines;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App
{
    private static IHost? _host;
    public static IHost Host => _host ??= Program.CreateHostBuilder(Environment.GetCommandLineArgs()).Build();
    public static IServiceProvider Services => Host.Services;

    public static void ConfigureServices(IServiceCollection services)
        => services.AddViewModelsAndServices();

    protected override async void OnStartup(StartupEventArgs e)
    {
        var host = Host;

        base.OnStartup(e);
        await host.StartAsync();
    }

    protected override async void OnExit(ExitEventArgs e)
    {
        base.OnExit(e);

        using (Host) await Host.StopAsync();
    }
}