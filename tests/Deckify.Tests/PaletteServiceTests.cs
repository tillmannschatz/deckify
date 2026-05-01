using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xunit;
using Deckify.Modules.Colors;

namespace Deckify.Tests
{
    public class PaletteServiceTests : IDisposable
    {
        private readonly string _tempFile;

        public PaletteServiceTests()
        {
            _tempFile = Path.GetTempFileName();
        }

        public void Dispose()
        {
            if (File.Exists(_tempFile))
            {
                File.Delete(_tempFile);
            }
        }

        [Fact]
        public void Constructor_Initialization_CreatesEmptyPalette_IfFileMissing()
        {
            // Arrange — Default-Palette ist seit dem deckify-Rebrand leer
            // (PaletteService.cs:54). User customized seine Farben selbst.
            File.Delete(_tempFile);

            // Act
            var service = new PaletteService(_tempFile);

            // Assert
            var palette = service.GetPalette();
            Assert.NotNull(palette);
            Assert.Empty(palette);
        }

        [Fact]
        public void AddColor_AddsColorToPaletteAndPersists()
        {
            // Arrange
            var service = new PaletteService(_tempFile);
            var initialCount = service.GetPalette().Count;
            var newColor = new ColorModel("Test Red", "#FF0000");

            // Act
            service.AddColor(newColor);

            // Assert
            Assert.Equal(initialCount + 1, service.GetPalette().Count);
            Assert.Contains(service.GetPalette(), c => c.Name == "Test Red");

            // Verify persistence by reloading
            var distinctService = new PaletteService(_tempFile);
            Assert.Contains(distinctService.GetPalette(), c => c.Name == "Test Red");
        }

        [Fact]
        public void RemoveColor_RemovesColorFromPaletteAndPersists()
        {
            // Arrange — Default-Palette ist leer; erst eine Color hinzufuegen,
            // dann removen, damit der Test deterministisch ist.
            File.Delete(_tempFile);
            var service = new PaletteService(_tempFile);
            var colorToRemove = new ColorModel("Test Blue", "#0000FF");
            service.AddColor(colorToRemove);
            var initialCount = service.GetPalette().Count;

            // Act
            service.RemoveColor(colorToRemove);

            // Assert
            Assert.Equal(initialCount - 1, service.GetPalette().Count);
            Assert.DoesNotContain(service.GetPalette(), c => c.Name == colorToRemove.Name);

            // Verify persistence
            var distinctService = new PaletteService(_tempFile);
            Assert.DoesNotContain(distinctService.GetPalette(), c => c.Name == colorToRemove.Name);
        }
    }
}
