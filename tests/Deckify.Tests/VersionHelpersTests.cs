using System;
using Xunit;
using Deckify.Common;

namespace Deckify.Tests
{
    public class VersionHelpersTests
    {
        [Fact]
        public void IsNewerVersion_NewerMajor_ReturnsTrue()
        {
            Assert.True(VersionHelpers.IsNewerVersion("1.0.0", "2.0.0"));
        }

        [Fact]
        public void IsNewerVersion_NewerMinor_ReturnsTrue()
        {
            Assert.True(VersionHelpers.IsNewerVersion("1.0.0", "1.1.0"));
        }

        [Fact]
        public void IsNewerVersion_NewerPatch_ReturnsTrue()
        {
            Assert.True(VersionHelpers.IsNewerVersion("1.0.0", "1.0.1"));
        }

        [Fact]
        public void IsNewerVersion_OlderVersion_ReturnsFalse()
        {
            Assert.False(VersionHelpers.IsNewerVersion("2.0.0", "1.0.0"));
        }

        [Fact]
        public void IsNewerVersion_SameVersion_ReturnsFalse()
        {
            Assert.False(VersionHelpers.IsNewerVersion("1.0.0", "1.0.0"));
        }

        [Fact]
        public void IsNewerVersion_NullOrEmptyInput_ReturnsFalse()
        {
            Assert.False(VersionHelpers.IsNewerVersion(null, "1.0.0"));
            Assert.False(VersionHelpers.IsNewerVersion("1.0.0", ""));
        }

        [Fact]
        public void IsNewerVersion_InvalidFormat_ReturnsFalse()
        {
             // Should fail gracefully rather than throw exception
            Assert.False(VersionHelpers.IsNewerVersion("invalid", "1.0.0"));
        }
    }
}
