namespace MaterialColorUtilities.Maui;

internal class InitializeService : IMauiInitializeService
{
    private readonly DynamicColorService _dynamicColorService;

    public InitializeService(DynamicColorService dynamicColorService)
    {
        _dynamicColorService = dynamicColorService;
    }

    public void Initialize(IServiceProvider services)
    {
        _dynamicColorService.Initialize();
    }
}
