using DiGi.WebAPI.Classes;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace DiGi.GIS.WebAPI.UI
{
    public static partial class Query
    {
        /// <summary>
        /// Fetches the boundary polygon of the selected administrative area, for clipping the county-wide building data down to the area when it is below county level.
        /// <para>A municipality or subdivision is a subset of its county, so the county's buildings must be kept only where they fall inside the area's boundary. The polygon is read from <c>itembyid</c> and sits in the same planar coordinate system as the building rows' <c>internal_point_x</c>/<c>internal_point_y</c>, so no transformation is needed before the point-in-polygon test.</para>
        /// </summary>
        /// <param name="httpClient">The HTTP client used for the request. This value can be null.</param>
        /// <param name="id">The unique identifier of the selected administrative area.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by the caller to cancel the asynchronous operation.</param>
        /// <returns>The area's boundary polygon, or <see langword="null"/> when the upstream is unreachable or the area carries no geometry.</returns>
        public static async Task<DiGi.Geometry.Planar.Classes.PolygonalFace2D?> AreaPolygonAsync(this HttpClient? httpClient, int id, CancellationToken cancellationToken = default)
        {
            if (httpClient is null || id <= 0)
            {
                return null;
            }

            UrlBuilder urlBuilder = new($"{Constants.Default.GISWebAPIUri}/gis/administrativeareal2d/itembyid");
            urlBuilder = urlBuilder.AddParameter("id", id);

            List<DiGi.GIS.Classes.AdministrativeAreal2D>? items = await httpClient.ItemsAsync<DiGi.GIS.Classes.AdministrativeAreal2D>(urlBuilder.ToString(), cancellationToken);
            DiGi.GIS.Classes.AdministrativeAreal2D? item = items?.FirstOrDefault();

            return item?.PolygonalFace2D;
        }
    }
}
