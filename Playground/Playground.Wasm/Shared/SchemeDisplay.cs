using Microsoft.AspNetCore.Components;
using Playground.Wasm.Services;

namespace Playground.Wasm.Shared;

public class SchemeDisplay : ComponentBase, IDisposable
{
    [Inject] public ThemeService ThemeService { get; set; } = null!;
    
    protected override void OnInitialized()
    {
        ThemeService.Changed += StateHasChanged;
    }
    
    public void Dispose()
    {
        ThemeService.Changed -= StateHasChanged;
    }
}