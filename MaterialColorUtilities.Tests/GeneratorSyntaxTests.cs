using MaterialColorUtilities.Schemes;

namespace MaterialColorUtilities.Tests;

// If this builds -> the return type is good -> the source generator is working
public class GeneratorSyntaxExamples
{
    // Has type parameters, but no TColor
    public void TypeParametersWithoutTColor()
    {
        SchemeWithTypeParametersButWithoutTColor<object> scheme = new();
        Scheme<double> converted = scheme.Convert(i => (double)i);
    }

    public void AdditionalTypeParameters()
    {
        MyScheme3<object, int> scheme = new();
        MyScheme3<object, double> converted = scheme.Convert(i => (double)i);
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

// Has an additional type parameter
public partial class MyScheme3<TWhatever, T> : MyScheme<T>
{
    public T MyColor { get; set; }
}