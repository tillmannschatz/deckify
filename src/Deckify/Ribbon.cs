// in "Deckify.csproj" --> <LangVersion>12.0</LangVersion> added to support c# 12 (.NET 8.x required)
using Microsoft.Office.Core;
using Microsoft.Office.Tools;
using System;
using Newtonsoft.Json.Linq; // Required for JSON Parsing
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using Office = Microsoft.Office.Core;
using PowerPoint = Microsoft.Office.Interop.PowerPoint;
using Outlook = Microsoft.Office.Interop.Outlook; // For this Add-->Reference-->Extensions-->Microsoft.Office.Interop.Outlook needs to be activated first!
using System.Xml.Linq;
using Microsoft.Office.Interop.PowerPoint;
using System.Linq.Expressions;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using Microsoft.Office.Interop.Outlook;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Rebar;
using System.Security.Policy;
using HWND = System.IntPtr; // Required for Screenshot (Window identification)
using System.Drawing.Imaging; // Required for Screenshot (Save Picture)
using System.Net.NetworkInformation;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;
using Deckify.Modules.Services;
using Microsoft.Extensions.DependencyInjection;
using Deckify.Modules.Mirror;

namespace Deckify
{
    [ComVisible(true)]
    public class Ribbon : Office.IRibbonExtensibility
    {
        // We don't need the private ribbon here, since we registered it as a global variable in "ThisAddin.cs"
        //private Office.IRibbonUI ribbon;
        // Declare variables to be used within this class

        public Ribbon()
        {

        }

        /// <summary>
        /// Tab-ID des deckify-Ribbon-Tabs fuer das aktuell laufende Assembly.
        /// Production-Build (Deckify.dll): "deckify".
        /// Debug-Side-by-Side-Build (Deckify.Debug.dll): "deckify-debug".
        /// Wird vom Ribbon-XML-Loader und allen ActivateTab()-Aufrufen genutzt,
        /// damit ein parallel installiertes Production-Add-In den Debug-Tab
        /// nicht versehentlich aktiviert (und umgekehrt).
        /// </summary>
        public static string ActiveTabId =>
            Assembly.GetExecutingAssembly().GetName().Name == "Deckify.Debug"
                ? "deckify-debug"
                : "deckify";

        #region IRibbonExtensibility Members

        public string GetCustomUI(string ribbonID)
        {
            string resourceName =
                Assembly.GetExecutingAssembly().GetName().Name == "Deckify.Debug"
                    ? "Deckify.Ribbon.Debug.xml"
                    : "Deckify.Ribbon.xml";
            return GetResourceText(resourceName);
        }

        #endregion

        #region Ribbon Callbacks
        //Create callback methods here. For more information about adding callback methods, visit https://go.microsoft.com/fwlink/?LinkID=271226

        public void Ribbon_Load(Office.IRibbonUI ribbonUI)
        {
            // Use Globals since we registered the Ribbon as global in ThisAddIn.cs
            Globals.ThisAddIn._ribbonUI = ribbonUI;
            Globals.ThisAddIn._ribbonUI.ActivateTab(ActiveTabId);
        }
        public System.Drawing.Image GetImage(string ImageName)
        {
            // Callback to retrieve custom button images
            return (System.Drawing.Image)Properties.Resources.ResourceManager.GetObject(ImageName);
        }
        public void OpenAbout(Office.IRibbonControl control)
        {
            Globals.ThisAddIn.EnsureActiveWindowTaskPanesInitialized();
            Globals.ThisAddIn.TogglePaneVisibility(Globals.ThisAddIn.myCustomTaskPane, 300);
        }
        public void OpenSettings(Office.IRibbonControl control)
        {
            Globals.ThisAddIn.EnsureActiveWindowTaskPanesInitialized();
            Globals.ThisAddIn.TogglePaneVisibility(Globals.ThisAddIn.mySettingsPane, 300);

        }

