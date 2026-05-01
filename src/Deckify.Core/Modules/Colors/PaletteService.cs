using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Deckify.Common;

namespace Deckify.Modules.Colors
{
    public class PaletteService : IPaletteService
    {
        private List<ColorModel> _palette;
        private readonly string _filePath;

        public PaletteService(string filePath = null)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                // Save in %AppData%/deckify/palette.json
                string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                string pluginDir = Path.Combine(appData, "deckify");
                
                if (!Directory.Exists(pluginDir))
                {
                    Directory.CreateDirectory(pluginDir);
                }

                _filePath = Path.Combine(pluginDir, "palette.json");
            }
            else
            {
                _filePath = filePath;
            }

            LoadPalette();
        }

        private void LoadPalette()
        {
            if (File.Exists(_filePath))
            {
                try
                {
                    string json = File.ReadAllText(_filePath);
                    _palette = JsonConvert.DeserializeObject<List<ColorModel>>(json) ?? new List<ColorModel>();
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, "Failed to load palette");
                    _palette = new List<ColorModel>();
                }
            }
            else
            {
                _palette = new List<ColorModel>();
                SavePalette();
            }
        }

        public List<ColorModel> GetPalette()
        {
            return _palette;
        }

        public void AddColor(ColorModel color)
        {
            _palette.Add(color);
            SavePalette();
        }

        public void RemoveColor(ColorModel color)
        {
            _palette.Remove(color);
            SavePalette();
        }

        public void SavePalette()
        {
            try
            {
                string json = JsonConvert.SerializeObject(_palette, Formatting.Indented);
                File.WriteAllText(_filePath, json);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Failed to save palette");
            }
        }
    }
}
