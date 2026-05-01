using PowerPoint = Microsoft.Office.Interop.PowerPoint;

namespace Deckify.Modules.Services
{
    public interface IReferenceService
    {
        void NewLink(PowerPoint.Presentation pActive, PowerPoint.DocumentWindow activeWindow);
        void EditLink(PowerPoint.Presentation pActive, PowerPoint.DocumentWindow activeWindow);
        void UpdateSlide(PowerPoint.Presentation pActive, PowerPoint.DocumentWindow activeWindow);
        void NewObjectLink(PowerPoint.DocumentWindow activeWindow);
        void PasteObject(PowerPoint.Presentation pActive, PowerPoint.DocumentWindow activeWindow);
        void UpdateObject(PowerPoint.Presentation pActive, PowerPoint.DocumentWindow activeWindow);
    }
}
