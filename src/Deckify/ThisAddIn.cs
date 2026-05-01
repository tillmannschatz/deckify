using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using PowerPoint = Microsoft.Office.Interop.PowerPoint;
using Office = Microsoft.Office.Core;
using System.Windows.Forms;
using System.ComponentModel.Design;
using Deckify;
using Microsoft.Office.Core;
using Outlook = Microsoft.Office.Interop.Outlook; // For this Add-->Reference-->Extensions-->Microsoft.Office.Interop.Outlook needs to be activated first!
using System.Runtime.InteropServices;
using Microsoft.Office.Interop.Outlook;
using Microsoft.Office.Interop.PowerPoint;
using Microsoft.Extensions.DependencyInjection;
using Deckify.Modules.Services;
using Deckify.Modules.Mirror;
using Deckify.Modules.AboutManager;
using System.Threading.Tasks;

namespace Deckify
{
    public partial class ThisAddIn
    {
        // Declare instance of Ribbon as members of the ThisAddIn class.
        public Microsoft.Office.Core.IRibbonUI _ribbonUI;
        protected override Microsoft.Office.Core.IRibbonExtensibility CreateRibbonExtensibilityObject()
        {
            return new Ribbon();
        }

        // make Ribbon accessible from other classes
        Ribbon ribbon;

        // Declare instances of myCustomTaskpane and mySettingsPane as members of the ThisAddIn class. --> These are added via Add->New Item->User Control
        private class TaskPaneSet
        {
            public Microsoft.Office.Tools.CustomTaskPane AboutPane { get; set; }
            public Microsoft.Office.Tools.CustomTaskPane SettingsPane { get; set; }
            public Microsoft.Office.Tools.CustomTaskPane ScreenshotPane { get; set; }
            public Microsoft.Office.Tools.CustomTaskPane ColorsPane { get; set; }
        }


        private readonly Dictionary<int, TaskPaneSet> taskPaneSets = new();
        public Microsoft.Office.Tools.CustomTaskPane myCustomTaskPane;
        public Microsoft.Office.Tools.CustomTaskPane mySettingsPane;
        public Microsoft.Office.Tools.CustomTaskPane myScreenshotPane;
        public Microsoft.Office.Tools.CustomTaskPane myColorsPane;



        // Other public variables
        public string groupTag;

        private void ThisAddIn_Startup(object sender, System.EventArgs e)
        {
            // Register event handlers (see: https://docs.microsoft.com/en-us/visualstudio/vsto/how-to-create-event-handlers-in-office-projects?view=vs-2022)
            this.Application.WindowSelectionChange += Application_WindowSelectionChange;
            this.Application.WindowActivate += Application_WindowActivate;
            this.Application.AfterPresentationOpen += Application_AfterPresentationOpen;
            this.Application.AfterNewPresentation += Application_AfterNewPresentation;

            // Register Ribbon
            ribbon = new Ribbon();

            ConfigureServices();

            EnsureActiveWindowTaskPanesInitialized();

            StartAutoUpdateCheck();
        }

        private void StartAutoUpdateCheck()
        {
            // Fire-and-forget: a slow GitHub poll or download must not block the
            // VSTO startup sequence. Errors are logged inside the checker but we
            // also wrap here in case the checker itself throws synchronously.
            _ = Task.Run(async () =>
            {
                try
                {
                    // Channel-migration takes precedence: existing Inno-installed users
                    // have HKCU\…\Manifest = file:// which breaks ClickOnce-native update.
                    // Show the migration toast once per session and skip the auto-check —
                    // post-migration, the next session will pick up the new flow normally.
                    var migration = ServiceProvider.GetService(typeof(IUpdateChannelMigration)) as IUpdateChannelMigration;
                    var notification = ServiceProvider.GetService(typeof(IUpdateNotificationService)) as IUpdateNotificationService;
                    if (migration != null && notification != null && migration.NeedsMigration())
                    {
                        notification.ShowChannelMigrationAvailable();
                        return;
                    }

                    var checker = ServiceProvider.GetService(typeof(IAutoUpdateChecker)) as IAutoUpdateChecker;
                    if (checker != null)
                        await checker.RunStartupCheckAsync();
                }
                catch (System.Exception ex)
                {
                    Deckify.Common.Logger.LogError(ex, "AutoUpdateChecker startup");
                }
            });
        }

