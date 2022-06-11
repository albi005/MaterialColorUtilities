using MaterialColorUtilities.ColorAppearance;
using Playground.Wasm.Services;

namespace Playground.Wasm.Shared
{
    public partial class HctSliders 
    {
        private double _hue;
        private double _chroma;
        private double _tone;

        double Hue { get => _hue; set { if (_hue == value) return; _hue = value; HctChanged(); } }
        double Chroma { get => _chroma; set { if (_chroma == value) return; _chroma = value; HctChanged(); } }
        double Tone { get => _tone; set { if (_tone == value) return; _tone = value; HctChanged(); } }

        protected override void SetFromSeed(int seed)
        {
            Hct hct = Hct.FromInt(seed);
            _hue = hct.Hue;
            _chroma = hct.Chroma;
            _tone = hct.Tone;
        }

        void HctChanged()
        {
            Hct hct = Hct.From(Hue, Chroma, Tone);
            int color = hct.ToInt();
            ThemeService.SetSeed(color, this);
        }
    }
}
