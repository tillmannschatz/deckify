using System;
using System.IO;
using System.Windows.Forms;
using PowerPoint = Microsoft.Office.Interop.PowerPoint;
using Outlook = Microsoft.Office.Interop.Outlook;
using Deckify;

namespace Deckify.Modules.Services
{
    public class EmailService : IEmailService
    {
        public void EmailPresentation(PowerPoint.Presentation activePresentation)
        {
            // Mail settings (Subject, Body, ...)
            string mailSubjectSuffix = "Emailing: ";
            string mailSubjectPostfix = "";
            string mailSubject = mailSubjectSuffix + activePresentation.Name + mailSubjectPostfix;
            string mailBody = "Slides are attached.";

            // make a local copy of the presentation
            string mailAttachment = Path.GetTempPath() + Path.GetFileNameWithoutExtension(activePresentation.Name) + ".pptx";
            activePresentation.SaveCopyAs(mailAttachment);

            SendMailWithAttachment(mailAttachment, mailSubject, mailBody);

            try 
            { 
                File.Delete(mailAttachment); 
            } 
            catch (Exception ex)
            { 
                Deckify.Common.Logger.LogError(ex, "EmailPresentation Cleanup");
            }
        }

        public void EmailSelectedSlides(PowerPoint.Presentation activePresentation, Func<PowerPoint.Presentation, string, bool, string> createExtractCallback)
        {
             string mailAttachment = createExtractCallback(activePresentation, "_EXTRACT", true);

            // Mail settings (Subject, Body, ...) and close extracted copy
            string mailSubjectSuffix = "Emailing: ";
            string mailSubjectPostfix = "";
            string mailSubject = mailSubjectSuffix + Path.GetFileName(mailAttachment) + mailSubjectPostfix;
            string mailBody = "Extracted slides are attached.";

            SendMailWithAttachment(mailAttachment, mailSubject, mailBody);

            try 
            { 
                File.Delete(mailAttachment); 
            } 
            catch (Exception ex)
            {
                Deckify.Common.Logger.LogError(ex, "EmailSelectedSlides Cleanup");
            }
        }


        private void SendMailWithAttachment(string attachmentPath, string subject, string body)
        {
             // Check file size
            long length = new System.IO.FileInfo(attachmentPath).Length;

            // Create Mail and attach File, in case of >25MB give out warning
            if (length < 25 * 1024 * 1024)
            {
                CreateMail(mailAttachment: attachmentPath, mailSubject: subject, mailBody: body);
            }
            else 
            { 
                MessageBox.Show("Error: Presentation filesize is above 25MB!"); 
            }
        }
        public void CreateMail(string mailTo = "", string mailCC = "", string mailAttachment = "", string mailSubject = "", string mailBody = "") //optional --> requires at least C# 4.0
        {
            Outlook.Application outlookApp;
            try 
            { 
                outlookApp = (Outlook.Application)System.Runtime.InteropServices.Marshal.GetActiveObject("Outlook.Application"); 
            }
            catch 
            { 
                try
                {
                    outlookApp = new Outlook.Application();
                }
                catch
                {
                    throw new Exception("Microsoft Outlook could not be started. Please ensure it is installed and working.");
                }
            }

            Outlook.MailItem mailItem = (Outlook.MailItem)
               outlookApp.CreateItem(Outlook.OlItemType.olMailItem);
            if (!string.IsNullOrWhiteSpace(mailTo)) { mailItem.To = mailTo; }
            if (!string.IsNullOrWhiteSpace(mailCC)) { mailItem.CC = mailCC; }
            if (!string.IsNullOrWhiteSpace(mailAttachment)) { mailItem.Attachments.Add(mailAttachment); }
            if (!string.IsNullOrWhiteSpace(mailSubject)) { mailItem.Subject = mailSubject; }
            mailItem.Display(mailItem);
            if (!string.IsNullOrWhiteSpace(mailBody)) { mailItem.HTMLBody = mailBody + mailItem.HTMLBody; }
        }
    }
}
