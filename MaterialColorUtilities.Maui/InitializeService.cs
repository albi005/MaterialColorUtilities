namespace MaterialColorUtilities.Maui;

internal class InitializeService<TDynamicColorService> : IMauiInitializeService
    where TDynamicColorService : IDynamicColorService
{
    private readonly TDynamicColorService _dynamicColorService;

    public InitializeService(TDynamicColorService dynamicColorService)
    {
        _dynamicColorService = dynamicColorService;
    }

    public void Initialize(IServiceProvider services)
    {
        _dynamicColorService.Initialize();
    }
}
