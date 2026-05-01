using System;
using Microsoft.Office.Core;
using PowerPoint = Microsoft.Office.Interop.PowerPoint;
using Deckify.Common;

namespace Deckify.Modules.Services
{
    public class ShapeService : IShapeService
    {
        /// <summary>
        /// Sets the height of all selected shapes to match the tallest shape in the selection.
        /// </summary>
        /// <param name="activeWindow">The active PowerPoint document window.</param>
        public void MakeSameHeight(PowerPoint.DocumentWindow activeWindow)
        {
            EnsureShapesSelected(activeWindow);
            Deckify.Common.SystemUtilities.RunBatchOperation(activeWindow.Application, () =>
            {
                float shp_height = 0;
                PowerPoint.ShapeRange selectedShapeRange = activeWindow.Selection.ShapeRange;
                if (selectedShapeRange.Count > 1)
                {
                    // Loop over selected shapes to get tallest
                    for (int shp = 1; shp <= selectedShapeRange.Count; shp++)
                    {
                        if (shp_height < selectedShapeRange[shp].Height)
                        {
                            shp_height = selectedShapeRange[shp].Height;
                        }
                    }

                    // Loop over selected shapes to set height
                    for (int shp = 1; shp <= selectedShapeRange.Count; shp++)
                    {
                        selectedShapeRange[shp].Height = shp_height;
                    }
                }
            });
        }

        /// <summary>
        /// Sets the width of all selected shapes to match the widest shape in the selection.
        /// </summary>
        /// <param name="activeWindow">The active PowerPoint document window.</param>
        public void MakeSameWidth(PowerPoint.DocumentWindow activeWindow)
        {
            EnsureShapesSelected(activeWindow);
            Deckify.Common.SystemUtilities.RunBatchOperation(activeWindow.Application, () =>
            {
                float shp_width = 0;
                PowerPoint.ShapeRange selectedShapeRange = activeWindow.Selection.ShapeRange;
                if (selectedShapeRange.Count > 1)
                {
                    // Loop over selected shapes to get widest
                    for (int shp = 1; shp <= selectedShapeRange.Count; shp++)
                    {
                        if (shp_width < selectedShapeRange[shp].Width)
                        {
                            shp_width = selectedShapeRange[shp].Width;
                        }
                    }

                    // Loop over selected shapes to set width
                    for (int shp = 1; shp <= selectedShapeRange.Count; shp++)
                    {
                        selectedShapeRange[shp].Width = shp_width;
                    }
                }
            });
        }

        /// <summary>
        /// Sets both the width and height of all selected shapes to match the largest dimensions in the selection.
        /// </summary>
        /// <param name="activeWindow">The active PowerPoint document window.</param>
        public void MakeSameSize(PowerPoint.DocumentWindow activeWindow)
        {
            MakeSameWidth(activeWindow);
            MakeSameHeight(activeWindow);
        }

        /// <summary>
        /// Locks or unlocks the aspect ratio for all selected shapes.
        /// </summary>
        /// <param name="activeWindow">The active PowerPoint document window.</param>
        /// <param name="isLocked">If true, locks the aspect ratio; otherwise unlocks it.</param>
        public void LockRatio(PowerPoint.DocumentWindow activeWindow, bool isLocked)
        {
            EnsureShapesSelected(activeWindow);
            PowerPoint.ShapeRange selectedShapeRange = activeWindow.Selection.ShapeRange;
            if (selectedShapeRange.Count >= 1)
            {
                // Set lock state for all selected shapes
                selectedShapeRange.LockAspectRatio = isLocked ? MsoTriState.msoTrue : MsoTriState.msoFalse;
            }
        }

        /// <summary>
        /// Checks if the aspect ratio is locked for the selected shapes.
        /// </summary>
        /// <param name="activeWindow">The active PowerPoint document window.</param>
        /// <returns>True if the aspect ratio is locked; otherwise false.</returns>
        public bool IsRatioLocked(PowerPoint.DocumentWindow activeWindow)
        {
             if (activeWindow.Selection.Type != PowerPoint.PpSelectionType.ppSelectionShapes)
            {
                return false;
            }
            
            PowerPoint.ShapeRange selectedShapeRange = activeWindow.Selection.ShapeRange;
            if (selectedShapeRange.Count > 0)
            {
                // Return true only if valid and locked
                // Check if ALL are locked, or at least one? Usually "Pressed" means "All Mixed" or "All Locked".
                // Simplest: Check the properties of the range.
                return selectedShapeRange.LockAspectRatio == MsoTriState.msoTrue;
            }
            return false;
        }

