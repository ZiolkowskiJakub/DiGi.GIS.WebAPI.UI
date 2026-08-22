using DiGi.Geometry.Planar.Classes;
using DiGi.Geometry.Spatial.Classes;
using System.Collections.Generic;

namespace DiGi.GIS.WebAPI.UI
{
    public static partial class Modify
    {
        /// <summary>
        /// Clips a 3D mesh in plan view to the interior of the specified boundary polygon.
        /// </summary>
        /// <param name="mesh3D">The mesh to clip. This value can be null.</param>
        /// <param name="boundaryPolygon">The boundary polygon defining the allowed plan view region. This value can be null.</param>
        /// <param name="tolerance">The distance tolerance used for the plan view clipping.</param>
        /// <returns>A new <see cref="Mesh3D"/> clipped to the boundary polygon, or <see langword="null"/> if no geometry remains.</returns>
        public static Mesh3D? Clip(this Mesh3D? mesh3D, Polygon2D? boundaryPolygon, double tolerance = Core.Constants.Tolerance.Distance)
        {
            if (mesh3D is null)
            {
                return null;
            }

            List<Point2D>? point2Ds_Boundary = boundaryPolygon?.GetPoints();
            if (boundaryPolygon is null || point2Ds_Boundary is null || point2Ds_Boundary.Count < 3)
            {
                return new Mesh3D(mesh3D);
            }

            BoundingBox3D? boundingBox3D = mesh3D.GetBoundingBox();
            if (boundingBox3D is null)
            {
                return new Mesh3D(mesh3D);
            }

            double margin = 100.0;
            BoundingBox2D boundingBox2D_Outer = new(
                new Point2D(boundingBox3D.MinX - margin, boundingBox3D.MinY - margin),
                new Point2D(boundingBox3D.MaxX + margin, boundingBox3D.MaxY + margin));

            Polygon2D? polygon2D_Outer = boundingBox2D_Outer.Polygon2D();
            if (polygon2D_Outer is null)
            {
                return new Mesh3D(mesh3D);
            }

            PolygonalFace2D? cutter = Geometry.Planar.Create.PolygonalFace2D(polygon2D_Outer, [boundaryPolygon], tolerance);
            if (cutter is null)
            {
                return new Mesh3D(mesh3D);
            }

            return Geometry.Spatial.Query.Difference(mesh3D, [cutter], tolerance);
        }

        /// <summary>
        /// Clips a 3D mesh in plan view to the regular circular boundary defined by the specified <see cref="Circle2D"/>.
        /// </summary>
        /// <param name="mesh3D">The mesh to clip. This value can be null.</param>
        /// <param name="circle2D">The circle defining the allowed plan view boundary. This value can be null.</param>
        /// <param name="segmentCount">The number of segments used to discretize the circular boundary.</param>
        /// <param name="tolerance">The distance tolerance used for the plan view clipping.</param>
        /// <returns>A new <see cref="Mesh3D"/> clipped to the circular boundary, or <see langword="null"/> if no geometry remains.</returns>
        public static Mesh3D? Clip(this Mesh3D? mesh3D, Circle2D? circle2D, int segmentCount = Constants.Default.TerrainCircleSegmentCount, double tolerance = Core.Constants.Tolerance.Distance)
        {
            if (mesh3D is null || circle2D is null)
            {
                return mesh3D;
            }

            Polygon2D? polygon2D = circle2D.Polygon2D(segmentCount);
            return Clip(mesh3D, polygon2D, tolerance);
        }

        /// <summary>
        /// Clips a 3D mesh in plan view to the rectangular boundary defined by the specified <see cref="BoundingBox2D"/>.
        /// </summary>
        /// <param name="mesh3D">The mesh to clip. This value can be null.</param>
        /// <param name="boundingBox2D">The bounding box defining the allowed plan view boundary. This value can be null.</param>
        /// <param name="tolerance">The distance tolerance used for the plan view clipping.</param>
        /// <returns>A new <see cref="Mesh3D"/> clipped to the rectangular boundary, or <see langword="null"/> if no geometry remains.</returns>
        public static Mesh3D? Clip(this Mesh3D? mesh3D, BoundingBox2D? boundingBox2D, double tolerance = Core.Constants.Tolerance.Distance)
        {
            if (mesh3D is null || boundingBox2D is null)
            {
                return mesh3D;
            }

            Polygon2D? polygon2D = boundingBox2D.Polygon2D();
            return Clip(mesh3D, polygon2D, tolerance);
        }
    }
}
