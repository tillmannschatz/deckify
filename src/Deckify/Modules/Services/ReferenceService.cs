using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using Microsoft.Office.Core;
using Newtonsoft.Json.Linq;
using PowerPoint = Microsoft.Office.Interop.PowerPoint;
using Deckify.Common;

namespace Deckify.Modules.Services
{
    public class ReferenceService : IReferenceService
    {
        #region Slide Master References

        /// <summary>
        /// Creates a new link between the current slide and a slide master.
        /// </summary>
        /// <param name="pActive">The active PowerPoint presentation.</param>
        /// <param name="activeWindow">The active PowerPoint document window.</param>
        public void NewLink(PowerPoint.Presentation pActive, PowerPoint.DocumentWindow activeWindow)
        {
            PowerPoint.Slide sld = activeWindow.View.Slide;
            // Removed unnecessary Select()


            string sharePointLink = Microsoft.VisualBasic.Interaction.InputBox("In order to make the current slide a reusable Masterslide, please add below the 'Link to this Slide'", "Setup Masterslide");

            if (!string.IsNullOrEmpty(sharePointLink))
            {
                if (Deckify.Common.ReferenceHelpers.GetPPTURL(sharePointLink) == "") { MessageBox.Show("Invalid Link! Please ensure to select 'Link to this slide' with the option 'People with existing access'!"); }
                else { UpdatePPTSlide(pActive, sharePointLink, activeWindow); }
            }
        }

        /// <summary>
        /// Edits an existing link to a slide master on the current slide.
        /// </summary>
        /// <param name="pActive">The active PowerPoint presentation.</param>
        /// <param name="activeWindow">The active PowerPoint document window.</param>
        public void EditLink(PowerPoint.Presentation pActive, PowerPoint.DocumentWindow activeWindow)
        {
            PowerPoint.Slide sld = activeWindow.View.Slide;
            // Removed unnecessary Select()

            int SlideIndex = sld.SlideIndex;

            string sharePointLink = pActive.Slides[SlideIndex].Tags[Constants.Tags.SlideMasterReference];
            if (sharePointLink == "") { MessageBox.Show("No Reference Link to Master Slide found!"); }
            else
            {
                sharePointLink = Microsoft.VisualBasic.Interaction.InputBox("In order to make the current slide a reusable Masterslide, please add below the 'Link to this Slide'", "Setup Masterslide", sharePointLink);

                if (!string.IsNullOrEmpty(sharePointLink))
                {
                    if (Deckify.Common.ReferenceHelpers.GetPPTURL(sharePointLink) == "") { MessageBox.Show("Invalid Link! Please ensure to select 'Link to this slide' with the option 'People with existing access'!"); }
                    else
                    {
                        if (SlideIndex > 1)
                        {
                            pActive.Slides[SlideIndex].Delete();
                            pActive.Slides[SlideIndex - 1].Select();
                        }
                        UpdatePPTSlide(pActive, sharePointLink, activeWindow);
                    }
                }
            }
        }

        /// <summary>
        /// Updates the current slide content from the linked slide master.
        /// </summary>
        /// <param name="pActive">The active PowerPoint presentation.</param>
        /// <param name="activeWindow">The active PowerPoint document window.</param>
        public void UpdateSlide(PowerPoint.Presentation pActive, PowerPoint.DocumentWindow activeWindow)
        {
            PowerPoint.Slide sld = activeWindow.View.Slide;
            // Removed unnecessary Select()

            int SlideIndex = sld.SlideIndex;

            string sharePointLink = pActive.Slides[SlideIndex].Tags[Constants.Tags.SlideMasterReference];
            if (sharePointLink == "") { MessageBox.Show("No Reference Link to Master Slide found!"); }
            else
            {
                if (Deckify.Common.ReferenceHelpers.GetPPTURL(sharePointLink) == "") { MessageBox.Show("Invalid Link! Please use edit link functionality to correct link!"); }
                else
                {
                    if (SlideIndex > 1)
                    {
                        pActive.Slides[SlideIndex].Delete();
                        pActive.Slides[SlideIndex - 1].Select();
                    }
                    UpdatePPTSlide(pActive, sharePointLink, activeWindow);
                }
            }
        }



