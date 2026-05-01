namespace Deckify.Modules.AboutManager
{
    public interface IUpdateNotificationService
    {
        void ShowUpdateAvailable(string version, string releaseNotes);
        void ShowUpdateInstalling();
        void ShowUpdateInstalled();
        void ShowUpdateError(string message);
        void ShowChannelMigrationAvailable();
    }
}
