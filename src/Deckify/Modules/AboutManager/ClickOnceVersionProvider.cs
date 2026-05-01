using System.Deployment.Application;
using System.Reflection;

namespace Deckify.Modules.AboutManager
{
    public static class ClickOnceVersionProvider
    {
        public static string GetInstalledVersion()
        {
            try
            {
                if (ApplicationDeployment.IsNetworkDeployed)
                {
                    return ApplicationDeployment.CurrentDeployment.CurrentVersion.ToString();
                }
            }
            catch
            {
            }

            return Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }
    }
}