        private void UpdatePPTSlide(PowerPoint.Presentation pActive, string sharePointLink, PowerPoint.DocumentWindow activeWindow)
        {
            PowerPoint.Slide slaveslide = activeWindow.View.Slide;

            string presentationLink = Deckify.Common.ReferenceHelpers.GetPPTURL(sharePointLink);
            int slideID = Deckify.Common.ReferenceHelpers.GetPPTSlideID(sharePointLink);

            PowerPoint.Presentation pptPresentation = pActive.Application.Presentations.Open(
                presentationLink,
                MsoTriState.msoTrue,   // ReadOnly
                MsoTriState.msoFalse,  // Untitled
                MsoTriState.msoFalse    // WithoutWindow
            );

            // Select and prepare slide
             // Note: using pActive.Application to access standard FindBySlideID if available or iterate
             // Standard Interop Slides.FindBySlideID returns a Slide object directly.
            PowerPoint.Slide foundSlide = pptPresentation.Slides.FindBySlideID(slideID);
            // Removed unnecessary Select()

            int slideIndex = foundSlide.SlideIndex;
            
            PowerPoint.Slide sld = pptPresentation.Slides[slideIndex];
            sld.Tags.Delete(Constants.Tags.SlideMasterReference);
            sld.Tags.Add(Constants.Tags.SlideMasterReference, sharePointLink);

            PowerPoint.Shape shp = sld.Shapes.AddShape((MsoAutoShapeType)16, -220, 0, 200, 100);
            FormatShape(shp);
            shp.Fill.ForeColor.RGB = System.Drawing.Color.FromArgb(68, 114, 196).ToArgb();
            shp.TextFrame.TextRange.Text = "Attention, this is a linked slide.\n\nUse update button to update to newest version of master slide.";
            PowerPoint.Hyperlink hyperlink = shp.ActionSettings[PowerPoint.PpMouseActivation.ppMouseClick].Hyperlink;
            hyperlink.Address = sharePointLink;

            pptPresentation.Slides[slideIndex].Copy();
            sld.Tags.Delete(Constants.Tags.SlideMasterReference);
            shp.Delete();
            pptPresentation.Close();
            
            // Only select if not already active
            if (activeWindow.View.Slide.SlideIndex != slaveslide.SlideIndex)
            {
                slaveslide.Select();
            }
            pActive.Application.CommandBars.ExecuteMso("PasteSourceFormatting");
        }

        #endregion

        #region Object References

        /// <summary>
        /// Creates a new object link by copying the object's UUID to the clipboard.
        /// </summary>
        /// <param name="activeWindow">The active PowerPoint document window.</param>
        public void NewObjectLink(PowerPoint.DocumentWindow activeWindow)
        {
            PowerPoint.Selection selection = activeWindow.Selection;
            if (selection.ShapeRange.Count == 1)
            {
                if (selection.Type == PowerPoint.PpSelectionType.ppSelectionShapes)
                {
                    PowerPoint.Shape selectedShape = selection.ShapeRange[1];
                    string tagValue = GetOrCreateUuidTag(selectedShape);
                    Clipboard.SetText(tagValue);
                }
            }
        }

        /// <summary>
        /// Pastes an object linked to a source object via UUID.
        /// </summary>
        /// <param name="pActive">The active PowerPoint presentation.</param>
        /// <param name="activeWindow">The active PowerPoint document window.</param>
        public void PasteObject(PowerPoint.Presentation pActive, PowerPoint.DocumentWindow activeWindow)
        {
            PowerPoint.Slide sld = activeWindow.View.Slide;
            int activeSlideId = sld.SlideIndex;

            string uuid = PromptForUuid();
            if (string.IsNullOrEmpty(uuid)) return;

            FindShapeResult foundResult = FindShapeByUuid(pActive, uuid);

            if (foundResult.Shape != null)
            {
                foundResult.Shape.Copy();
                activeWindow.View.GotoSlide(activeSlideId);
                activeWindow.View.Paste();

                PowerPoint.ShapeRange pastedShapeRange = activeWindow.Selection.ShapeRange;
                PowerPoint.Shape pastedShape;
                if (pastedShapeRange[1].Type == MsoShapeType.msoGroup)
                {
                    pastedShape = pastedShapeRange[1].GroupItems[1];
                }
                else
                {
                    pastedShape = activeWindow.Selection.ShapeRange[1];
                }

                string tagValue = pastedShape.Tags[Constants.Tags.ObjReference];
                if (!string.IsNullOrEmpty(tagValue)) { pastedShape.Tags.Delete(Constants.Tags.ObjReference); }
                
                string tagName = Constants.Tags.ObjLinkedReference;
                pastedShape.Tags.Add(tagName, uuid);
            }
            else
            {
                MessageBox.Show("No object with the specified UUID was found.");
            }
        }

