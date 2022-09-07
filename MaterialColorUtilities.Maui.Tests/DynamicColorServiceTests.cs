using Microsoft.Extensions.Options;

namespace MaterialColorUtilities.Maui.Tests;

[TestClass]
public class DynamicColorServiceTests
{
    private readonly IAccentColorService _accentColorService = new MockAccentColorService();
    private readonly Application _application = new();
    private readonly IPreferences _preferences = new MockPreferences();
    
    [TestMethod]
    public void EnableTheming_False()
    {
        IOptions<DynamicColorOptions> options = CreateOptions(opt =>
        {
            opt.EnableTheming = false;
        });

        DynamicColorService dynamicColorService = new(options, _accentColorService, _application, _preferences);
        
        dynamicColorService.Initialize(null);
        
        Assert.IsFalse(dynamicColorService.EnableTheming);
        Assert.IsNull(dynamicColorService.CorePalette);
        Assert.IsNull(dynamicColorService.SchemeMaui);
        Assert.IsTrue(_application.Resources.Count == 0);
    }

    [TestMethod]
    public void EnableDynamicColor_False()
    {
        IOptions<DynamicColorOptions> options = CreateOptions(opt =>
        {
            opt.EnableDynamicColor = false;
        });

        DynamicColorService dynamicColorService = new(options, _accentColorService, _application, _preferences);
        
        dynamicColorService.Initialize(null);
        
        Assert.IsTrue(dynamicColorService.EnableTheming);
        Assert.IsFalse(dynamicColorService.EnableDynamicColor);
        Assert.AreEqual(unchecked((int)0xff4285F4), dynamicColorService.Seed);
        Assert.AreEqual(unchecked((int)0xff005AC1), dynamicColorService.SchemeInt.Primary);
        Assert.AreEqual(dynamicColorService.SchemeInt.Primary, dynamicColorService.SchemeMaui.Primary.ToInt());
        Assert.IsNotNull(_application.Resources[Schemes.Keys.Primary]);
    }

    private static IOptions<DynamicColorOptions> CreateOptions(Action<DynamicColorOptions> configure)
    {
        DynamicColorOptions options = new();
        configure(options);
        return Options.Create(options);
    }
}