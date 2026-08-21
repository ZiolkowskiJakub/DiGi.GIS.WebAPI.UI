using DiGi.GIS.PostgreSQL.Classes;
using DiGi.WebAPI.Classes;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace DiGi.GIS.WebAPI.UI
{
    public static partial class Query
    {
        /// <summary>
        /// Asynchronously reads the GIS Web API reference record of a building.
        /// <para>Several panels of the building details page each need this same record for the identifiers and the administrative context it carries, so the lookup lives here rather than being spelled out again in every one of them.</para>
        /// <para>Building data is partitioned per county, so <paramref name="countyId"/> addresses the data set the reference belongs to. Without it the GIS Web API resolves the reference to the lowest county part holding it, which is only unambiguous while no building is filed under two parts of the same county - pass it whenever it is known.</para>
        /// </summary>
        /// <param name="httpClient">The HTTP client used for the request. This value can be null.</param>
        /// <param name="reference">The reference of the building. This value can be null.</param>
        /// <param name="countyId">The identifier of the county part the building is filed under. This value can be null.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by the caller to cancel the asynchronous operation.</param>
        /// <returns>The reference record, or <see langword="null"/> when there is none.</returns>
        public static async Task<Building2DReference?> Building2DReferenceAsync(this HttpClient? httpClient, string? reference, int? countyId, CancellationToken cancellationToken = default)
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

            return await httpClient.ItemAsync<Building2DReference>(urlBuilder.ToString(), cancellationToken);
        }
    }
}
