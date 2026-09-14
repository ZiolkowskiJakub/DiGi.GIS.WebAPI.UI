using DiGi.GIS.PostgreSQL.Classes;
using DiGi.GIS.PostgreSQL.Enums;
using DiGi.WebAPI.Classes;
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
        /// Resolves an administrative area code and type into the list of county part identifiers that scope a building data request.
        /// <para>Shared by <c>GET /typology/countyids</c> and <c>POST /typology/buildings</c> so both actions resolve parts through one code path. A county code maps to one identifier per polygon part (18 codes have several — see <c>Coding - GIS Administrative Data.md</c>), so the resolution goes through <c>idsbycode</c> rather than the single identifier the modal row carries.</para>
        /// <para><see langword="null"/> is the answer for every way of not getting parts: the upstream is unreachable, the code is blank, or the type names no parts. The caller maps <see langword="null"/> to 503 and an empty list to 404.</para>
        /// </summary>
        /// <param name="httpClient">The HTTP client used for the request. This value can be null.</param>
        /// <param name="code">The administrative code of the selected area.</param>
        /// <param name="administrativeArealType">The type of the selected area.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by the caller to cancel the asynchronous operation.</param>
        /// <returns>The county part identifiers, an empty list for a country, or <see langword="null"/> when the upstream answers nothing.</returns>
        public static async Task<List<int>?> CountyPartsAsync(this HttpClient? httpClient, string code, AdministrativeArealType administrativeArealType, CancellationToken cancellationToken = default)
        {
            // A country carries no parts whatever its code, so this guard precedes the blank-code one: a country is a valid, in-scope answer (404 - no buildings), not an upstream absence (503).
            if (administrativeArealType == AdministrativeArealType.Country)
            {
                return [];
            }

            if (string.IsNullOrWhiteSpace(code))
            {
                return null;
            }

            if (httpClient is null)
            {
                return null;
            }

            if (administrativeArealType == AdministrativeArealType.Voivodeship)
            {
                UrlBuilder urlBuilder = new($"{Constants.Default.GISWebAPIUri}/gis/administrativeareal2D/administrativeareal2Dreferencesbyadministrativearealtype");
                urlBuilder = urlBuilder.AddParameter("administrativearealtype", (int)AdministrativeArealType.County);

                List<DiGi.GIS.PostgreSQL.Classes.AdministrativeAreal2DReference>? references = await httpClient.ItemsAsync<DiGi.GIS.PostgreSQL.Classes.AdministrativeAreal2DReference>(urlBuilder.ToString(), cancellationToken);
                if (references is null)
                {
                    return null;
                }

                List<int> ids = [];
                foreach (DiGi.GIS.PostgreSQL.Classes.AdministrativeAreal2DReference reference in references)
                {
                    if (reference.Id >= 0 && reference.Code is not null && reference.Code.StartsWith(code, System.StringComparison.Ordinal))
                    {
                        if (!ids.Contains(reference.Id))
                        {
                            ids.Add(reference.Id);
                        }
                    }
                }

                ids.Sort();
                return ids;
            }

            string code_County = administrativeArealType == AdministrativeArealType.County ? code : (code.Length >= 4 ? code.Substring(0, 4) : code);

            UrlBuilder urlBuilder_Parts = new($"{Constants.Default.GISWebAPIUri}/gis/administrativeareal2D/idsbycode");
            urlBuilder_Parts = urlBuilder_Parts.AddParameter("code", code_County);
            urlBuilder_Parts = urlBuilder_Parts.AddParameter("administrativearealtype", (int)AdministrativeArealType.County);

            string? json = await httpClient.JsonAsync(urlBuilder_Parts.ToString(), cancellationToken);
            if (string.IsNullOrWhiteSpace(json))
            {
                return null;
            }

            List<int>? parts = null;
            try
            {
                parts = JsonSerializer.Deserialize<List<int>>(json);
            }
            catch
            {
                // The upstream answered with a body that is not a JSON array of integers;
                // treat it as absence rather than a parse error.
            }

            return parts is null || parts.Count == 0 ? null : parts;
        }
    }
}