        /// <summary>
        /// Updates a linked object with the latest version from its source.
        /// </summary>
        /// <param name="pActive">The active PowerPoint presentation.</param>
        /// <param name="activeWindow">The active PowerPoint document window.</param>
        public void UpdateObject(PowerPoint.Presentation pActive, PowerPoint.DocumentWindow activeWindow)
        {
            PowerPoint.Selection selection = activeWindow.Selection;
            int activeSlideId = activeWindow.View.Slide.SlideIndex;

            string uuid = "";
            PowerPoint.Shape shapeToDelete = null;
            string tagName = Constants.Tags.ObjLinkedReference;
            if (selection.Type == PowerPoint.PpSelectionType.ppSelectionShapes)
            {
                PowerPoint.ShapeRange selectedShapes = selection.ShapeRange;
                if (selectedShapes.Count == 1)
                {
                    shapeToDelete = selectedShapes[1];
                    if (shapeToDelete.Type == MsoShapeType.msoGroup)
                    {
                        for (int j = 1; j <= shapeToDelete.GroupItems.Count; j++)
                        {
                            uuid = GetUuidTag(shapeToDelete.GroupItems[j], tagName);
                            if (!string.IsNullOrEmpty(uuid)) break;
                        }
                        if (string.IsNullOrEmpty(uuid)) return;
                    }
                    else
                    {
                        uuid = GetUuidTag(shapeToDelete, tagName);
                        if (string.IsNullOrEmpty(uuid)) return;
                    }
                }
            }

            if (string.IsNullOrEmpty(uuid) || shapeToDelete == null)
            {
                 // ensure we don't continue if no valid object found
                 return;
            }

            FindShapeResult foundResult = FindShapeByUuid(pActive, uuid);

            if (foundResult.Shape != null)
            {
                foundResult.Shape.Copy();
                
                // Ensure we are on the correct slide before deleting/pasting
                if (activeWindow.View.Slide.SlideIndex != activeSlideId)
                {
                    activeWindow.View.GotoSlide(activeSlideId);
                }
                
                // Safe delete using reference
                try 
                {
                    shapeToDelete.Delete(); 
                }
                catch 
                { 
                    // Fallback or log if delete fails (e.g. user changed selection manually)
                    Deckify.Common.Logger.LogError(new Exception("Failed to delete original shape during update"), "UpdateObject");
                }

                activeWindow.View.Paste();

                PowerPoint.ShapeRange pastedShapeRange = activeWindow.Selection.ShapeRange;
                PowerPoint.Shape pastedShape;
                if (pastedShapeRange[1].Type == MsoShapeType.msoGroup)
                {
                    pastedShape = pastedShapeRange[1].GroupItems[1];
                }
                else
                {
                    pastedShape = activeWindow.Selection.ShapeRange[1];
                }

                string tagValue = pastedShape.Tags[Constants.Tags.ObjReference];
                if (!string.IsNullOrEmpty(tagValue)) { pastedShape.Tags.Delete(Constants.Tags.ObjReference); }

                pastedShape.Tags.Add(tagName, uuid);
            }
            else
            {
                MessageBox.Show("No object with the specified UUID was found.");
            }
        }

        #endregion

        #region Helpers

