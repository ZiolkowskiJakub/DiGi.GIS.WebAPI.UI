using DiGi.WebAPI.Classes;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace DiGi.GIS.WebAPI.UI
{
    public static partial class Query
    {
        /// <summary>
        /// Asynchronously reads the GIS Web API reference record of a building while preserving the status the service answered with.
        /// <para>The status-preserving counterpart of <see cref="Building2DReferenceAsync(HttpClient, string, int?, CancellationToken)"/>: where that read collapses every failure into <see langword="null"/> so a panel assembled from several independent requests survives one of them coming back empty, this one reports the status, so a caller whose outcome must name the cause can tell a refusal (or a fault) from an absence - the Orto Data direct-mode read is exactly such a caller (issue #48). See <see cref="Classes.WebAPIResponse"/> for why absence and refusal must not be the same answer.</para>
        /// <para><see langword="null"/> is still returned for the failures that carry no status at all: no client, no URL, the service unreachable, or the caller cancelling. Those are the cases where nothing was answered, as opposed to something being refused.</para>
        /// <para>Building data is partitioned per county, so <paramref name="countyId"/> addresses the data set the reference belongs to. Without it the GIS Web API resolves the reference to the lowest county part holding it, which is only unambiguous while no building is filed under two parts of the same county - pass it whenever it is known.</para>
        /// <para>The request is made anonymously: <c>gis/building2D/building2Dreferencebyreference</c> is not session-gated, matching <see cref="Building2DReferenceAsync(HttpClient, string, int?, CancellationToken)"/>.</para>
        /// </summary>
        /// <param name="httpClient">The HTTP client used for the request. This value can be null.</param>
        /// <param name="reference">The reference of the building. This value can be null.</param>
        /// <param name="countyId">The identifier of the county part the building is filed under. This value can be null.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by the caller to cancel the asynchronous operation.</param>
        /// <returns>The status and body the GIS Web API answered with, or <see langword="null"/> when it answered nothing at all.</returns>
        public static async Task<Classes.WebAPIResponse?> Building2DReferenceResponseAsync(this HttpClient? httpClient, string? reference, int? countyId, CancellationToken cancellationToken = default)
        {
            if (httpClient is null || string.IsNullOrWhiteSpace(reference))
            {
                return null;
            }

            UrlBuilder urlBuilder = new($"{Constants.Default.GISWebAPIUri}/gis/building2D/building2Dreferencebyreference");
            urlBuilder = urlBuilder.AddParameter("reference", reference);
            if (countyId.HasValue)
            {
                urlBuilder = urlBuilder.AddParameter("countyid", countyId.Value);
            }

            return await httpClient.ResponseAsync(HttpMethod.Get, urlBuilder.ToString(), null, cancellationToken);
        }
    }
}
