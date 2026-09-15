using DiGi.GIS.PostgreSQL.Classes;
using DiGi.WebAPI.Classes;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace DiGi.GIS.WebAPI.UI
{
    public static partial class Query
    {
        /// <summary>
        /// Asynchronously reads the bounding-box centres of every building of an administrative area from the GIS Web API, keyed by reference and county part.
        /// <para>The request retries once with a doubled command timeout when the first attempt answers nothing: the known cause is a command timeout on a cold partition (DiGi.GIS.WebAPI issue #27 precedent), and the retry succeeds because the partition is warm by then. A second failure is not retried - it is a genuine defect, not a cold start.</para>
        /// <para>An area the upstream resolves to no subdivision answers an empty list, not <see langword="null"/>; <see langword="null"/> means the upstream failed or could not be reached, for the reasons given on <see cref="JsonAsync(HttpClient, string, CancellationToken)"/>.</para>
        /// </summary>
        /// <param name="httpClient">The HTTP client used for the request. This value can be null.</param>
        /// <param name="administrativeAreal2DId">The unique identifier of the administrative area.</param>
        /// <param name="commandTimeout">The upstream command timeout in seconds for the first attempt; the retry doubles it. Defaults to 30.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by the caller to cancel the asynchronous operation.</param>
        /// <returns>The centroids of the area, an empty list when the area holds none, or <see langword="null"/> when the upstream failed even after the retry.</returns>
        public static async Task<List<Building2DCentroid>?> Building2DCentroidsAsync(this HttpClient? httpClient, int administrativeAreal2DId, int commandTimeout = 30, CancellationToken cancellationToken = default)
        {
            if (httpClient is null || administrativeAreal2DId <= 0)
            {
                return null;
            }

            List<Building2DCentroid>? building2DCentroids = await httpClient.ItemsAsync<Building2DCentroid>(RequestUri(administrativeAreal2DId, commandTimeout), cancellationToken);
            if (building2DCentroids is null && !cancellationToken.IsCancellationRequested)
            {
                building2DCentroids = await httpClient.ItemsAsync<Building2DCentroid>(RequestUri(administrativeAreal2DId, commandTimeout * 2), cancellationToken);
            }

            return building2DCentroids;

            static string RequestUri(int administrativeAreal2DId, int commandTimeout)
            {
                UrlBuilder urlBuilder = new($"{Constants.Default.GISWebAPIUri}/gis/building2D/point2dsbyadministrativeareal2Did");
                urlBuilder = urlBuilder.AddParameter("administrativeareal2Did", administrativeAreal2DId);
                urlBuilder = urlBuilder.AddParameter("commandtimeout", commandTimeout);
                return urlBuilder.ToString();
            }
        }
    }
}
