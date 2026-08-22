using DiGi.Geometry.Planar.Classes;
using System.Collections.Generic;

namespace DiGi.GIS.WebAPI.UI
{
    public static partial class Create
    {
        /// <summary>
        /// Creates a closed <see cref="DiGi.Geometry.Planar.Classes.Polygon2D"/> approximating the specified <see cref="Circle2D"/> with the given number of segments.
        /// </summary>
        /// <param name="circle2D">The circle to discretize. This value can be null.</param>
        /// <param name="segmentCount">The number of segments to divide the circle perimeter into. Defaults to <see cref="Constants.Default.TerrainCircleSegmentCount"/>.</param>
        /// <returns>A <see cref="DiGi.Geometry.Planar.Classes.Polygon2D"/> representing the discretized circle, or <see langword="null"/> if the circle is null or invalid.</returns>
        public static Polygon2D? Polygon2D(this Circle2D? circle2D, int segmentCount = Constants.Default.TerrainCircleSegmentCount)
        {
            if (circle2D is null || circle2D.Center is null || double.IsNaN(circle2D.Radius) || circle2D.Radius <= 0 || segmentCount < 3)
            {
                return null;
            }

            Point2D center = circle2D.Center;
            double radius = circle2D.Radius;
            List<Point2D> point2Ds = [];
            double step = (2 * System.Math.PI) / segmentCount;

            for (int i = 0; i < segmentCount; i++)
            {
                double angle = i * step;
                point2Ds.Add(new Point2D(center.X + (radius * System.Math.Cos(angle)), center.Y + (radius * System.Math.Sin(angle))));
            }

            return new Polygon2D(point2Ds);
        }

        /// <summary>
        /// Creates a 4-corner rectangular <see cref="DiGi.Geometry.Planar.Classes.Polygon2D"/> from the specified <see cref="BoundingBox2D"/>.
        /// </summary>
        /// <param name="boundingBox2D">The 2D bounding box to convert. This value can be null.</param>
        /// <returns>A <see cref="DiGi.Geometry.Planar.Classes.Polygon2D"/> representing the bounding box rectangle, or <see langword="null"/> if the bounding box is null or invalid.</returns>
        public static Polygon2D? Polygon2D(this BoundingBox2D? boundingBox2D)
        {
            if (boundingBox2D is null)
            {
                return null;
            }

            List<Point2D>? point2Ds = boundingBox2D.GetPoints();
            if (point2Ds is null || point2Ds.Count < 3)
            {
                return null;
            }

            return new Polygon2D(point2Ds);
        }
    }
}