        /// <summary>
        /// Removes horizontal spacing between selected shapes, aligning them adjacently.
        /// </summary>
        /// <param name="activeWindow">The active PowerPoint document window.</param>
        public void ObjectsRemoveSpacingHorizontal(PowerPoint.DocumentWindow activeWindow)
        {
            ProcessHorizontalSpacing(activeWindow, (range, sortedIndices, i) => 
            {
                range[sortedIndices[i - 1]].Left = Deckify.Common.SpacingHelpers.CalculateAdjacentPosition(range[sortedIndices[i - 2]].Left, range[sortedIndices[i - 2]].Width);
            });
        }

        /// <summary>
        /// Increases the horizontal spacing between selected shapes by a default step.
        /// </summary>
        /// <param name="activeWindow">The active PowerPoint document window.</param>
        public void ObjectsIncreaseSpacingHorizontal(PowerPoint.DocumentWindow activeWindow)
        {
            ProcessHorizontalSpacing(activeWindow, (range, sortedIndices, i) =>
            {
                float offset = Deckify.Common.SpacingHelpers.CalculateSpacingOffset(i, Deckify.Common.SpacingHelpers.DefaultSpacingStep);
                range[sortedIndices[i - 1]].Left = range[sortedIndices[i - 1]].Left + offset;
            });
        }

        /// <summary>
        /// Decreases the horizontal spacing between selected shapes by a default step.
        /// </summary>
        /// <param name="activeWindow">The active PowerPoint document window.</param>
        public void ObjectsDecreaseSpacingHorizontal(PowerPoint.DocumentWindow activeWindow)
        {
            ProcessHorizontalSpacing(activeWindow, (range, sortedIndices, i) =>
            {
                float offset = Deckify.Common.SpacingHelpers.CalculateSpacingOffset(i, Deckify.Common.SpacingHelpers.DefaultSpacingStep);
                range[sortedIndices[i - 1]].Left = range[sortedIndices[i - 1]].Left - offset;
            });
        }

        /// <summary>
        /// Removes vertical spacing between selected shapes, aligning them adjacently.
        /// </summary>
        /// <param name="activeWindow">The active PowerPoint document window.</param>
        public void ObjectsRemoveSpacingVertical(PowerPoint.DocumentWindow activeWindow)
        {
             ProcessVerticalSpacing(activeWindow, (range, sortedIndices, i) =>
            {
                range[sortedIndices[i - 1]].Top = Deckify.Common.SpacingHelpers.CalculateAdjacentPosition(range[sortedIndices[i - 2]].Top, range[sortedIndices[i - 2]].Height);
            });
        }

        /// <summary>
        /// Increases the vertical spacing between selected shapes by a default step.
        /// </summary>
        /// <param name="activeWindow">The active PowerPoint document window.</param>
        public void ObjectsIncreaseSpacingVertical(PowerPoint.DocumentWindow activeWindow)
        {
            ProcessVerticalSpacing(activeWindow, (range, sortedIndices, i) =>
            {
                float offset = Deckify.Common.SpacingHelpers.CalculateSpacingOffset(i, Deckify.Common.SpacingHelpers.DefaultSpacingStep);
                range[sortedIndices[i - 1]].Top = range[sortedIndices[i - 1]].Top + offset;
            });
        }

        /// <summary>
        /// Decreases the vertical spacing between selected shapes by a default step.
        /// </summary>
        /// <param name="activeWindow">The active PowerPoint document window.</param>
        public void ObjectsDecreaseSpacingVertical(PowerPoint.DocumentWindow activeWindow)
        {
             ProcessVerticalSpacing(activeWindow, (range, sortedIndices, i) =>
            {
                float offset = Deckify.Common.SpacingHelpers.CalculateSpacingOffset(i, Deckify.Common.SpacingHelpers.DefaultSpacingStep);
                range[sortedIndices[i - 1]].Top = range[sortedIndices[i - 1]].Top - offset;
            });
        }