        public IServiceProvider ServiceProvider { get; private set; }

        private void ConfigureServices()
        {
            var services = new ServiceCollection();
            
            services.AddSingleton(typeof(IEmailService), typeof(EmailService));
            services.AddSingleton(typeof(IShapeService), typeof(ShapeService));
            services.AddSingleton(typeof(Deckify.Modules.Services.IReferenceService), typeof(ReferenceService));
            services.AddSingleton(typeof(ILanguageService), typeof(LanguageService));
            services.AddSingleton(typeof(IPresentationService), typeof(PresentationService));
            services.AddSingleton(typeof(Deckify.Modules.Colors.IPaletteService), typeof(Deckify.Modules.Colors.PaletteService));
            services.AddSingleton(typeof(Deckify.Modules.Mirror.IMirrorService), provider => 
            {
                var service = new Deckify.Modules.Mirror.MirrorService(this.Application);
                service.Initialize();
                return service;
            });
            services.AddSingleton(typeof(Deckify.Modules.AboutManager.IUpdateService), typeof(Deckify.Modules.AboutManager.UpdateService));
            services.AddSingleton(typeof(Deckify.Modules.AboutManager.IUpdateNotificationService), typeof(Deckify.Modules.AboutManager.UpdateNotificationService));
            services.AddSingleton(typeof(Deckify.Modules.AboutManager.IAutoUpdateSettings), typeof(Deckify.Modules.AboutManager.DefaultAutoUpdateSettings));
            services.AddSingleton(typeof(Deckify.Modules.AboutManager.IAutoUpdateChecker), typeof(Deckify.Modules.AboutManager.AutoUpdateChecker));
            services.AddSingleton(typeof(Deckify.Modules.AboutManager.IUpdateChannelMigration), typeof(Deckify.Modules.AboutManager.UpdateChannelMigration));

            ServiceProvider = services.BuildServiceProvider();
        }

        // Define opening existing Presentation event
        void Application_AfterPresentationOpen(PowerPoint.Presentation Pres)
        {
            //this.Application.ActiveWindow.ViewType = PowerPoint.PpViewType.ppViewNormal; --> Normal is with Notes!

        }

        // Define opening new Presentation event
        void Application_AfterNewPresentation(PowerPoint.Presentation Pres)
        {
            //this.Application.ActiveWindow.ViewType = PowerPoint.PpViewType.ppViewNormal;
        }

        // Define Active Presentation Window changed event
        void Application_WindowActivate(PowerPoint.Presentation Pres, PowerPoint.DocumentWindow Wn)
        {
            EnsureTaskPanesInitializedForWindow(Wn);

            if (myCustomTaskPane != null) myCustomTaskPane.Visible = false;
            if (mySettingsPane != null) mySettingsPane.Visible = false;
            if (myScreenshotPane != null) myScreenshotPane.Visible = false;
            if (myColorsPane != null) myColorsPane.Visible = false;
        }


        // Define SelectionChanged (Slide, Textbox, ...) event
        private void Application_WindowSelectionChange(PowerPoint.Selection Sel)
        {
            if (DeckifySettings.Default.AutoFocus) { Globals.ThisAddIn._ribbonUI.ActivateTab(Ribbon.ActiveTabId); }
            //CheckActiveShapeType();
            DynamicGroupActivateDeactivate();


        }

        private void ThisAddIn_Shutdown(object sender, System.EventArgs e)
        {
        }

