using System.IO;
using PowerPoint = Microsoft.Office.Interop.PowerPoint;
using Deckify;

namespace Deckify.Modules.Services
{
    public class PresentationService : IPresentationService
    {
        public string CreateSelectedSlidesExtract(PowerPoint.Presentation activePresentation, string suffix, bool closeExtractPresentation)
        {
            RemoveSlideTagsSelected(activePresentation);

            foreach (PowerPoint.Slide sld in Globals.ThisAddIn.Application.ActiveWindow.Selection.SlideRange)
            {
                sld.Tags.Add("SELECTED", "YES");
            }

            string extractPath = Path.Combine(Path.GetTempPath(), Path.GetFileNameWithoutExtension(activePresentation.Name) + suffix + ".pptx");
            activePresentation.SaveCopyAs(extractPath);
            RemoveSlideTagsSelected(activePresentation);

            PowerPoint.Presentation tempPresentation = Globals.ThisAddIn.Application.Presentations.Open(extractPath);
            RemoveSlidesWithoutTag(tempPresentation, "SELECTED", "YES");
            RemoveSlideTagsSelected(tempPresentation);
            DeleteEmptySections(tempPresentation);
            tempPresentation.Save();

            if (closeExtractPresentation)
            {
                tempPresentation.Close();
            }

            return extractPath;
        }

        private void RemoveSlidesWithoutTag(PowerPoint.Presentation presentation, string tagName, string tagValue)
        {
            for (int i = presentation.Slides.Count; i >= 1; i--)
            {
                bool keepSlide = false;
                for (int j = 1; j <= presentation.Slides[i].Tags.Count; j++)
                {
                    if (presentation.Slides[i].Tags.Name(j) == tagName && presentation.Slides[i].Tags.Value(j) == tagValue)
                    {
                        keepSlide = true;
                        break;
                    }
                }

                if (!keepSlide)
                {
                    presentation.Slides[i].Delete();
                }
            }
        }

        private void RemoveSlideTagsSelected(PowerPoint.Presentation p)
        {
            foreach (PowerPoint.Slide sld in p.Slides) { sld.Tags.Delete("SELECTED"); }
        }

        private void DeleteEmptySections(PowerPoint.Presentation p)
        {
            for (int i = p.SectionProperties.Count; i >= 1; i--) { if (p.SectionProperties.SlidesCount(i) == 0) { p.SectionProperties.Delete(i, true); } }
        }
        public void OpenFileLocation(PowerPoint.Presentation pActive)
        {
            // Check if the presentation has been saved (i.e., has a valid path)
            if (string.IsNullOrEmpty(pActive.Path))
            {
                System.Windows.Forms.MessageBox.Show("This presentation hasn't been saved yet.");
            }
            else
            {
                // Open Windows Explorer to the presentation's folder
                System.Diagnostics.Process.Start("explorer.exe", pActive.Path);
            }
        }
    }
}
