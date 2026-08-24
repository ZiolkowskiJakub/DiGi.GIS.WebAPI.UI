using DiGi.Geometry.Planar.Classes;
using DiGi.WebAPI.Classes;

namespace DiGi.GIS.WebAPI.UI
{
    public static partial class Query
    {
        /// <summary>
        /// Builds the GIS Web API terrain service URL for a circular area.
        /// <para>The radius is checked only for being a usable number: the ceiling on how large an area may be asked for belongs to the terrain service, which rejects an oversized request itself.</para>
        /// <para>An omitted tolerance is left out of the URL entirely rather than sent as a value of this application's choosing, so the terrain service applies its own default.</para>
        /// </summary>
        /// <param name="circle2D">The area to request the terrain surface for, in PL-1992 (EPSG:2180) metres. This value can be null.</param>
        /// <param name="tolerance">An optional tolerance for the spatial query, in metres.</param>
        /// <returns>The terrain service URL, or <see langword="null"/> when the area cannot be requested.</returns>
        public static string? TerrainRequestUri(this Circle2D? circle2D, double? tolerance = null)
        {
            Point2D? center = circle2D?.Center;
            if (center is null)
            {
                return null;
            }

            double radius = circle2D!.Radius;
            if (!double.IsFinite(center.X) || !double.IsFinite(center.Y) || !double.IsFinite(radius) || radius <= 0)
            {
                return null;
            }

            double radius_Query = System.Math.Min(radius, Constants.Default.TerrainRadiusMax);

            UrlBuilder urlBuilder = new($"{Constants.Default.TerrainUri}/mesh3dbycircle");
            urlBuilder = urlBuilder.AddParameter("x", center.X);
            urlBuilder = urlBuilder.AddParameter("y", center.Y);
            urlBuilder = urlBuilder.AddParameter("radius", radius_Query);

            if (tolerance.HasValue && double.IsFinite(tolerance.Value) && tolerance.Value >= 0)
            {
                urlBuilder = urlBuilder.AddParameter("tolerance", tolerance.Value);
            }

            return urlBuilder.ToString();
        }

        /// <summary>
        /// Builds the GIS Web API terrain service URL for an axis aligned rectangular area.
        /// <para>The corners are checked only for being usable numbers: the ceiling on how large an area may be asked for belongs to the terrain service, which rejects an oversized request itself.</para>
        /// <para>An omitted tolerance is left out of the URL entirely rather than sent as a value of this application's choosing, so the terrain service applies its own default.</para>
        /// </summary>
        /// <param name="boundingBox2D">The area to request the terrain surface for, in PL-1992 (EPSG:2180) metres. This value can be null.</param>
        /// <param name="tolerance">An optional tolerance for the spatial query, in metres.</param>
        /// <returns>The terrain service URL, or <see langword="null"/> when the area cannot be requested.</returns>
        public static string? TerrainRequestUri(this BoundingBox2D? boundingBox2D, double? tolerance = null)
        {
            if (boundingBox2D is null)
            {
                return null;
            }

            Point2D min = boundingBox2D.Min;
            Point2D max = boundingBox2D.Max;

            if (!double.IsFinite(min.X) || !double.IsFinite(min.Y) || !double.IsFinite(max.X) || !double.IsFinite(max.Y))
            {
                return null;
            }

            UrlBuilder urlBuilder = new($"{Constants.Default.TerrainUri}/mesh3dbyboundingbox");
            urlBuilder = urlBuilder.AddParameter("x_1", min.X);
            urlBuilder = urlBuilder.AddParameter("y_1", min.Y);
            urlBuilder = urlBuilder.AddParameter("x_2", max.X);
            urlBuilder = urlBuilder.AddParameter("y_2", max.Y);

            if (tolerance.HasValue && double.IsFinite(tolerance.Value) && tolerance.Value >= 0)
            {
                urlBuilder = urlBuilder.AddParameter("tolerance", tolerance.Value);
            }

            return urlBuilder.ToString();
        }
    }
}