        public void OpenScreenshot(Office.IRibbonControl control)
        {
            Globals.ThisAddIn.EnsureActiveWindowTaskPanesInitialized();
            Globals.ThisAddIn.TogglePaneVisibility(Globals.ThisAddIn.myScreenshotPane, 300);
        }
        public void OpenLogFile(Office.IRibbonControl control)
        {
             Deckify.Common.ErrorHandler.SafeExecute(() =>
             {
                 string logPath = Path.Combine(Path.GetTempPath(), Deckify.Common.Logger.LogFileName);
                 if (File.Exists(logPath))
                 {
                     Process.Start(logPath);
                 }
                 else
                 {
                     MessageBox.Show("Log file not found. It will be created when the first error occurs.", "Log File Missing");
                 }
             }, "Open Log File");
        }
        public void OpenColorsPane(Office.IRibbonControl control)
        {
            Globals.ThisAddIn.EnsureActiveWindowTaskPanesInitialized();
            Globals.ThisAddIn.TogglePaneVisibility(Globals.ThisAddIn.myColorsPane, 300);
        }

        public void EmailPresentation(Office.IRibbonControl control)
        {
            ExecuteServiceAction<IEmailService>(service => service.EmailPresentation(Globals.ThisAddIn.Application.ActivePresentation), "Email Presentation");
        }
        public void OpenFileLocation(Office.IRibbonControl control)
        {
            Deckify.Common.ErrorHandler.SafeExecute(() =>
            {
                // Get the current PowerPoint application instance
                PowerPoint.Presentation pActive = Globals.ThisAddIn.Application.ActivePresentation;
                var presentationService = Globals.ThisAddIn.ServiceProvider.GetService<IPresentationService>();
                presentationService.OpenFileLocation(pActive);
            }, "Open File Location");
        }
        public void EmailSelectedSlides(Office.IRibbonControl control)
        {
            Deckify.Common.ErrorHandler.SafeExecute(() =>
            {
                PowerPoint.Presentation pActive = Globals.ThisAddIn.Application.ActivePresentation;
                var emailService = Globals.ThisAddIn.ServiceProvider.GetService<IEmailService>();
                var presentationService = Globals.ThisAddIn.ServiceProvider.GetService<IPresentationService>();
                emailService.EmailSelectedSlides(pActive, presentationService.CreateSelectedSlidesExtract);
            }, "Email Selected Slides");
        }
        public void ExportSlides(Office.IRibbonControl control)
        {
            ExecuteServiceAction<IPresentationService>(service => service.CreateSelectedSlidesExtract(Globals.ThisAddIn.Application.ActivePresentation, "_EXTRACT", closeExtractPresentation: false), "Export Slides");
        }
        public void SetLanguage(Office.IRibbonControl control)
        {
            Deckify.Common.ErrorHandler.SafeExecute(() =>
            {
                MsoLanguageID languageID;
                if (control.Tag == "enUK") { languageID = MsoLanguageID.msoLanguageIDEnglishUK; }
                else if (control.Tag == "deDE") { languageID = MsoLanguageID.msoLanguageIDGerman; }
                else if (control.Tag == "frFR") { languageID = MsoLanguageID.msoLanguageIDFrench; }
                else if (control.Tag == "enUS") { languageID = MsoLanguageID.msoLanguageIDEnglishUS; }
                else { languageID = MsoLanguageID.msoLanguageIDEnglishUK; }

                var languageService = Globals.ThisAddIn.ServiceProvider.GetService<ILanguageService>();
                languageService.SetLanguage(Globals.ThisAddIn.Application.ActivePresentation, languageID);
            }, "Set Language");
        }
        public void AddCommentSticker(Office.IRibbonControl control)
        {
            ExecuteShapeAction(service => service.AddCommentSticker(Globals.ThisAddIn.Application.ActiveWindow, control.Tag), "Add Comment Sticker");
        }
        public void MakeSameHeight(Office.IRibbonControl control)
        {
            ExecuteShapeAction(service => service.MakeSameHeight(Globals.ThisAddIn.Application.ActiveWindow), "Make Same Height");
        }
        public void MakeSameWidth(Office.IRibbonControl control)
        {
            ExecuteShapeAction(service => service.MakeSameWidth(Globals.ThisAddIn.Application.ActiveWindow), "Make Same Width");
        }
        public void MakeSameSize(Office.IRibbonControl control)
        {
            ExecuteShapeAction(service => service.MakeSameSize(Globals.ThisAddIn.Application.ActiveWindow), "Make Same Size");
        }
        public void LockRatio(Office.IRibbonControl control, bool pressed)
        {
            ExecuteShapeAction(service => 
            {
                service.LockRatio(Globals.ThisAddIn.Application.ActiveWindow, pressed);
                Globals.ThisAddIn._ribbonUI.InvalidateControl("LockAspectRatioButton");
            }, "Lock Ratio");
        }

