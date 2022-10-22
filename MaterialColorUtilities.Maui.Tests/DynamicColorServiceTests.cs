using Microsoft.Extensions.Options;

namespace MaterialColorUtilities.Maui.Tests;

[TestClass]
public class DynamicColorServiceTests
{
    private readonly ISeedColorService _seedColorService = new MockSeedColorService();
    private readonly Application _application = new();
    private readonly IPreferences _preferences = new MockPreferences();
    
    [TestMethod]
    public void DisableTheming()
    {
        IOptions<DynamicColorOptions> options = CreateOptions(opt =>
        {
            opt.EnableTheming = false;
        });

        DynamicColorService dynamicColorService = new(options, _seedColorService, _application, _preferences);
        
        dynamicColorService.Initialize(null);
        
        Assert.IsFalse(dynamicColorService.EnableTheming);
        Assert.IsNull(dynamicColorService.CorePalette);
        Assert.IsNull(dynamicColorService.SchemeMaui);
        Assert.IsTrue(_application.Resources.Count == 0);
    }

    [TestMethod]
    public void DisableDynamicColor()
    {
        IOptions<DynamicColorOptions> options = CreateOptions(opt =>
        {
            opt.EnableDynamicColor = false;
        });

        DynamicColorService dynamicColorService = new(options, _seedColorService, _application, _preferences);
        
        dynamicColorService.Initialize(null);
        
        Assert.IsTrue(dynamicColorService.EnableTheming);
        Assert.IsFalse(dynamicColorService.EnableDynamicColor);
        Assert.AreEqual(0xff4285F4, dynamicColorService.Seed);
        Assert.AreEqual(0xff005AC1, dynamicColorService.SchemeInt.Primary);
        Assert.AreEqual(dynamicColorService.SchemeInt.Primary, dynamicColorService.SchemeMaui.Primary.ToUint());
        Assert.IsNotNull(_application.Resources[Schemes.Keys.Primary]);
    }

    [TestMethod]
    public void EnableDynamicColor_SeedColorNull()
    {
        IOptions<DynamicColorOptions> options = CreateOptions();

        DynamicColorService dynamicColorService = new(options, _seedColorService, _application, _preferences);
        
        dynamicColorService.Initialize(null);
        
        Assert.IsTrue(dynamicColorService.EnableTheming);
        Assert.IsTrue(dynamicColorService.EnableDynamicColor);
        Assert.AreEqual(0xff4285F4, dynamicColorService.Seed);
        Assert.AreEqual(0xff005AC1, dynamicColorService.SchemeInt.Primary);
        Assert.AreEqual(dynamicColorService.SchemeInt.Primary, dynamicColorService.SchemeMaui.Primary.ToUint());
        Assert.IsNotNull(_application.Resources[Schemes.Keys.Primary]);
    }

    [TestMethod]
    public void EnableDynamicColor_SeedColorAvailable()
    {
        IOptions<DynamicColorOptions> options = CreateOptions();

        ISeedColorService seedColorService = new MockSeedColorService(0xFFc07d52);
        DynamicColorService dynamicColorService = new(options, seedColorService, _application, _preferences);
        
        dynamicColorService.Initialize(null);
        
        Assert.AreEqual(0xFFc07d52, dynamicColorService.Seed);
        Assert.AreEqual(0xFF96490A, dynamicColorService.SchemeInt.Primary);
        Assert.AreEqual(dynamicColorService.SchemeInt.Primary, dynamicColorService.SchemeMaui.Primary.ToUint());
        Assert.IsNotNull(_application.Resources[Schemes.Keys.Primary]);
    }

    private static IOptions<DynamicColorOptions> CreateOptions()
    {
        DynamicColorOptions options = new();
        return Options.Create(options);
    }

    private static IOptions<DynamicColorOptions> CreateOptions(Action<DynamicColorOptions> configure)
    {
        DynamicColorOptions options = new();
        configure(options);
        return Options.Create(options);
    }
}