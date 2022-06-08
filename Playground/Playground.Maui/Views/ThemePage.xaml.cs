using Playground.Maui.ViewModels;

namespace Playground.Maui;

public partial class ThemePage : ContentPage
{
	public ThemePage(ThemeViewModel themeViewModel)
	{
		BindingContext = themeViewModel;
        InitializeComponent();
	}
}