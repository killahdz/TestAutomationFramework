using System;

namespace Core.Library
{
    public static class Urls
    {
        public static string BaseUrl => ConfigManager.WebsiteRoot;

        /// <summary>
        ///     Gets the absolute URl for a path
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string GetUrl(string path)
        {
            var urlString = $"{ConfigManager.WebsiteRoot}/{path}";

            Uri uriResult;
            var isValid = Uri.TryCreate(urlString, UriKind.Absolute, out uriResult)
                          && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);

            if (!isValid)
                throw new ApplicationException($"Invalid Url: {urlString}");

            return urlString;
        }

        public static bool IsAValidUrl(string urlToTest)
        {
            var result = false;
            Uri uriResult;
            result = Uri.TryCreate(urlToTest, UriKind.Absolute, out uriResult) &&
                     (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
            return result;
        }
    }
}