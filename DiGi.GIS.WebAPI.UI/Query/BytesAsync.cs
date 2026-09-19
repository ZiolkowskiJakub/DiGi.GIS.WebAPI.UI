using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace DiGi.GIS.WebAPI.UI
{
    public static partial class Query
    {
        /// <summary>
        /// Asynchronously reads the body of a GIS Web API response as bytes, for the payloads that are not JSON.
        /// <para>Every way of not getting a body collapses into <see langword="null"/>: the service answered with a failure status, answered with nothing, was unreachable, or the request was cancelled - for the reasons given on <see cref="JsonAsync(HttpClient, string, CancellationToken)"/>. The Orto Data image relay maps that absence to a 404 of its own, which the page turns into a hidden card, so a missing photo degrades the grid rather than failing the page.</para>
        /// <para>Sends no session token: the endpoint this serves (the orthophoto image read) is anonymous upstream. A bearer-carrying variant belongs on <see cref="ResponseAsync(HttpClient, HttpMethod, string, string, CancellationToken)"/> if one is ever needed.</para>
        /// </summary>
        /// <param name="httpClient">The HTTP client used for the request. This value can be null.</param>
        /// <param name="requestUri">The GIS Web API URL to read. This value can be null.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by the caller to cancel the asynchronous operation.</param>
        /// <returns>The response body, or <see langword="null"/> when there is none.</returns>
        public static async Task<byte[]?> BytesAsync(this HttpClient? httpClient, string? requestUri, CancellationToken cancellationToken = default)
        {
            if (httpClient is null || string.IsNullOrWhiteSpace(requestUri))
            {
                return null;
            }

            try
            {
                using HttpResponseMessage httpResponseMessage = await httpClient.GetAsync(requestUri, cancellationToken);
                if (!httpResponseMessage.IsSuccessStatusCode)
                {
                    return null;
                }

                return await httpResponseMessage.Content.ReadAsByteArrayAsync(cancellationToken);
            }
            catch
            {
                return null;
            }
        }
    }
}
