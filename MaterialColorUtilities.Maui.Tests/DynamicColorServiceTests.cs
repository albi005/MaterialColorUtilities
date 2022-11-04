using Microsoft.Extensions.Options;

namespace MaterialColorUtilities.Maui.Tests;

[TestClass]
public class DynamicColorServiceTests
{
    private readonly IDynamicColorService _dynamicColorService = new MockDynamicColorService();
    private readonly IPreferences _preferences = new MockPreferences();

    public DynamicColorServiceTests()
    {
        _ = new Application();
    }
    
    [TestMethod]
    public void DisableTheming()
    {
        IOptions<MaterialColorOptions> options = CreateOptions(opt =>
        {
            opt.EnableTheming = false;
        });
        MaterialColorService materialColorService = new(options, _dynamicColorService, _preferences);
        ResourceDictionary resourceDictionary = new();
        
        materialColorService.Initialize(resourceDictionary);
        
        Assert.IsFalse(materialColorService.EnableTheming);
        Assert.IsNull(materialColorService.SchemeMaui);
        Assert.IsTrue(resourceDictionary.Count == 0);
    }

    [TestMethod]
    public void DisableDynamicColor()
    {
        IOptions<MaterialColorOptions> options = CreateOptions(opt =>
        {
            opt.EnableDynamicColor = false;
        });
        MaterialColorService materialColorService = new(options, _dynamicColorService, _preferences);
        ResourceDictionary resourceDictionary = new();
        
        materialColorService.Initialize(resourceDictionary);
        
        Assert.IsTrue(materialColorService.EnableTheming);
        Assert.IsFalse(materialColorService.EnableDynamicColor);
        Assert.AreEqual(0xff4285F4, materialColorService.Seed);
        Assert.AreEqual(0xff445e91, materialColorService.SchemeInt.Primary);
        Assert.AreEqual(materialColorService.SchemeInt.Primary, materialColorService.SchemeMaui.Primary.ToUint());
        Assert.IsNotNull(resourceDictionary[Schemes.Keys.Primary]);
    }

    [TestMethod]
    public void EnableDynamicColor_SeedColorNull()
    {
        IOptions<MaterialColorOptions> options = CreateOptions();
        MaterialColorService materialColorService = new(options, _dynamicColorService, _preferences);
        ResourceDictionary resourceDictionary = new();
        
        materialColorService.Initialize(resourceDictionary);
        
        Assert.IsTrue(materialColorService.EnableTheming);
        Assert.IsTrue(materialColorService.EnableDynamicColor);
        Assert.AreEqual(0xff4285F4, materialColorService.Seed);
        Assert.AreEqual(0xff445e91, materialColorService.SchemeInt.Primary);
        Assert.AreEqual(materialColorService.SchemeInt.Primary, materialColorService.SchemeMaui.Primary.ToUint());
        Assert.IsNotNull(resourceDictionary[Schemes.Keys.Primary]);
    }

    [TestMethod]
    public void EnableDynamicColor_SeedColorAvailable()
    {
        IOptions<MaterialColorOptions> options = CreateOptions();
        IDynamicColorService dynamicColorService = new MockDynamicColorService(0xFFc07d52);
        MaterialColorService materialColorService = new(options, dynamicColorService, _preferences);
        ResourceDictionary resourceDictionary = new();
        
        materialColorService.Initialize(resourceDictionary);
        
        Assert.AreEqual(0xffc07d52, materialColorService.Seed);
        Assert.AreEqual(0xff8b4f26, materialColorService.SchemeInt.Primary);
        Assert.AreEqual(materialColorService.SchemeInt.Primary, materialColorService.SchemeMaui.Primary.ToUint());
        Assert.IsNotNull(resourceDictionary[Schemes.Keys.Primary]);
    }

    private static IOptions<MaterialColorOptions> CreateOptions()
    {
        MaterialColorOptions options = new();
        return Options.Create(options);
    }

    private static IOptions<MaterialColorOptions> CreateOptions(Action<MaterialColorOptions> configure)
    {
        MaterialColorOptions options = new();
        configure(options);
        return Options.Create(options);
    }
}