        private void FormatShape(PowerPoint.Shape shp)
        {
            shp.Line.Visible = MsoTriState.msoFalse;
            shp.Fill.Visible = MsoTriState.msoTrue;
            shp.Fill.Solid();
            shp.Fill.ForeColor.RGB = System.Drawing.Color.FromArgb(128, 255, 255).ToArgb();
            shp.Fill.ForeColor.TintAndShade = 0;
            shp.Fill.Transparency = 0;
            shp.Shadow.Type = (MsoShadowType)21;
            shp.Reflection.Type = MsoReflectionType.msoReflectionTypeNone;
            shp.Glow.Radius = 0;
            shp.SoftEdge.Type = MsoSoftEdgeType.msoSoftEdgeTypeNone;
            shp.Rotation = 0;
            shp.ThreeD.Visible = MsoTriState.msoFalse;
            shp.TextFrame.TextRange.ParagraphFormat.Alignment = PowerPoint.PpParagraphAlignment.ppAlignLeft;
            shp.TextFrame.VerticalAnchor = MsoVerticalAnchor.msoAnchorTop;
            shp.TextFrame.TextRange.Font.Color.RGB = System.Drawing.Color.FromArgb(0, 0, 0).ToArgb();
            shp.TextFrame.TextRange.Font.Size = 12;
            shp.TextFrame.TextRange.Font.Name = "Courier New";
            shp.TextFrame.TextRange.Font.Bold = MsoTriState.msoFalse;
            shp.TextFrame.TextRange.Font.Italic = MsoTriState.msoFalse;
            shp.TextFrame.TextRange.Font.Underline = MsoTriState.msoFalse;
            shp.Tags.Add(Constants.Tags.MDComment, Constants.Tags.ValueYes);
        }

        private string GetOrCreateUuidTag(PowerPoint.Shape shp)
        {
            string tagName = Constants.Tags.ObjReference;
            if (shp.Tags.Count > 0)
            {
                for (int j = 1; j <= shp.Tags.Count; j++)
                {
                    if (shp.Tags.Name(j) == tagName && shp.Tags.Value(j) != "")
                    {
                        return shp.Tags.Value(j);
                    }
                }
            }
            string uuid = Guid.NewGuid().ToString();
            shp.Tags.Add(tagName, uuid);
            return uuid;
        }

        private string GetUuidTag(PowerPoint.Shape shp, string tagName)
        {
            if (shp.Tags.Count > 0)
            {
                for (int j = 1; j <= shp.Tags.Count; j++)
                {
                    if (shp.Tags.Name(j) == tagName)
                    {
                        return shp.Tags.Value(j);
                    }
                }
            }
            MessageBox.Show("Selected object has no UUID Tag.");
            return null;
        }

        private class FindShapeResult
        {
            public PowerPoint.Shape Shape { get; set; }
            public PowerPoint.Slide Slide { get; set; }
        }

        private FindShapeResult FindShapeByUuid(PowerPoint.Presentation activePresentation, string uuid)
        {
            foreach (PowerPoint.Slide slide in activePresentation.Slides)
            {
                foreach (PowerPoint.Shape shape in slide.Shapes)
                {
                    if (shape.Tags[Constants.Tags.ObjReference] == uuid)
                    {
                        return new FindShapeResult { Shape = shape, Slide = slide };
                    }
                }
            }
            return new FindShapeResult { Shape = null, Slide = null };
        }

        private string PromptForUuid()
        {
            using (Form prompt = new Form())
            {
                prompt.Width = 500;
                prompt.Height = 150;
                prompt.Text = "Enter UUID";

                Label textLabel = new Label() { Left = 50, Top = 20, Text = "Enter the UUID of the object:" };
                System.Windows.Forms.TextBox textBox = new System.Windows.Forms.TextBox() { Left = 50, Top = 50, Width = 400 };
                System.Windows.Forms.Button confirmation = new System.Windows.Forms.Button() { Text = "Ok", Left = 350, Width = 100, Top = 70, DialogResult = DialogResult.OK };
                confirmation.Click += (sender, e) => { prompt.Close(); };

                prompt.Controls.Add(textBox);
                prompt.Controls.Add(confirmation);
                prompt.Controls.Add(textLabel);
                prompt.AcceptButton = confirmation;

                return prompt.ShowDialog() == DialogResult.OK ? textBox.Text : "";
            }
        }
        #endregion
    }
}
