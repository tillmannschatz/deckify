using System;
using Xunit;
using Deckify.Common;
using System.Text;
using Newtonsoft.Json;

namespace Deckify.Tests
{
    public class ReferenceHelpersTests
    {
        [Fact]
        public void GetPPTURL_ValidUrl_ReturnsCleanUrl()
        {
            // Arrange
            string input = "https://example.sharepoint.com/sites/test/doc.pptx?web=1&nav=xyz";
            string expected = "https://example.sharepoint.com/sites/test/doc.pptx";

            // Act
            string result = ReferenceHelpers.GetPPTURL(input);

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void GetPPTURL_NoExtension_ReturnsEmpty()
        {
            // Arrange
            string input = "https://example.sharepoint.com/sites/test/doc.docx?web=1";
            string expected = "";

            // Act
            string result = ReferenceHelpers.GetPPTURL(input);

            // Assert
            Assert.Equal(expected, result);
        }
        
        [Fact]
        public void GetPPTURL_NoQueryParams_ReturnsEmpty()
        {
            // Current implementation requires '.pptx?' to match
            string input = "https://example.sharepoint.com/sites/test/doc.pptx";
            string expected = ""; // Likely bug/feature of current implementation
            string result = ReferenceHelpers.GetPPTURL(input);
            Assert.Equal(expected, result);
        }

        [Fact]
        public void GetPPTSlideID_ValidNavParam_ReturnsSlideId()
        {
            // Arrange
            var payload = new { sId = 256, other = "data" };
            string json = JsonConvert.SerializeObject(payload);
            byte[] bytes = Encoding.UTF8.GetBytes(json);
            string base64 = Convert.ToBase64String(bytes);
            
            string input = $"https://example.com/doc.pptx?nav={base64}&other=param";
            
            // Act
            int result = ReferenceHelpers.GetPPTSlideID(input);

            // Assert
            Assert.Equal(256, result);
        }

        [Fact]
        public void GetPPTSlideID_ValidNavParam_WithPaddingMissing_ReturnsSlideId()
        {
             // Arrange
            var payload = new { sId = 1024 };
            string json = JsonConvert.SerializeObject(payload);
            byte[] bytes = Encoding.UTF8.GetBytes(json);
            string base64 = Convert.ToBase64String(bytes).TrimEnd('='); // Force missing padding
            
            string input = $"nav={base64}";

            // Act
            int result = ReferenceHelpers.GetPPTSlideID(input);

            // Assert
            Assert.Equal(1024, result);
        }

        [Fact]
        public void GetPPTSlideID_InvalidBase64_ThrowsException()
        {
            string input = "nav=INVALID_BASE64_&&&&";
            Assert.ThrowsAny<Exception>(() => ReferenceHelpers.GetPPTSlideID(input));
        }

        [Fact]
        public void GetPPTSlideID_InvalidJson_ThrowsException()
        {
            string invalidJsonBase64 = Convert.ToBase64String(Encoding.UTF8.GetBytes("Not Json"));
            string input = $"nav={invalidJsonBase64}";
            Assert.ThrowsAny<Exception>(() => ReferenceHelpers.GetPPTSlideID(input));
        }
    }
}
