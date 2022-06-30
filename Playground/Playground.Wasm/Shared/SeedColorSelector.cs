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
            ThemeService.SeedChanged += OnSeedChanged;
            OnSeedChanged(null, ThemeService.Seed);
        }

        private void OnSeedChanged(object sender, int e)
        {
            if (sender == this) return;
            SetFromSeed(e);
            StateHasChanged();
        }

        protected abstract void SetFromSeed(int seed);

        public void Dispose()
        {
            ThemeService.SeedChanged -= OnSeedChanged;
        }
    }
}
