using System.Text.RegularExpressions;

namespace AIProductify.API.Helper
{
    public static class ExtractProductId
    {

        public static string ExtractProduct(string productUrl)
        {
            var match = Regex.Match(productUrl, @"-p-(\d+)");
            return match.Success ? match.Groups[1].Value : null;
        }
    }
}
