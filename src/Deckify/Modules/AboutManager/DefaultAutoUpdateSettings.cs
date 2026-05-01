using System;

namespace Deckify.Modules.AboutManager
{
    public sealed class DefaultAutoUpdateSettings : IAutoUpdateSettings
    {
        public bool AutoCheckEnabled => DeckifySettings.Default.AutoCheckUpdate;

        public DateTime LastCheckUtc
        {
            // Stored value is local-time (existing manual flow uses DateTime.Now);
            // convert to UTC for arithmetic. New writes are UTC.
            get
            {
                var v = Properties.Settings.Default.LastUpdateCheck;
                if (v == default) return default;
                return v.Kind == DateTimeKind.Utc ? v : v.ToUniversalTime();
            }
            set { Properties.Settings.Default.LastUpdateCheck = value; }
        }

        public void Save() => Properties.Settings.Default.Save();
    }
}
