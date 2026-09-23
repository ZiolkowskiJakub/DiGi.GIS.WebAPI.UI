using DiGi.GIS.PostgreSQL.Classes;
using DiGi.GIS.WebAPI.UI.ViewModels;
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
        /// Reads the bounding-box centres of every building of an administrative area from the GIS Web API, as the view models the Typology area view draws its dots from.
        /// <para>The compact endpoint <see cref="Constants.Default.Building2DCentroidsUri"/> is asked first (DiGi.GIS.WebAPI#40) and read with <see cref="Convert.ToDiGi_Building2DCentroidViewModels(string?)"/>. It answers the same buildings as the full one without a <c>_type</c> and four property names per building: 15.9 MB and 2.1-2.6 s for county 1465 through the full endpoint (DiGi.GIS.WebAPI.UI#29).</para>
        /// <para>When the compact endpoint answers nothing, the full <c>point2dsbyadministrativeareal2Did</c> is read instead, retried once with a doubled command timeout for a cold partition, and mapped to the same view models. A centroid without a county part or a reference is skipped there, as the compact endpoint skips it. That fallback is temporary code, <c>TODO [CompactCentroids]</c>, and also covers a compact request that timed out on a cold partition.</para>
        /// </summary>
        /// <param name="httpClient">The HTTP client used for the request. This value can be null.</param>
        /// <param name="administrativeAreal2DId">The unique identifier of the administrative area.</param>
        /// <param name="commandTimeout">The upstream command timeout in seconds for the first attempt. Defaults to 30.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by the caller to cancel the asynchronous operation.</param>
        /// <returns>The centroids (empty when the area holds none), or <see langword="null"/> when the upstream answered nothing on either endpoint.</returns>
        public static async Task<List<Building2DCentroidViewModel>?> Building2DCentroidsAsync(this HttpClient? httpClient, int administrativeAreal2DId, int commandTimeout = 30, CancellationToken cancellationToken = default)
        {
            if (httpClient is null || administrativeAreal2DId <= 0)
            {
                return null;
            }

            UrlBuilder urlBuilder_Compact = new(Constants.Default.Building2DCentroidsUri);
            urlBuilder_Compact = urlBuilder_Compact.AddParameter("administrativeareal2Did", administrativeAreal2DId);
            urlBuilder_Compact = urlBuilder_Compact.AddParameter("commandtimeout", commandTimeout);

            List<Building2DCentroidViewModel>? result = Convert.ToDiGi_Building2DCentroidViewModels(await httpClient.JsonAsync(urlBuilder_Compact.ToString(), cancellationToken));
            if (result is not null || cancellationToken.IsCancellationRequested)
            {
                return result;
            }

            // TODO [CompactCentroids]: the full endpoint, for a DiGi.GIS.WebAPI build that does not serve the compact one yet
            // (it answers 404). Remove once GET /information/controllers reports a DiGi.GIS.WebAPI build carrying
            // centroidsbyadministrativeareal2Did - with the retry then moved onto the compact request.
            List<Building2DCentroid>? building2DCentroids = await httpClient.ItemsAsync<Building2DCentroid>(RequestUri(administrativeAreal2DId, commandTimeout), cancellationToken);
            if (building2DCentroids is null && !cancellationToken.IsCancellationRequested)
            {
                building2DCentroids = await httpClient.ItemsAsync<Building2DCentroid>(RequestUri(administrativeAreal2DId, commandTimeout * 2), cancellationToken);
            }

            if (building2DCentroids is null)
            {
                return null;
            }

            result = new(building2DCentroids.Count);
            foreach (Building2DCentroid building2DCentroid in building2DCentroids)
            {
                // A row without a county part or a reference cannot be joined by the view, so it is skipped rather than defaulted to a key no building has.
                if (!building2DCentroid.CountyId.HasValue || string.IsNullOrWhiteSpace(building2DCentroid.Reference))
                {
                    continue;
                }

                result.Add(new Building2DCentroidViewModel(building2DCentroid.Reference, building2DCentroid.CountyId.Value, building2DCentroid.X, building2DCentroid.Y));
            }

            return result;

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
