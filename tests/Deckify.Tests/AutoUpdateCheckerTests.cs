using System;
using System.Threading;
using System.Threading.Tasks;
using Deckify.Modules.AboutManager;
using Moq;
using Xunit;

namespace Deckify.Tests
{
    public class AutoUpdateCheckerTests
    {
        private static (Mock<IUpdateService> svc, Mock<IUpdateNotificationService> noti, FakeSettings settings, AutoUpdateChecker checker) Build(
            bool autoCheck = true,
            DateTime? lastCheckUtc = null)
        {
            var svc = new Mock<IUpdateService>(MockBehavior.Strict);
            var noti = new Mock<IUpdateNotificationService>(MockBehavior.Strict);
            var settings = new FakeSettings
            {
                AutoCheckEnabled = autoCheck,
                LastCheckUtc = lastCheckUtc ?? default
            };
            return (svc, noti, settings, new AutoUpdateChecker(svc.Object, noti.Object, settings));
        }

        [Fact]
        public async Task AutoCheckDisabled_DoesNothing()
        {
            var (svc, noti, _, checker) = Build(autoCheck: false);

            await checker.RunStartupCheckAsync();

            svc.VerifyNoOtherCalls();
            noti.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task LastCheckRecent_DoesNothing()
        {
            var (svc, noti, _, checker) = Build(lastCheckUtc: DateTime.UtcNow.AddHours(-1));

            await checker.RunStartupCheckAsync();

            // Throttle in effect — no API call, no toast.
            svc.VerifyNoOtherCalls();
            noti.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task NoUpdate_DoesNotNotify()
        {
            var (svc, noti, settings, checker) = Build();
            svc.Setup(s => s.CheckForUpdatesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new UpdateInfo { IsUpdateAvailable = false });

            await checker.RunStartupCheckAsync();

            svc.Verify(s => s.CheckForUpdatesAsync(It.IsAny<CancellationToken>()), Times.Once);
            noti.VerifyNoOtherCalls();
            Assert.True(settings.SaveCalled);
            Assert.NotEqual(default, settings.LastCheckUtc);
        }

        [Fact]
        public async Task UpdateAvailable_ShowsToast_NoDownload()
        {
            var (svc, noti, _, checker) = Build();
            var info = new UpdateInfo { IsUpdateAvailable = true, LatestVersion = "0.1.40", ReleaseNotes = "notes" };
            svc.Setup(s => s.CheckForUpdatesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(info);
            noti.Setup(n => n.ShowUpdateAvailable("0.1.40", "notes"));

            await checker.RunStartupCheckAsync();

            // ClickOnce-native flow: just notify. No pre-download by the checker —
            // the actual ApplicationDeployment.Update() runs only when the user
            // clicks Install in the toast.
            svc.Verify(s => s.CheckForUpdatesAsync(It.IsAny<CancellationToken>()), Times.Once);
            noti.Verify(n => n.ShowUpdateAvailable("0.1.40", "notes"), Times.Once);
        }

        [Fact]
        public async Task ExceptionInService_IsSwallowed()
        {
            var (svc, noti, _, checker) = Build();
            svc.Setup(s => s.CheckForUpdatesAsync(It.IsAny<CancellationToken>()))
                .Throws(new InvalidOperationException("boom"));

            // Should not propagate — startup must never fail because of update check.
            await checker.RunStartupCheckAsync();

            noti.VerifyNoOtherCalls();
        }

        private sealed class FakeSettings : IAutoUpdateSettings
        {
            public bool AutoCheckEnabled { get; set; }
            public DateTime LastCheckUtc { get; set; }
            public bool SaveCalled { get; private set; }
            public void Save() => SaveCalled = true;
        }
    }
}