        private void ProcessHorizontalSpacing(PowerPoint.DocumentWindow activeWindow, Action<PowerPoint.ShapeRange, int[], int> placementAction)
        {
            Deckify.Common.SystemUtilities.RunBatchOperation(activeWindow.Application, () =>
            {
                if (activeWindow.Selection.Type == PowerPoint.PpSelectionType.ppSelectionShapes)
                {
                    PowerPoint.ShapeRange selectedShapeRange = activeWindow.Selection.HasChildShapeRange 
                        ? activeWindow.Selection.ChildShapeRange 
                        : activeWindow.Selection.ShapeRange;

                    if (selectedShapeRange.Count > 1)
                    {
                        int[] indexValues = new int[selectedShapeRange.Count];
                        float[] sortValues = new float[selectedShapeRange.Count];
                        for (int shp = 1; shp <= selectedShapeRange.Count; shp++)
                        {
                            indexValues[shp - 1] = shp;
                            sortValues[shp - 1] = selectedShapeRange[shp].Left;
                        }

                        Array.Sort(sortValues, indexValues);

                        for (int shp = 2; shp <= selectedShapeRange.Count; shp++)
                        {
                            placementAction(selectedShapeRange, indexValues, shp);
                        }
                    }
                }
            });
        }

        private void ProcessVerticalSpacing(PowerPoint.DocumentWindow activeWindow, Action<PowerPoint.ShapeRange, int[], int> placementAction)
        {
            Deckify.Common.SystemUtilities.RunBatchOperation(activeWindow.Application, () =>
            {
                 if (activeWindow.Selection.Type == PowerPoint.PpSelectionType.ppSelectionShapes)
                {
                    PowerPoint.ShapeRange selectedShapeRange = activeWindow.Selection.HasChildShapeRange 
                        ? activeWindow.Selection.ChildShapeRange 
                        : activeWindow.Selection.ShapeRange;

                    if (selectedShapeRange.Count > 1)
                    {
                        int[] indexValues = new int[selectedShapeRange.Count];
                        float[] sortValues = new float[selectedShapeRange.Count];
                        for (int shp = 1; shp <= selectedShapeRange.Count; shp++)
                        {
                            indexValues[shp - 1] = shp;
                            sortValues[shp - 1] = selectedShapeRange[shp].Top;
                        }

                        Array.Sort(sortValues, indexValues);

                        for (int shp = 2; shp <= selectedShapeRange.Count; shp++)
                        {
                                placementAction(selectedShapeRange, indexValues, shp);
                        }
                    }
                }
            });
        }

        /// <summary>
        /// Removes all slide transitions from the active presentation.
        /// </summary>
        /// <param name="activePresentation">The active PowerPoint presentation.</param>
        public void RemoveTransitions(PowerPoint.Presentation activePresentation)
        {
            Deckify.Common.SystemUtilities.RunBatchOperation(activePresentation.Application, () =>
            {
                foreach (PowerPoint.Slide sld in activePresentation.Slides)
                {
                    sld.SlideShowTransition.AdvanceOnTime = MsoTriState.msoFalse;
                    sld.SlideShowTransition.AdvanceOnClick = MsoTriState.msoTrue;
                    sld.SlideShowTransition.EntryEffect = PowerPoint.PpEntryEffect.ppEffectNone;
                }
            });
        }

        /// <summary>
        /// Removes all animations from all slides in the active presentation.
        /// </summary>
        /// <param name="activePresentation">The active PowerPoint presentation.</param>
        public void RemoveAnimations(PowerPoint.Presentation activePresentation)
        {
            Deckify.Common.SystemUtilities.RunBatchOperation(activePresentation.Application, () =>
            {
                 foreach (PowerPoint.Slide sld in activePresentation.Slides)
                {
                    // loop through each slide animation on slide
                    for (int i = sld.TimeLine.MainSequence.Count; i >= 1; i--) { sld.TimeLine.MainSequence[i].Delete(); }
                }
            });
        }
        /// <summary>
        /// Adds a standardized comment sticker shape to the current slide.
        /// </summary>
        /// <param name="activeWindow">The active PowerPoint document window.</param>
        /// <param name="tag">The tag indicating placement (e.g. "inside").</param>
        public void AddCommentSticker(PowerPoint.DocumentWindow activeWindow, string tag)
        {
            // Only need to ensure a slide is active (View.Slide works in Normal View)
            if (activeWindow.ViewType != PowerPoint.PpViewType.ppViewNormal)
            {
                 throw new Exception("Please switch to Normal View to add stickers.");
            }

            // Add Sticker (check if inside or outside)
            PowerPoint.Slide sld = activeWindow.View.Slide;
            PowerPoint.Shape shp = (tag == Constants.StickerTags.Inside) ? sld.Shapes.AddShape(Microsoft.Office.Core.MsoAutoShapeType.msoShapeFoldedCorner, 20, 20, 200, 100) : sld.Shapes.AddShape(Microsoft.Office.Core.MsoAutoShapeType.msoShapeFoldedCorner, -220, 0, 200, 100);

            // Format Shape
            FormatShape(shp);

            // Select shape for further editing
            shp.Select();
        }

