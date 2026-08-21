using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace DiGi.GIS.WebAPI.UI
{
    public static partial class Query
    {
        /// <summary>
        /// Asynchronously reads the body of a GIS Web API response.
        /// <para>Every way of not getting a body collapses into <see langword="null"/>: the service answered with a failure status, answered with nothing, was unreachable, or the request was cancelled. This application renders a page out of several independent requests, so one of them coming back empty has to leave the rest of the page standing rather than fail it, and what to do about the absence (an empty panel, a hidden section, a not found page) is left to the caller.</para>
        /// <para>The price is that "there is nothing here" and "the service is down" are indistinguishable to the caller; the GIS Web API logs which one it was.</para>
        /// </summary>
        /// <param name="httpClient">The HTTP client used for the request. This value can be null.</param>
        /// <param name="requestUri">The GIS Web API URL to read. This value can be null.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by the caller to cancel the asynchronous operation.</param>
        /// <returns>The response body, or <see langword="null"/> when there is none.</returns>
        public static async Task<string?> JsonAsync(this HttpClient? httpClient, string? requestUri, CancellationToken cancellationToken = default)
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

                string json = await httpResponseMessage.Content.ReadAsStringAsync(cancellationToken);
                if (string.IsNullOrWhiteSpace(json))
                {
                    return null;
                }

                return json;
            }
            catch
            {
                return null;
            }
        }
    }
}
