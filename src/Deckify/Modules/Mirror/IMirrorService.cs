using System;
using PowerPoint = Microsoft.Office.Interop.PowerPoint;

namespace Deckify.Modules.Mirror
{
    public interface IMirrorService
    {
        void Initialize();
        void TagShapeAsMirror(PowerPoint.Shape shape);
        bool IsMirrorShape(PowerPoint.Shape shape);
        void SyncMirrorShapes(PowerPoint.Shape sourceShape);
    }
}