        public string GetLockRatioLabel(Office.IRibbonControl control)
        {
             try
             {
                 var shapeService = Globals.ThisAddIn.ServiceProvider.GetService<IShapeService>();
                 if (shapeService == null) return "Lock";

                 bool locked = shapeService.IsRatioLocked(Globals.ThisAddIn.Application.ActiveWindow);
                 return locked ? "Unlock" : "Lock";
             }
             catch 
             {
                 return "Lock";
             }
        }

        public bool GetLockRatioPressed(Office.IRibbonControl control)
        {
             try
             {
                 var shapeService = Globals.ThisAddIn.ServiceProvider.GetService<IShapeService>();
                 if (shapeService == null) return false;

                 return shapeService.IsRatioLocked(Globals.ThisAddIn.Application.ActiveWindow);
             }
             catch
             {
                 return false;
             }
        }

        public bool GetLockRatioEnabled(Office.IRibbonControl control)
        {
             // Enable only if shapes are selected
             try
             {
                 if (Globals.ThisAddIn.Application.ActiveWindow.Selection.Type == PowerPoint.PpSelectionType.ppSelectionShapes)
                 {
                     return true;
                 }
                 return false;
             }
             catch
             {
                 return false;
             }
        }
        public bool GetVisible(Office.IRibbonControl control)
        {
            try
            {
                if (string.IsNullOrEmpty(Globals.ThisAddIn.groupTag)) return false;
                return control.Tag == Globals.ThisAddIn.groupTag || Globals.ThisAddIn.groupTag == "Dynamic";
            }
            catch
            {
                return false;
            }
        }
        public bool GetDevelopmentVisibility(IRibbonControl control)
        {
            // Callback for getting Development visibility of Ribbon Items
            return DeckifySettings.Default.DevMode;
        }
        public void RemoveTransitions(Office.IRibbonControl control)
        {
            ExecuteShapeAction(service => service.RemoveTransitions(Globals.ThisAddIn.Application.ActivePresentation), "Remove Transitions");
        }
        public void RemoveAnimations(Office.IRibbonControl control)
        {
            ExecuteShapeAction(service => service.RemoveAnimations(Globals.ThisAddIn.Application.ActivePresentation), "Remove Animations");
        }
        public void ObjectsRemoveSpacingHorizontal(Office.IRibbonControl control)
        {
            ExecuteShapeAction(service => service.ObjectsRemoveSpacingHorizontal(Globals.ThisAddIn.Application.ActiveWindow), "Remove Horizontal Spacing");
        }
        public void ObjectsIncreaseSpacingHorizontal(Office.IRibbonControl control)
        {
            ExecuteShapeAction(service => service.ObjectsIncreaseSpacingHorizontal(Globals.ThisAddIn.Application.ActiveWindow), "Increase Horizontal Spacing");
        }
        public void ObjectsDecreaseSpacingHorizontal(Office.IRibbonControl control)
        {
            ExecuteShapeAction(service => service.ObjectsDecreaseSpacingHorizontal(Globals.ThisAddIn.Application.ActiveWindow), "Decrease Horizontal Spacing");
        }
        public void ObjectsRemoveSpacingVertical(Office.IRibbonControl control)
        {
            ExecuteShapeAction(service => service.ObjectsRemoveSpacingVertical(Globals.ThisAddIn.Application.ActiveWindow), "Remove Vertical Spacing");
        }
        public void ObjectsIncreaseSpacingVertical(Office.IRibbonControl control)
        {
            ExecuteShapeAction(service => service.ObjectsIncreaseSpacingVertical(Globals.ThisAddIn.Application.ActiveWindow), "Increase Vertical Spacing");
        }
        public void ObjectsDecreaseSpacingVertical(Office.IRibbonControl control)
        {
            ExecuteShapeAction(service => service.ObjectsDecreaseSpacingVertical(Globals.ThisAddIn.Application.ActiveWindow), "Decrease Vertical Spacing");
        }
        public void NewLink(Office.IRibbonControl control)
        {
            ExecuteReferenceAction(service => service.NewLink(Globals.ThisAddIn.Application.ActivePresentation, Globals.ThisAddIn.Application.ActiveWindow), "New Link");
        }
        public void EditLink(Office.IRibbonControl control)
        {
            ExecuteReferenceAction(service => service.EditLink(Globals.ThisAddIn.Application.ActivePresentation, Globals.ThisAddIn.Application.ActiveWindow), "Edit Link");
        }
        public void UpdateSlide(Office.IRibbonControl control)
        {
            ExecuteReferenceAction(service => service.UpdateSlide(Globals.ThisAddIn.Application.ActivePresentation, Globals.ThisAddIn.Application.ActiveWindow), "Update Slide");
        }
        public void NewObjectLink(Office.IRibbonControl control)
        {
            ExecuteReferenceAction(service => service.NewObjectLink(Globals.ThisAddIn.Application.ActiveWindow), "New Object Link");
        }
        public void PasteObject(Office.IRibbonControl control)
        {
            ExecuteReferenceAction(service => service.PasteObject(Globals.ThisAddIn.Application.ActivePresentation, Globals.ThisAddIn.Application.ActiveWindow), "Paste Object");
        }
        public void UpdateObject(Office.IRibbonControl control)
        {
            ExecuteReferenceAction(service => service.UpdateObject(Globals.ThisAddIn.Application.ActivePresentation, Globals.ThisAddIn.Application.ActiveWindow), "Update Object");
        }

