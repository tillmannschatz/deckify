using System.Threading.Tasks;

namespace Deckify.Modules.AboutManager
{
    // One-time channel migration for users installed via the bundled Inno installer.
    // Their HKCU\…\Addins\Deckify\Manifest points at a file:// URL inside
    // %LocalAppData%\deckify-installer-bundle\, which breaks ClickOnce-native
    // auto-update. Migration repoints it at the gh-pages URL via VSTOInstaller.exe.
    public interface IUpdateChannelMigration
    {
        bool NeedsMigration();
        Task<bool> RunMigrationAsync();
    }
}
