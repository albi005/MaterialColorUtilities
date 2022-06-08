using MaterialColorUtilities.Samples.Wasm.Services;
using Microsoft.AspNetCore.Components;

namespace MaterialColorUtilities.Samples.Wasm.Shared
{
    public abstract class SeedColorSelector : ComponentBase, IDisposable
    {
        [Inject] public ThemeService ThemeService { get; set; }

        protected override void OnInitialized()
        {
            base.OnInitialized();
            ThemeService.ThemeChanged += OnSeedChanged;
            OnSeedChanged(null, null);
        }

        private void OnSeedChanged(object sender, EventArgs e)
        {
            SetFromSeed(ThemeService.Seed);
            StateHasChanged();
        }

        protected abstract void SetFromSeed(int seed);

        public void Dispose()
        {
            ThemeService.ThemeChanged -= OnSeedChanged;
        }
    }
}
