using MaterialColorUtilities.ColorAppearance;
using MaterialColorUtilities.Samples.Wasm.Services;
using Microsoft.AspNetCore.Components;

namespace MaterialColorUtilities.Samples.Wasm.Shared
{
    public partial class HctSliders : IDisposable
    {
        private double _hue;
        private double _chroma;
        private double _tone;

        [Inject] public ThemeService ThemeService { get; set; }        
        double Hue { get => _hue; set { if (_hue == value) return; _hue = value; ComponentChanged(); } }
        double Chroma { get => _chroma; set { if (_chroma == value) return; _chroma = value; ComponentChanged(); } }
        double Tone { get => _tone; set { if (_tone == value) return; _tone = value; ComponentChanged(); } }

        void Update(object sender, EventArgs eventArgs)
        {
            Hct hct = Hct.FromInt(ThemeService.Seed);
            _hue = hct.Hue;
            _chroma = hct.Chroma;
            _tone = hct.Tone;
            StateHasChanged();
        }

        void ComponentChanged()
        {
            Hct hct = Hct.From(Hue, Chroma, Tone);
            ThemeService.Seed = hct.ToInt();
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();
            ThemeService.ThemeChanged += Update;
            Update(null, null);
        }

        public void Dispose() => ThemeService.ThemeChanged -= Update;

    }
}
