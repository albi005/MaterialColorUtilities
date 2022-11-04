namespace MaterialColorUtilities.Maui;

public class MaterialColorResourceDictionary : ResourceDictionary
{
    public MaterialColorResourceDictionary()
    {
        IMaterialColorService.Current.Initialize(this);
    }
}
