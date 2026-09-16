using DiGi.Geometry.Planar.Classes;

namespace DiGi.GIS.WebAPI.UI
{
    public static partial class Query
    {
        /// <summary>
        /// Sizes the circle to ask the GIS Web API terrain service for so that the surface it answers contains the given display circle.
        /// <para>The service triangulates only the stored lattice points inside the query circle, so its surface stops short of the query radius by up to one lattice diagonal (see <see cref="Constants.Default.TerrainLatticeStepMax"/>). The query is therefore grown by that diagonal for the coarsest lattice in use, plus the clip <paramref name="buffer"/>, and the result is guaranteed to cover the display circle whenever <paramref name="maximumRadius"/> leaves room for the full extension. Where it does not - a display circle within the extension of the service's cap - the query is as large as the service admits, which is still the larger of the two sides to err on.</para>
        /// </summary>
        /// <param name="circle2D">The circle the surface is displayed and clipped to, in PL-1992 (EPSG:2180) metres. This value can be null.</param>
        /// <param name="buffer">The clip margin in metres added beyond the display boundary.</param>
        /// <param name="latticeStep">The coarsest lattice step in metres the stored elevation points may be sampled on; the query grows by its diagonal.</param>
        /// <param name="maximumRadius">The largest radius the terrain service accepts; both the display radius and the query radius are capped by it.</param>
        /// <returns>The circle to request, or <see langword="null"/> when the display circle cannot be requested.</returns>
        public static Circle2D? TerrainQueryCircle(this Circle2D? circle2D, double buffer = Constants.Default.TerrainBuffer, double latticeStep = Constants.Default.TerrainLatticeStepMax, double maximumRadius = Constants.Default.TerrainRadiusMax)
        {
            Point2D? center = circle2D?.Center;
            if (center is null)
            {
                return null;
            }

            double radius = circle2D!.Radius;
            if (!double.IsFinite(radius) || radius <= 0)
            {
                return null;
            }

            double radius_Display = System.Math.Min(radius, maximumRadius);
            double extension = System.Math.Max(0, buffer) + (System.Math.Max(0, latticeStep) * System.Math.Sqrt(2));
            double radius_Query = System.Math.Min(radius_Display + extension, maximumRadius);

            return new Circle2D(center, radius_Query);
        }
    }
}
