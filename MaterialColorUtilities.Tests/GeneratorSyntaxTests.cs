using MaterialColorUtilities.Schemes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MaterialColorUtilities.Tests;

[TestClass]
public class SyntaxTests
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

public partial class SchemeWithTypeParametersButWithoutTColor<T> : Scheme<int>
{
}

public partial class SchemeWithDifferentTColor<T> : Scheme<T>
{
}

public partial class SchemeWithDifferentTColor2<T> : SchemeWithDifferentTColor<T>
{
}
