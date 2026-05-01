using System;

namespace Deckify.Modules.AboutManager
{
    // Abstraction over the persisted user settings so AutoUpdateChecker is testable
    // without ApplicationSettingsBase (which lives in the VSTO project and depends on
    // app.config plumbing that's awkward to spin up under xUnit).
    public interface IAutoUpdateSettings
    {
        bool AutoCheckEnabled { get; }
        DateTime LastCheckUtc { get; set; }
        void Save();
    }
}
