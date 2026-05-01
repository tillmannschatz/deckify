using System;
using Newtonsoft.Json.Linq;

namespace Deckify.Common
{
    public static class ReferenceHelpers
    {
        public static string GetPPTURL(string sharePointLink)
        {
            int indexOfPptx = sharePointLink.IndexOf(".pptx?");
            if (indexOfPptx != -1) { sharePointLink = sharePointLink.Substring(0, indexOfPptx + 5); } else { sharePointLink = ""; }
            return sharePointLink;
        }

        public static int GetPPTSlideID(string sharePointLink)
        {
            string encodedBase64String = sharePointLink;
            string decodedBase64String;

            int indexOfNav = encodedBase64String.IndexOf("nav=");
            if (indexOfNav != -1)
            {
                encodedBase64String = encodedBase64String.Substring(indexOfNav + "nav=".Length);
                int indexOfAmpersand = encodedBase64String.IndexOf('&');
                if (indexOfAmpersand != -1) { encodedBase64String = encodedBase64String.Substring(0, indexOfAmpersand); }
            }

            int padding = 4 - (encodedBase64String.Length % 4);
            if (padding > 0 && padding < 4) { encodedBase64String += new string('=', padding); }

            byte[] bytes = Convert.FromBase64String(encodedBase64String);
            decodedBase64String = System.Text.Encoding.UTF8.GetString(bytes);

            string jsonData = decodedBase64String;
            JObject jsonObject = JObject.Parse(jsonData);

            return jsonObject["sId"].Value<int>();
        }
    }
}
