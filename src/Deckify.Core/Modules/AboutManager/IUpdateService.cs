using System.Threading;
using System.Threading.Tasks;

namespace Deckify.Modules.AboutManager
{
    public interface IUpdateService
    {
        Task<UpdateInfo> CheckForUpdatesAsync(CancellationToken ct = default);

        // Triggers ClickOnce-native update via ApplicationDeployment.Update(). Returns
        // true if a new version was downloaded into the ClickOnce cache and is staged
        // for activation on the next PowerPoint launch. Returns false on no-update or
        // any failure (errors are logged via Deckify.Common.Logger).
        Task<bool> ApplyPendingUpdateAsync(CancellationToken ct = default);
    }
}