        #region VSTO generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InternalStartup()
        {
            this.Startup += new System.EventHandler(ThisAddIn_Startup);
            this.Shutdown += new System.EventHandler(ThisAddIn_Shutdown);
        }

        #endregion

        #region Custom code (e.g. Helper functions)

        

        public void RefreshRibbon(string Tag)
        {
            if (this.groupTag == Tag) return;
            groupTag = Tag;
            try { Globals.ThisAddIn._ribbonUI.Invalidate(); }
            catch { }
        }

        public void EnsureActiveWindowTaskPanesInitialized()
        {
            PowerPoint.DocumentWindow activeWindow = null;
            try
            {
                activeWindow = this.Application.ActiveWindow;
            }
            catch (COMException)
            {
                // No active window available yet.
            }

            EnsureTaskPanesInitializedForWindow(activeWindow);
        }

        public void EnsureTaskPanesInitializedForWindow(PowerPoint.DocumentWindow window)
        {
            if (window == null) { return; }

            int windowHandle = window.HWND;
            if (!taskPaneSets.TryGetValue(windowHandle, out TaskPaneSet paneSet))
            {
                var aboutPane = this.CustomTaskPanes.Add(new TaskPaneAbout(), "About", window);
                aboutPane.Visible = false;

                var settingsPane = this.CustomTaskPanes.Add(new TaskPaneSettings(), "Settings", window);
                settingsPane.Visible = false;

                var screenshotPane = this.CustomTaskPanes.Add(new TaskPaneScreenshot(), "Screenshot", window);
                screenshotPane.Visible = false;

                var colorsPane = this.CustomTaskPanes.Add(new Deckify.Modules.Colors.TaskPaneColors(), "Color Palette", window);
                colorsPane.Visible = false;

                paneSet = new TaskPaneSet
                {
                    AboutPane = aboutPane,
                    SettingsPane = settingsPane,
                    ScreenshotPane = screenshotPane,
                    ColorsPane = colorsPane
                };


                taskPaneSets.Add(windowHandle, paneSet);
            }

            myCustomTaskPane = paneSet.AboutPane;
            mySettingsPane = paneSet.SettingsPane;
            myScreenshotPane = paneSet.ScreenshotPane;
            myColorsPane = paneSet.ColorsPane;
        }


        public void TogglePaneVisibility(Microsoft.Office.Tools.CustomTaskPane pane, int width)
        {
            if (pane == null) { return; }

            bool makeVisible = !pane.Visible;
            pane.Visible = makeVisible;
            if (makeVisible) { pane.Width = width; }
        }
        public void DynamicGroupActivateDeactivate()
        {
            if (DeckifySettings.Default.DynamicUI)
            {
                if (this.Application.ActiveWindow.Selection.Type == PpSelectionType.ppSelectionShapes || this.Application.ActiveWindow.Selection.Type == PpSelectionType.ppSelectionText)
                {
                    // check that only one item is selected!
                    try
                    {
                        PowerPoint.ShapeRange selectedShapeRange = Globals.ThisAddIn.Application.ActiveWindow.Selection.ShapeRange;
                        string ActiveShapeType = "";
                        if (selectedShapeRange.Count == 1)
                        {
                            var shapeService = Globals.ThisAddIn.ServiceProvider.GetService<IShapeService>();
                            ActiveShapeType = shapeService.CheckActiveShapeType(this.Application.ActiveWindow);
                        }

                        if (ActiveShapeType == "TABLE") { RefreshRibbon("DynamicTableGroup"); }
                        else if (ActiveShapeType == "PICTURE") { RefreshRibbon("DynamicPictureGroup"); }
                        else { RefreshRibbon(""); }
                    }
                    catch (System.Exception ex_dynamic)
                    {
                        Deckify.Common.Logger.LogError(ex_dynamic, "DynamicGroupActivateDeactivate");
                    }
                }
                else { RefreshRibbon(""); }
            }
            else { RefreshRibbon("Dynamic"); }
        }





        #endregion
    }
}
