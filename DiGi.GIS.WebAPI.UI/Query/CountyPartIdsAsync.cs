using DiGi.GIS.PostgreSQL.Enums;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace DiGi.GIS.WebAPI.UI
{
    public static partial class Query
    {
        /// <summary>
        /// Resolves an administrative area to the county part identifiers whose building data a Typology solve reads, answering the outcome as a status the actions can relay.
        /// <para>The resolution is <see cref="CountyPartsAsync(HttpClient?, string, AdministrativeArealType, CancellationToken)"/>; this member reads its answer into identifiers, so the solve (<c>POST /typology/buildings</c>) and its pre-flight count (<c>GET /typology/buildingcount</c>) resolve an area the same way.</para>
        /// <para>Outcomes: <see cref="StatusCodes.Status200OK"/> with at least one identifier; <see cref="StatusCodes.Status404NotFound"/> when the area names no parts, the upstream's empty answer included; <see cref="StatusCodes.Status503ServiceUnavailable"/> when the upstream refused, failed or answered nothing. A country resolves to no parts (404), because it is every part there is rather than a scope a solve reads.</para>
        /// </summary>
        /// <param name="httpClient">The HTTP client used for the request. This value can be null.</param>
        /// <param name="code">The code of the administrative area. This value can be null.</param>
        /// <param name="administrativeArealType">The type of the administrative area.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by the caller to cancel the asynchronous operation.</param>
        /// <returns>The status of the resolution and, for 200, the distinct county part identifiers in the order the upstream named them.</returns>
        public static async Task<(int StatusCode, List<int>? CountyIds)> CountyPartIdsAsync(this HttpClient? httpClient, string? code, AdministrativeArealType administrativeArealType, CancellationToken cancellationToken = default)
        {
            Classes.WebAPIResponse? webAPIResponse = await httpClient.CountyPartsAsync(code ?? "", administrativeArealType, cancellationToken);
            if (webAPIResponse is null || (webAPIResponse.StatusCode != StatusCodes.Status200OK && webAPIResponse.StatusCode != StatusCodes.Status404NotFound))
            {
                return (StatusCodes.Status503ServiceUnavailable, null);
            }

            string? json = webAPIResponse.Json;
            if (webAPIResponse.StatusCode == StatusCodes.Status404NotFound || string.IsNullOrWhiteSpace(json))
            {
                return (StatusCodes.Status404NotFound, null);
            }

            List<int>? countyIds = null;
            try
            {
                countyIds = JsonSerializer.Deserialize<List<int>>(json);
            }
            catch
            {
                // The upstream answered with a body that is not a JSON array of integers; treat it as no parts.
            }

            if (countyIds is null || countyIds.Count == 0)
            {
                return (StatusCodes.Status404NotFound, null);
            }

            List<int> result = [];
            HashSet<int> countyIds_Distinct = [];
            foreach (int countyId in countyIds)
            {
                if (countyIds_Distinct.Add(countyId))
                {
                    result.Add(countyId);
                }
            }

            return (StatusCodes.Status200OK, result);
        }
    }
}
