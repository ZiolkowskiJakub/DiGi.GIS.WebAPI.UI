using DiGi.Geometry.Planar.Classes;

namespace DiGi.GIS.WebAPI.UI
{
    public static partial class Query
    {
        /// <summary>
        /// Sizes the axis aligned rectangle to ask the GIS Web API terrain service for so that the surface it answers contains the given display rectangle.
        /// <para>Every side is moved outwards by the clip <paramref name="buffer"/> plus one lattice diagonal of the coarsest lattice in use, for the reason given on <see cref="TerrainQueryCircle(Circle2D?, double, double, double)"/>. The ceiling on how large an area may be requested belongs to the terrain service and is not applied here.</para>
        /// </summary>
        /// <param name="boundingBox2D">The rectangle the surface is displayed and clipped to, in PL-1992 (EPSG:2180) metres. This value can be null.</param>
        /// <param name="buffer">The clip margin in metres added beyond the display boundary.</param>
        /// <param name="latticeStep">The coarsest lattice step in metres the stored elevation points may be sampled on; the query grows by its diagonal.</param>
        /// <returns>The rectangle to request, or <see langword="null"/> when the display rectangle cannot be requested.</returns>
        public static BoundingBox2D? TerrainQueryBoundingBox(this BoundingBox2D? boundingBox2D, double buffer = Constants.Default.TerrainBuffer, double latticeStep = Constants.Default.TerrainLatticeStepMax)
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

            double extension = System.Math.Max(0, buffer) + (System.Math.Max(0, latticeStep) * System.Math.Sqrt(2));

            return new BoundingBox2D(new Point2D(min.X - extension, min.Y - extension), new Point2D(max.X + extension, max.Y + extension));
        }
    }
}
