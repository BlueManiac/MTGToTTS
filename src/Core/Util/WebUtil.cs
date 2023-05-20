using System.Net;

namespace Core.Util;

internal static class WebUtil
{
    public static async Task<string> ExpandUrl(string url)
    {
        using var handler = new HttpClientHandler { AllowAutoRedirect = false };
        using var httpClient = new HttpClient(handler);
        using var request = new HttpRequestMessage(HttpMethod.Head, url);
        using var response = await httpClient.SendAsync(request);

        if (IsRedirected(response))
        {
            var uri = request.RequestUri;

            return $"{uri.Scheme}://{uri.Host}{response.Headers.Location}";
        }

        return url;

        static bool IsRedirected(HttpResponseMessage response)
        {
            var code = response.StatusCode;

            return code == HttpStatusCode.MovedPermanently || code == HttpStatusCode.Found;
        }
    }
}