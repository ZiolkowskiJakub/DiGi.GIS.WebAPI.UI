using DiGi.Geometry.Spatial.Classes;
using DiGi.Geometry.Spatial.Interfaces;
using DiGi.Solar.Classes;
using DiGi.Solar.Interfaces;
using System.Collections.Generic;

namespace DiGi.GIS.WebAPI.UI
{
    public static partial class Query
    {
        /// <summary>
        /// Counts the triangles of the shading-only elements of a shading model - the neighbours and the analysed building's own non-receiving components - which every receiver is solved against.
        /// <para>The elements are triangulated exactly as <see cref="ShadingSolver.Solve"/> triangulates its casters, so the count is the quantity <see cref="Constants.Default.SolarCasterTriangleCountMax"/> was measured on (ZiolkowskiJakub/DiGi.Solar#7). The receivers' own triangles are not counted.</para>
        /// </summary>
        /// <param name="shadingModel">The shading model. This value can be null.</param>
        /// <param name="tolerance">The distance tolerance used by the triangulation.</param>
        /// <returns>The number of triangles, or 0 when the model is null or has no shading-only element.</returns>
        public static int CasterTriangleCount(this ShadingModel? shadingModel, double tolerance = Core.Constants.Tolerance.Distance)
        {
            List<IShadingElement>? shadingElements = shadingModel?.GetShadingElements<IShadingElement>(true);
            if (shadingElements is null)
            {
                return 0;
            }

            int result = 0;
            foreach (IShadingElement shadingElement in shadingElements)
            {
                IPolygonalFace3D? polygonalFace3D = shadingElement?.PolygonalFace3D;

                List<Triangle3D>? triangle3Ds = polygonalFace3D?.Triangulate(tolerance);
                if (triangle3Ds is not null)
                {
                    result += triangle3Ds.Count;
                }
            }

            return result;
        }
    }
}
