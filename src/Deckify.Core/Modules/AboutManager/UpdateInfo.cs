using System;

namespace Deckify.Modules.AboutManager
{
    public class UpdateInfo
    {
        public bool IsUpdateAvailable { get; set; }
        public bool IsNetworkDeployed { get; set; }
        public string LatestVersion { get; set; }
        public string ReleaseNotes { get; set; }
        public DateTime ReleaseDate { get; set; }
    }
}
