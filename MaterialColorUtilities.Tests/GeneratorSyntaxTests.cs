using MaterialColorUtilities.Schemes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MaterialColorUtilities.Tests;

[TestClass]
public class GeneratorSyntaxTests
{
    [TestMethod]
    // Has type parameters, but no TColor
    public void TypeParametersWithoutTColor()
    {
        SchemeWithTypeParametersButWithoutTColor<object> scheme = new();
        var converted = scheme.ConvertTo(i => (double)i);
        Assert.IsInstanceOfType(converted, typeof(Scheme<double>));
    }
}


// TColor has a different name
public partial class MyScheme<T> : Scheme<T>
{
}

// Spread across multiple declarations
// > Only generate one
public partial class MyScheme2<T> : MyScheme<T>
{
    public T Color1 { get; set; }
}
public partial class MyScheme2<T> : MyScheme<T>
{
    public T Color2 { get; set; }
}

// Default TColor
// > Don't generate
public partial class MyScheme2 : MyScheme2<int>
{
}

// Has type parameters, but none of them change TColor
// > Don't generate
public partial class SchemeWithTypeParametersButWithoutTColor<T> : Scheme<int>
{
}