        private void FormatShape(PowerPoint.Shape shp)
        {
            // Outline and fill color
            shp.Line.Visible = MsoTriState.msoFalse;
            shp.Fill.Visible = MsoTriState.msoTrue;
            shp.Fill.Solid();
            // here the color RGB is given in format of BGR because interop reads it as BGR and not RGB
            shp.Fill.ForeColor.RGB = System.Drawing.Color.FromArgb(128, 255, 255).ToArgb();
            shp.Fill.ForeColor.TintAndShade = 0;
            shp.Fill.Transparency = 0;

            // Effects
            shp.Shadow.Type = (MsoShadowType)21;
            //shp.Shadow.visible = MsoTriState.msoFalse;
            shp.Reflection.Type = MsoReflectionType.msoReflectionTypeNone;
            shp.Glow.Radius = 0;
            shp.SoftEdge.Type = MsoSoftEdgeType.msoSoftEdgeTypeNone;
            shp.Rotation = 0;
            shp.ThreeD.Visible = MsoTriState.msoFalse;

            // Text frames
            shp.TextFrame.TextRange.ParagraphFormat.Alignment = PowerPoint.PpParagraphAlignment.ppAlignLeft;
            shp.TextFrame.VerticalAnchor = MsoVerticalAnchor.msoAnchorTop;
            shp.TextFrame.TextRange.Font.Color.RGB = System.Drawing.Color.FromArgb(0, 0, 0).ToArgb();
            shp.TextFrame.TextRange.Font.Size = 12;
            shp.TextFrame.TextRange.Font.Name = "Courier New";
            shp.TextFrame.TextRange.Font.Bold = MsoTriState.msoFalse;
            shp.TextFrame.TextRange.Font.Italic = MsoTriState.msoFalse;
            shp.TextFrame.TextRange.Font.Underline = MsoTriState.msoFalse;
            //shp.TextFrame.TextRange.Text = "Here is some test text"

            // Add Tag: comment from deckify
            shp.Tags.Add(Constants.Tags.MDComment, Constants.Tags.ValueYes);
        }
        /// <summary>
        /// Checks the type of the selected shape.
        /// </summary>
        /// <param name="activeWindow">The active PowerPoint document window.</param>
        /// <returns>A string indicating the shape type (e.g., PICTURE, TABLE) or empty string.</returns>
        public string CheckActiveShapeType(PowerPoint.DocumentWindow activeWindow)
        {
            string sReturnString = "";

            // Get selected Objecttype
            if (activeWindow.Selection.Type == PowerPoint.PpSelectionType.ppSelectionShapes || activeWindow.Selection.Type == PowerPoint.PpSelectionType.ppSelectionText)
            {
                PowerPoint.ShapeRange selectedShapeRange = activeWindow.Selection.ShapeRange;
                if (selectedShapeRange.Count == 1)
                {
                    switch (selectedShapeRange.Type)
                    {
                        case Microsoft.Office.Core.MsoShapeType.msoPicture:
                            sReturnString = Constants.ShapeTypes.Picture;
                            break;
                        case Microsoft.Office.Core.MsoShapeType.msoTable:
                            sReturnString = Constants.ShapeTypes.Table;
                            break;
                        case Microsoft.Office.Core.MsoShapeType.msoPlaceholder:
                            switch (selectedShapeRange.PlaceholderFormat.Type)
                            {
                                case PowerPoint.PpPlaceholderType.ppPlaceholderBitmap:
                                    sReturnString = Constants.ShapeTypes.Picture;
                                    break;
                                case PowerPoint.PpPlaceholderType.ppPlaceholderTable:
                                    sReturnString = Constants.ShapeTypes.Table;
                                    break;
                                case PowerPoint.PpPlaceholderType.ppPlaceholderObject:
                                    switch (selectedShapeRange.PlaceholderFormat.ContainedType)
                                    {
                                        case Microsoft.Office.Core.MsoShapeType.msoPicture:
                                            sReturnString = Constants.ShapeTypes.Picture;
                                            break;
                                        case Microsoft.Office.Core.MsoShapeType.msoTable:
                                            sReturnString = Constants.ShapeTypes.Table;
                                            break;
                                    }
                                    break;
                            }
                            break;
                        default:
                            break;
                    }
                }
            }
            return sReturnString;
        }
        private void EnsureShapesSelected(PowerPoint.DocumentWindow activeWindow)
        {
            if (activeWindow.Selection.Type != PowerPoint.PpSelectionType.ppSelectionShapes)
            {
                throw new Exception("Please select valid shapes.");
            }
        }
    }
}
