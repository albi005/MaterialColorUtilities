﻿#if ANDROID
using Google.Android.Material.Color;
# endif
using Playground.Maui.ViewModels;

namespace Playground.Maui;

public partial class ThemePage : ContentPage
{
    public ThemePage(ThemeViewModel themeViewModel)
    {
        BindingContext = themeViewModel;
        InitializeComponent();
    }

    private void PrimaryButton_Clicked(object sender, EventArgs e)
    {
#if ANDROID
        // Collect every available Android resource color.
        // Used for checking if one of them is the seed color.
        var attrs = typeof(Android.Resource.Attribute)
            .GetFields()
            .Select(p =>
            {
                try
                {
                    int id = (int)p.GetValue(null);
                    int val = MaterialColors.GetColor(Platform.AppContext, id, "ඞ");
                    return (p.Name, Convert.ToString(val, 16));
                }
                catch
                {
                    return (p.Name, null);
                }
            })
            .Where(x => x.Item2 != null && x.Item2.StartsWith("ff"))
            .Select(x => x.ToString())
            .ToList();
        var colors = typeof(Android.Resource.Color)
            .GetFields()
            .Select(p =>
            {
                try
                {
                    int id = (int)p.GetValue(null);
                    int val = Platform.AppContext.Resources.GetColor(id, null);
                    return (p.Name, Convert.ToString(val, 16));
                }
                catch
                {
                    return (p.Name, null);
                }
            })
            .Where(x => x.Item2 != null && x.Item2.StartsWith("ff"))
            .Select(x => x.ToString())
            .ToList();
#endif
    }
}