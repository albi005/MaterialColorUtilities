namespace MaterialColorUtilities.Maui;

public class DynamicColorService : IDynamicColorService
{
    public uint? SeedColor => null;
    public event Action Changed { add { } remove { } }
}