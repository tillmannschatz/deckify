using PowerPoint = Microsoft.Office.Interop.PowerPoint;

namespace Deckify.Modules.Services
{
    public interface IPresentationService
    {
        string CreateSelectedSlidesExtract(PowerPoint.Presentation activePresentation, string suffix, bool closeExtractPresentation);
        void OpenFileLocation(PowerPoint.Presentation pActive);
    }
}
