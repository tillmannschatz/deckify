using System.Threading;
using System.Threading.Tasks;

namespace Deckify.Modules.AboutManager
{
    public interface IAutoUpdateChecker
    {
        Task RunStartupCheckAsync(CancellationToken ct = default);
    }
}
