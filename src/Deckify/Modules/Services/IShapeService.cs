using PowerPoint = Microsoft.Office.Interop.PowerPoint;

namespace Deckify.Modules.Services
{
    public interface IShapeService
    {
        void MakeSameHeight(PowerPoint.DocumentWindow activeWindow);
        void MakeSameWidth(PowerPoint.DocumentWindow activeWindow);
        void MakeSameSize(PowerPoint.DocumentWindow activeWindow);
        void LockRatio(PowerPoint.DocumentWindow activeWindow, bool isLocked);
        bool IsRatioLocked(PowerPoint.DocumentWindow activeWindow);
        void ObjectsRemoveSpacingHorizontal(PowerPoint.DocumentWindow activeWindow);
        void ObjectsIncreaseSpacingHorizontal(PowerPoint.DocumentWindow activeWindow);
        void ObjectsDecreaseSpacingHorizontal(PowerPoint.DocumentWindow activeWindow);
        void ObjectsRemoveSpacingVertical(PowerPoint.DocumentWindow activeWindow);
        void ObjectsIncreaseSpacingVertical(PowerPoint.DocumentWindow activeWindow);
        void ObjectsDecreaseSpacingVertical(PowerPoint.DocumentWindow activeWindow);
        void RemoveTransitions(PowerPoint.Presentation activePresentation);
        void RemoveAnimations(PowerPoint.Presentation activePresentation);
        void AddCommentSticker(PowerPoint.DocumentWindow activeWindow, string tag);
        string CheckActiveShapeType(PowerPoint.DocumentWindow activeWindow);
    }
}
