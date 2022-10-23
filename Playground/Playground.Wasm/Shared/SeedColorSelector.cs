using Playground.Wasm.Services;
using Microsoft.AspNetCore.Components;

namespace Playground.Wasm.Shared
{
    public abstract class SeedColorSelector : ComponentBase, IDisposable
    {
        [Inject] public ThemeService ThemeService { get; set; }

        protected override void OnInitialized()
        {
            base.OnInitialized();
            ThemeService.Changed += OnThemeChanged;
            OnThemeChanged();
        }

        private void OnThemeChanged()
        {
            SetFromSeed(ThemeService.Seed);
            StateHasChanged();
        }

        protected abstract void SetFromSeed(uint seed);

        public void Dispose()
        {
            ThemeService.Changed -= OnThemeChanged;
        }
    }
}
