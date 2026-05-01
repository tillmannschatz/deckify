using System;
using System.Linq;
using System.Runtime.InteropServices;
using Microsoft.Office.Core;
using PowerPoint = Microsoft.Office.Interop.PowerPoint;
using Deckify.Common;

namespace Deckify.Modules.Mirror
{
    public class MirrorService : IMirrorService
    {
        private const string MIRROR_TAG = "Deckify_MirrorID";
        private PowerPoint.Application _application;
        private string _lastShapeId;
        private string _lastShapeText;
        private int _lastInternalShapeId; 

        public MirrorService(PowerPoint.Application application)
        {
            _application = application;
        }

        public void Initialize()
        {
            _application.WindowSelectionChange += Application_WindowSelectionChange;
        }

        public void TagShapeAsMirror(PowerPoint.Shape shape)
        {
            if (shape == null) return;
            
            // If already tagged, keep existing tag
            string existingTag = GetTag(shape, MIRROR_TAG);
            if (string.IsNullOrEmpty(existingTag))
            {
                string guid = Guid.NewGuid().ToString();
                shape.Tags.Add(MIRROR_TAG, guid);
            }
        }

        public bool IsMirrorShape(PowerPoint.Shape shape)
        {
            if (shape == null) return false;
            return !string.IsNullOrEmpty(GetTag(shape, MIRROR_TAG));
        }

        private string GetTag(PowerPoint.Shape shape, string name)
        {
            try
            {
                for (int i = 1; i <= shape.Tags.Count; i++)
                {
                    string tagName = shape.Tags.Name(i);
                    if (tagName.Equals(name, StringComparison.OrdinalIgnoreCase))
                    {
                        return shape.Tags.Value(i);
                    }
                }
            }
            catch { }
            return null;
        }

        private void Application_WindowSelectionChange(PowerPoint.Selection selection)
        {
            // 1. Check if we moved AWAY from a mirror shape that we were tracking
            if (!string.IsNullOrEmpty(_lastShapeId) && !string.IsNullOrEmpty(_lastShapeText))
            {
                 CheckAndSyncPreviousShape();
            }

            // 2. Track the NEW selection
            TrackNewSelection(selection);
        }

        private void CheckAndSyncPreviousShape()
        {
            if (_lastTrackedShape != null)
            {
                try
                {
                    // Check if it's still valid
                    string currentText = _lastTrackedShape.TextFrame.TextRange.Text;
                    if (IsMirrorShape(_lastTrackedShape) && currentText != _lastShapeText)
                    {
                        // Text changed! Sync.
                        SyncMirrorShapes(_lastTrackedShape);
                    }
                }
                catch
                {
                    // Shape might be deleted or invalid
                }
            }
            
            _lastTrackedShape = null;
            _lastShapeId = null;
            _lastShapeText = null;
            _lastInternalShapeId = 0;
        }

        private PowerPoint.Shape _lastTrackedShape;

        private void TrackNewSelection(PowerPoint.Selection selection)
        {
            try
            {
                if (selection.Type == PowerPoint.PpSelectionType.ppSelectionShapes || selection.Type == PowerPoint.PpSelectionType.ppSelectionText)
                {
                    if (selection.ShapeRange.Count == 1)
                    {
                         PowerPoint.Shape shape = selection.ShapeRange[1];
                         bool isMirror = IsMirrorShape(shape);
                         
                         if (isMirror && shape.HasTextFrame == MsoTriState.msoTrue)
                         {
                             _lastTrackedShape = shape;
                             _lastShapeId = GetTag(shape, MIRROR_TAG);
                             _lastShapeText = shape.TextFrame.TextRange.Text;
                             _lastInternalShapeId = shape.Id;
                         }
                    }
                }
            }
            catch { }
        }

        public void SyncMirrorShapes(PowerPoint.Shape sourceShape)
        {
            string mirrorId = GetTag(sourceShape, MIRROR_TAG);
            if (string.IsNullOrEmpty(mirrorId)) 
            {
                return;
            }

            string newText = sourceShape.TextFrame.TextRange.Text;
            int sourceInternalId = sourceShape.Id;

            // Iterate all slides
            foreach (PowerPoint.Slide slide in _application.ActivePresentation.Slides)
            {
                foreach (PowerPoint.Shape shape in slide.Shapes)
                {
                    // Don't update self - robust comparison
                    if (shape.Id == sourceInternalId && shape.Parent == sourceShape.Parent) continue; 

                    if (IsMirrorShape(shape) && GetTag(shape, MIRROR_TAG) == mirrorId)
                    {
                        if (shape.HasTextFrame == MsoTriState.msoTrue)
                        {
                            shape.TextFrame.TextRange.Text = newText;
                        }
                    }
                }
            }
        }
    }
}
