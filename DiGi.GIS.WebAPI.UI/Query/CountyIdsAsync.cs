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
        /// Asynchronously resolves which county polygon parts a plan position can be filed under.
        /// <para>Used where a building is known by its reference and a point but not by its county: the reference alone does not say which county partition holds it (the building details page, the Building Viewer opened by reference).</para>
        /// <para>Every part of the county the point falls in is returned, not just the part covering the point. A county whose territory is disconnected is stored as one row per part and the building is filed under one of them, which need not be the part its own coordinates fall in (ZiolkowskiJakub/DiGi.GIS.PostgreSQL#64).</para>
        /// </summary>
        /// <param name="httpClient">The HTTP client used for the requests. This value can be null.</param>
        /// <param name="x">The X coordinate, in PL-1992 (EPSG:2180) metres. This value can be null.</param>
        /// <param name="y">The Y coordinate, in PL-1992 (EPSG:2180) metres. This value can be null.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by the caller to cancel the asynchronous operation.</param>
        /// <returns>The identifiers of every polygon part of the county, in ascending order, or <see langword="null"/> when no county could be resolved.</returns>
        public static async Task<List<int>?> CountyIdsAsync(this HttpClient? httpClient, double? x, double? y, CancellationToken cancellationToken = default)
        {
            if (httpClient is null || !x.HasValue || !y.HasValue || !double.IsFinite(x.Value) || !double.IsFinite(y.Value))
            {
                return null;
            }

            // The type filter is what makes this a county lookup rather than a request for every administrative
            // area covering the point - the country, the voivodeship and the municipality cover it too, and the
            // first of those would otherwise be read as the answer. The integer token is used rather than the
            // member name because it binds against every deployed build of the GIS Web API regardless of enum
            // renames.
            UrlBuilder urlBuilder = new($"{Constants.Default.GISWebAPIUri}/gis/administrativeareal2D/itemsbypoint");
            urlBuilder = urlBuilder.AddParameter("x", x.Value);
            urlBuilder = urlBuilder.AddParameter("y", y.Value);
            urlBuilder = urlBuilder.AddParameter("administrativearealtype", (int)AdministrativeArealType.County);

            GIS.Classes.AdministrativeAreal2D? administrativeAreal2D = await httpClient.ItemAsync<GIS.Classes.AdministrativeAreal2D>(urlBuilder.ToString(), cancellationToken);

            string? code = administrativeAreal2D?.Code;
            if (string.IsNullOrWhiteSpace(code))
            {
                return null;
            }

            // idsbycode rather than idbycode: the latter collapses a code to its lowest polygon part, and for
            // 3020 that part holds no building at all. Every part is returned instead and the caller tries them
            // in turn. Ordered so the part tried first does not change with the query plan.
            urlBuilder = new($"{Constants.Default.GISWebAPIUri}/gis/administrativeareal2D/idsbycode");
            urlBuilder = urlBuilder.AddParameter("code", code);
            urlBuilder = urlBuilder.AddParameter("administrativearealtype", (int)AdministrativeArealType.County);

            // A bare JSON array of integers rather than a serializable object, so it is read as text and
            // deserialized here - ItemsAsync only handles ISerializableObject payloads.
            string? json = await httpClient.JsonAsync(urlBuilder.ToString(), cancellationToken);
            if (string.IsNullOrWhiteSpace(json))
            {
                return null;
            }

            List<int>? countyIds;
            try
            {
                countyIds = JsonSerializer.Deserialize<List<int>>(json);
            }
            catch (JsonException)
            {
                return null;
            }

            if (countyIds is null || countyIds.Count == 0)
            {
                return null;
            }

            countyIds.Sort();

            return countyIds;
        }
    }
}
