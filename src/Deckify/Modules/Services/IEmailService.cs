using System;
using PowerPoint = Microsoft.Office.Interop.PowerPoint;

namespace Deckify.Modules.Services
{
    public interface IEmailService
    {
        void EmailPresentation(PowerPoint.Presentation presentation);
        void EmailSelectedSlides(PowerPoint.Presentation presentation, Func<PowerPoint.Presentation, string, bool, string> createExtractCallback);
        void CreateMail(string mailTo = "", string mailCC = "", string mailAttachment = "", string mailSubject = "", string mailBody = "");
    }
}
