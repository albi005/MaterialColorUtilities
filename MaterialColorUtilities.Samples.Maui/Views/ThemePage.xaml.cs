using MaterialColorUtilities.Samples.Maui.Services;
using MaterialColorUtilities.Samples.Maui.ViewModels;

namespace MaterialColorUtilities.Samples.Maui;

public partial class ThemePage : ContentPage
{
	public ThemePage(ThemeViewModel themeViewModel)
	{
		BindingContext = themeViewModel;
        InitializeComponent();
	}
}