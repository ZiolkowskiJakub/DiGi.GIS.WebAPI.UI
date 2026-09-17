using DiGi.GIS.PostgreSQL.Classes;
using DiGi.GIS.PostgreSQL.Enums;
using DiGi.WebAPI.Classes;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace DiGi.GIS.WebAPI.UI
{
    public static partial class Query
    {
        /// <summary>
        /// Resolves an administrative area code and type into the county part identifiers that scope a building data request, as the status and body the upstream answered with.
        /// <para>Shared by <c>GET /typology/countyids</c> and <c>POST /typology/buildings</c> so both actions resolve parts through one code path. A county code maps to one identifier per polygon part (18 codes have several — see <c>Coding - GIS Administrative Data.md</c>), so the resolution goes through <c>idsbycode</c> rather than the single identifier the modal row carries.</para>
        /// <para><see langword="null"/> is the answer for the ways of not getting parts that carry no status: the upstream is unreachable, or the code is blank. The rest is reported as the status the service answered with — an empty parts answer (the upstream's 404) stays the 404 the relay turns into the page's 204, an answered failure passes through as the refusal the page names (issue #40).</para>
        /// </summary>
        /// <param name="httpClient">The HTTP client used for the request. This value can be null.</param>
        /// <param name="code">The administrative code of the selected area.</param>
        /// <param name="administrativeArealType">The type of the selected area.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by the caller to cancel the asynchronous operation.</param>
        /// <returns>The status and body the upstream answered with — a 200 carrying the county part identifiers (empty for a country), a 404 for an area that names no parts — or <see langword="null"/> when the upstream answered nothing at all.</returns>
        public static async Task<Classes.WebAPIResponse?> CountyPartsAsync(this HttpClient? httpClient, string code, AdministrativeArealType administrativeArealType, CancellationToken cancellationToken = default)
        {
            // A country carries no parts whatever its code, so this guard precedes the blank-code one: a country is a valid, in-scope answer (no parts), not an upstream absence.
            if (administrativeArealType == AdministrativeArealType.Country)
            {
                return new Classes.WebAPIResponse(StatusCodes.Status200OK, "[]");
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

                // Status-preserving rather than the collapsing ItemsAsync: the relay must name a refusal, so the
                // service's own failure status passes through instead of collapsing into the no-parts answer (issue #40).
                Classes.WebAPIResponse? references = await httpClient.ResponseAsync(HttpMethod.Get, urlBuilder.ToString(), null, cancellationToken);
                if (references is null)
                {
                    return null;
                }

                if (references.StatusCode != StatusCodes.Status200OK)
                {
                    return references;
                }

                List<AdministrativeAreal2DReference>? administrativeAreal2DReferences = Core.Convert.ToDiGi<AdministrativeAreal2DReference>(references.Json);
                if (administrativeAreal2DReferences is null)
                {
                    // A 200 that carries no decodable references is a broken contract - the relay answers the refusal.
                    return new Classes.WebAPIResponse(StatusCodes.Status502BadGateway, null);
                }

                List<int> ids = [];
                foreach (AdministrativeAreal2DReference reference in administrativeAreal2DReferences)
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

                if (ids.Count == 0)
                {
                    // The voivodeship names no county: the upstream's empty answer (404), which the relay turns into the page's 204.
                    return new Classes.WebAPIResponse(StatusCodes.Status404NotFound, null);
                }

                return new Classes.WebAPIResponse(StatusCodes.Status200OK, JsonSerializer.Serialize(ids));
            }

            string code_County = administrativeArealType == AdministrativeArealType.County ? code : (code.Length >= 4 ? code.Substring(0, 4) : code);

            UrlBuilder urlBuilder_Parts = new($"{Constants.Default.GISWebAPIUri}/gis/administrativeareal2D/idsbycode");
            urlBuilder_Parts = urlBuilder_Parts.AddParameter("code", code_County);
            urlBuilder_Parts = urlBuilder_Parts.AddParameter("administrativearealtype", (int)AdministrativeArealType.County);

            Classes.WebAPIResponse? parts = await httpClient.ResponseAsync(HttpMethod.Get, urlBuilder_Parts.ToString(), null, cancellationToken);
            if (parts is null)
            {
                return null;
            }

            if (parts.StatusCode != StatusCodes.Status200OK)
            {
                return parts;
            }

            string? partsJson = parts.Json;
            if (string.IsNullOrWhiteSpace(partsJson))
            {
                // The upstream's empty answer is the 404 - the relay turns it into the page's 204 "no parts".
                return new Classes.WebAPIResponse(StatusCodes.Status404NotFound, null);
            }

            List<int>? ids_Parts = null;
            try
            {
                ids_Parts = JsonSerializer.Deserialize<List<int>>(partsJson);
            }
            catch
            {
                // The upstream answered with a body that is not a JSON array of integers;
                // treat it as absence rather than a parse error.
            }

            if (ids_Parts is null || ids_Parts.Count == 0)
            {
                // The upstream's empty answer is the 404 - the relay turns it into the page's 204 "no parts".
                return new Classes.WebAPIResponse(StatusCodes.Status404NotFound, null);
            }

            return parts;
        }
    }
}
