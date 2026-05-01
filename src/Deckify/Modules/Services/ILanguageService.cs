using Microsoft.Office.Core;
using PowerPoint = Microsoft.Office.Interop.PowerPoint;

namespace Deckify.Modules.Services
{
    public interface ILanguageService
    {
        void SetLanguage(PowerPoint.Presentation activePresentation, MsoLanguageID languageID);
    }
}
