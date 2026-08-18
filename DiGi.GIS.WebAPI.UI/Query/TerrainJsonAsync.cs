using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace DiGi.GIS.WebAPI.UI
{
    public static partial class Query
    {
        /// <summary>
        /// Asynchronously reads a GIS Web API terrain service response body.
        /// <para>Terrain is optional by contract, so every way of not getting a body collapses into <see langword="null"/>: the area holds no stored elevation points (404), the terrain store cannot be queried (500 - the elevation table does not exist in every environment yet), the service is unreachable, or the request timed out. A caller that only wants terrain when it exists cannot be broken by any of them, and no scene loading terrain alongside other objects loses those objects because of it. The price is that "no terrain here" and "terrain service down" are indistinguishable to the caller; the terrain service logs which one it was.</para>
        /// <para>The wait is bounded by <see cref="Constants.Default.TerrainRequestTimeout"/> rather than by the 100 second <see cref="HttpClient"/> default, so a stalled terrain query cannot hold a page request open.</para>
        /// </summary>
        /// <param name="httpClient">The HTTP client used for the request. This value can be null.</param>
        /// <param name="requestUri">The terrain service URL to read, as built by <see cref="TerrainRequestUri(Geometry.Planar.Classes.Circle2D, double?)"/>. This value can be null.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by the caller to cancel the asynchronous operation.</param>
        /// <returns>The response body, or <see langword="null"/> when there is none.</returns>
        public static async Task<string?> TerrainJsonAsync(this HttpClient? httpClient, string? requestUri, CancellationToken cancellationToken = default)
        {
            if (httpClient is null || string.IsNullOrWhiteSpace(requestUri))
            {
                return null;
            }

            try
            {
                using CancellationTokenSource cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
                cancellationTokenSource.CancelAfter(TimeSpan.FromSeconds(Constants.Default.TerrainRequestTimeout));

                HttpResponseMessage httpResponseMessage = await httpClient.GetAsync(requestUri, cancellationTokenSource.Token);
                if (!httpResponseMessage.IsSuccessStatusCode)
                {
                    return null;
                }

                string json = await httpResponseMessage.Content.ReadAsStringAsync(cancellationTokenSource.Token);
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
