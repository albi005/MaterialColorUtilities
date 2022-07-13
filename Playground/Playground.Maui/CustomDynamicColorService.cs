using MaterialColorUtilities.Maui;
using MaterialColorUtilities.Palettes;
using Microsoft.Extensions.Options;
using Microsoft.Maui.LifecycleEvents;
using Playground.Shared;

namespace Playground.Maui;

public class CustomDynamicColorService : DynamicColorService<CorePalette, AppScheme<int>, AppScheme<Color>, LightAppSchemeMapper, DarkAppSchemeMapper>
{
    public CustomDynamicColorService(IOptions<DynamicColorOptions> options, IApplication application, ILifecycleEventService lifecycleEventService) : base(options, application, lifecycleEventService)
    {
    }
}
