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
        /// <para>The compact endpoint <see cref="Constants.Default.Building2DCentroidsUri"/> is read with <see cref="Convert.ToDiGi_Building2DCentroidViewModels(string?)"/>. It answers the same buildings as the full one without a <c>_type</c> and four property names per building: 15.9 MB and 2.1-2.6 s for county 1465 through the full endpoint (DiGi.GIS.WebAPI.UI#29).</para>
        /// <para>A compact request that times out on a cold partition is retried once with a doubled command timeout.</para>
        /// </summary>
        /// <param name="httpClient">The HTTP client used for the request. This value can be null.</param>
        /// <param name="administrativeAreal2DId">The unique identifier of the administrative area.</param>
        /// <param name="commandTimeout">The upstream command timeout in seconds for the first attempt. Defaults to 30.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by the caller to cancel the asynchronous operation.</param>
        /// <returns>The centroids (empty when the area holds none), or <see langword="null"/> when the upstream answered nothing.</returns>
        public static async Task<List<Building2DCentroidViewModel>?> Building2DCentroidsAsync(this HttpClient? httpClient, int administrativeAreal2DId, int commandTimeout = 30, CancellationToken cancellationToken = default)
        {
            if (httpClient is null || administrativeAreal2DId <= 0)
            {
                return null;
            }

            List<Building2DCentroidViewModel>? result = Convert.ToDiGi_Building2DCentroidViewModels(await httpClient.JsonAsync(RequestUri(administrativeAreal2DId, commandTimeout), cancellationToken));
            if (result is not null || cancellationToken.IsCancellationRequested)
            {
                return result;
            }

            // A compact request that times out on a cold partition is retried once with a doubled command timeout.
            result = Convert.ToDiGi_Building2DCentroidViewModels(await httpClient.JsonAsync(RequestUri(administrativeAreal2DId, commandTimeout * 2), cancellationToken));
            return result;

            static string RequestUri(int administrativeAreal2DId, int commandTimeout)
            {
                UrlBuilder urlBuilder = new(Constants.Default.Building2DCentroidsUri);
                urlBuilder = urlBuilder.AddParameter("administrativeareal2Did", administrativeAreal2DId);
                urlBuilder = urlBuilder.AddParameter("commandtimeout", commandTimeout);
                return urlBuilder.ToString();
            }
        }
    }
}
