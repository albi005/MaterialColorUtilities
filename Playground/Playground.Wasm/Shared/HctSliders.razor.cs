using MaterialColorUtilities.ColorAppearance;
using Playground.Wasm.Services;

namespace Playground.Wasm.Shared
{
    public partial class HctSliders 
    {
        private double _hue;
        private double _chroma;
        private double _tone;
        private Hct _hct = Hct.FromInt(0);
        private bool _showActualValues;

        double Hue { get => _hue; set { if (_hue == value) return; _hue = value; HctChanged(); } }
        double Chroma { get => _chroma; set { if (_chroma == value) return; _chroma = value; HctChanged(); } }
        double Tone { get => _tone; set { if (_tone == value) return; _tone = value; HctChanged(); } }

        protected override void SetFromSeed(uint seed)
        {
            if (_hct.ToInt() == seed) return;
            _hct = Hct.FromInt(seed);
            _hue = _hct.Hue;
            _chroma = _hct.Chroma;
            _tone = _hct.Tone;
            _showActualValues = false;
        }

        void HctChanged()
        {
            _hct = Hct.From(Hue, Chroma, Tone);
            uint color = _hct.ToInt();
            ThemeService.Seed = color;
            _showActualValues = true;
        }
    }
}
