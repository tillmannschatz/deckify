using Microsoft.Office.Core;
using PowerPoint = Microsoft.Office.Interop.PowerPoint;

namespace Deckify.Modules.Services
{
    public class LanguageService : ILanguageService
    {
        public void SetLanguage(PowerPoint.Presentation activePresentation, MsoLanguageID languageID)
        {
            activePresentation.DefaultLanguageID = languageID;
            foreach (PowerPoint.Slide sld in activePresentation.Slides)
            {
                foreach (PowerPoint.Shape shp in sld.Shapes) { ChangeLanguageOfAllSubShapes(shp, languageID); }
            }
            foreach (PowerPoint.Shape shp in activePresentation.SlideMaster.Shapes) { ChangeLanguageOfAllSubShapes(shp, languageID); }
        }

        private void ChangeLanguageOfAllSubShapes(PowerPoint.Shape targetShape, MsoLanguageID languageID)
        {
            if (targetShape.HasTextFrame == MsoTriState.msoTrue) { targetShape.TextFrame.TextRange.LanguageID = languageID; }
             if (targetShape.Type == MsoShapeType.msoGroup || targetShape.Type == MsoShapeType.msoSmartArt)
            {
                foreach (PowerPoint.Shape shp in targetShape.GroupItems) { ChangeLanguageOfAllSubShapes(shp, languageID); }
            }
            if (targetShape.HasTable == MsoTriState.msoTrue)
            {
                for (int i = 1; i <= targetShape.Table.Rows.Count; i++)
                {
                    for (int j = 1; j <= targetShape.Table.Columns.Count; j++) { targetShape.Table.Cell(i, j).Shape.TextFrame.TextRange.LanguageID = languageID; }
                }
            }
        }
    }
}
