using DiGi.WebAPI.Classes;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace DiGi.GIS.WebAPI.UI
{
    public static partial class Query
    {
        /// <summary>
        /// Asynchronously reads the number of building data rows one county part holds from the GIS Web API.
        /// <para>The cheapest question the table answers (<c>GET gis/BuildingData/countbycountyid</c>, an exact count on one partition - 0.15 s on the largest parts), asked before a single page is fetched so that an area above the solve ceiling is refused in seconds rather than after minutes of paging. The upstream answers 404 for a county part that has no building data partition at all; that is a part holding nothing, not a failure, so it counts as zero. <see langword="null"/> is every other way of not getting a number: a failure status, an unreachable service, a body that is not an integer.</para>
        /// </summary>
        /// <param name="httpClient">The HTTP client used for the request. This value can be null.</param>
        /// <param name="countyId">The county part identifier whose rows are counted.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by the caller to cancel the asynchronous operation.</param>
        /// <returns>The row count, 0 when the part has no partition, or <see langword="null"/> when the upstream answered nothing usable.</returns>
        public static async Task<long?> BuildingDataCountAsync(this HttpClient? httpClient, int countyId, CancellationToken cancellationToken = default)
        {
            if (httpClient is null || countyId <= 0)
            {
                return null;
            }

            UrlBuilder urlBuilder = new($"{Constants.Default.GISWebAPIUri}/gis/BuildingData/countbycountyid");
            urlBuilder = urlBuilder.AddParameter("countyid", countyId);

            try
            {
                using HttpResponseMessage httpResponseMessage = await httpClient.GetAsync(urlBuilder.ToString(), cancellationToken);
                if (httpResponseMessage.StatusCode == HttpStatusCode.NotFound)
                {
                    return 0;
                }

                if (!httpResponseMessage.IsSuccessStatusCode)
                {
                    return null;
                }

                string text = await httpResponseMessage.Content.ReadAsStringAsync(cancellationToken);
                return long.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture, out long count) && count >= 0 ? count : null;
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                throw;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Asynchronously reads the number of building data rows a set of county parts holds together, one part at a time.
        /// <para>The pre-flight of a solve: the parts are counted sequentially (the deployed host is queried one request at a time, never fanned out) and summed. A part whose count cannot be read makes the sum unknown - <see langword="null"/> - rather than an understatement, so a caller comparing it with a ceiling does not let an area through on a partial figure; the fetch itself still stops at the ceiling.</para>
        /// </summary>
        /// <param name="httpClient">The HTTP client used for the request. This value can be null.</param>
        /// <param name="countyIds">The county part identifiers whose rows are counted; a part listed twice is counted once. This value can be null.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by the caller to cancel the asynchronous operation.</param>
        /// <returns>The row count over every part, 0 for no parts, or <see langword="null"/> when any part could not be counted.</returns>
        public static async Task<long?> BuildingDataCountAsync(this HttpClient? httpClient, IEnumerable<int>? countyIds, CancellationToken cancellationToken = default)
        {
            if (httpClient is null || countyIds is null)
            {
                return null;
            }

            long result = 0;
            HashSet<int> countyIds_Counted = [];
            foreach (int countyId in countyIds)
            {
                if (!countyIds_Counted.Add(countyId))
                {
                    continue;
                }

                cancellationToken.ThrowIfCancellationRequested();

                long? count = await httpClient.BuildingDataCountAsync(countyId, cancellationToken);
                if (count is null)
                {
                    return null;
                }

                result += count.Value;
            }

            return result;
        }
    }
}
