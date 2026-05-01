using System.Collections.Generic;

namespace Deckify.Modules.Colors
{
    public interface IPaletteService
    {
        List<ColorModel> GetPalette();
        void AddColor(ColorModel color);
        void RemoveColor(ColorModel color);
        void SavePalette();
    }
}