        public void CopyMirror(Office.IRibbonControl control)
        {
            ExecuteServiceAction<IMirrorService>(service => 
            {
                var selection = Globals.ThisAddIn.Application.ActiveWindow.Selection;
                if (selection.Type == PowerPoint.PpSelectionType.ppSelectionShapes || selection.Type == PowerPoint.PpSelectionType.ppSelectionText)
                {
                     if (selection.ShapeRange.Count == 1)
                     {
                         var shape = selection.ShapeRange[1];
                         service.TagShapeAsMirror(shape);
                         shape.Copy();
                     }
                     else
                     {
                         MessageBox.Show("Please select a single shape to mirror.");
                     }
                }
            }, "Copy Mirror");
        }

        public void PasteMirror(Office.IRibbonControl control)
        {
             Deckify.Common.ErrorHandler.SafeExecute(() =>
             {
                 Globals.ThisAddIn.Application.ActiveWindow.View.Paste();
             }, "Paste Mirror");
        }

        #endregion

        #region Helpers

        private void ExecuteServiceAction<T>(Action<T> action, string actionName)
        {
            Deckify.Common.ErrorHandler.SafeExecute(() =>
            {
                var service = Globals.ThisAddIn.ServiceProvider.GetService<T>();
                action(service);
            }, actionName);
        }

        private void ExecuteShapeAction(Action<IShapeService> action, string actionName)
        {
            ExecuteServiceAction(action, actionName);
        }

        private void ExecuteReferenceAction(Action<IReferenceService> action, string actionName)
        {
            ExecuteServiceAction(action, actionName);
        }

        private static string GetResourceText(string resourceName)
        {
            Assembly asm = Assembly.GetExecutingAssembly();
            string[] resourceNames = asm.GetManifestResourceNames();
            for (int i = 0; i < resourceNames.Length; ++i)
            {
                if (string.Compare(resourceName, resourceNames[i], StringComparison.OrdinalIgnoreCase) == 0)
                {
                    using (StreamReader resourceReader = new StreamReader(asm.GetManifestResourceStream(resourceNames[i])))
                    {
                        if (resourceReader != null)
                        {
                            return resourceReader.ReadToEnd();
                        }
                    }
                }
            }
            return null;
        }


        #endregion

    }
}
