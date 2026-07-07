using DiGi.Communication.Classes;
using DiGi.Geometry.Planar.Classes;
using DiGi.Geometry.Spatial;
using DiGi.Geometry.Spatial.Classes;
using DiGi.GIS.Classes;
using System.Collections.Generic;

namespace DiGi.GIS.WebAPI.UI
{
    public static partial class Convert
    {
        // Building -> Mesh3D -> ScatteringObject conversion keeps the strict dependency rule intact:
        // DiGi.Communication and DiGi.Communication.WebAPI never reference any GIS library, so the
        // GIS domain objects are reduced to plain triangulated geometry before leaving this project.

        /// <summary>
        /// Converts the specified <see cref="Building2D"/> into a <see cref="ScatteringObject"/> by extruding its 2D polygonal footprint by the number of storeys multiplied by the storey height and triangulating the resulting polyhedron into a <see cref="Mesh3D"/>.
        /// </summary>
        /// <param name="building2D">The <see cref="Building2D"/> to be converted. This value can be null.</param>
        /// <param name="storeyHeight">The height of a single storey in meters used for the extrusion.</param>
        /// <param name="tolerance">The distance tolerance used during triangulation.</param>
        /// <returns>A <see cref="ScatteringObject"/> holding the triangulated building geometry in world coordinates, or null if the building or its geometry is null.</returns>
        public static ScatteringObject? ToCommunication_ScatteringObject(this Building2D? building2D, double storeyHeight = Constants.Default.StoreyHeight, double tolerance = DiGi.Core.Constants.Tolerance.Distance)
        {
            PolygonalFace2D? polygonalFace2D = building2D?.PolygonalFace2D;
            if (building2D is null || polygonalFace2D is null)
            {
                return null;
            }

            Plane? plane = Geometry.Spatial.Create.Plane(0);
            if (plane is null)
            {
                return null;
            }

            PolygonalFace3D? polygonalFace3D = plane.Convert(polygonalFace2D);
            if (polygonalFace3D is null)
            {
                return null;
            }

            int storeys = building2D.Storeys < 1 ? 1 : building2D.Storeys;

            PolygonalFaceExtrusion polygonalFaceExtrusion = new(polygonalFace3D, new Vector3D(0, 0, storeys * storeyHeight));

            Polyhedron? polyhedron = Geometry.Spatial.Create.Polyhedron(polygonalFaceExtrusion, tolerance);
            if (polyhedron is null)
            {
                return null;
            }

            Mesh3D? mesh3D = Geometry.Spatial.Create.Mesh3D(polyhedron, tolerance);
            if (mesh3D is null)
            {
                return null;
            }

            string? reference = building2D.Reference ?? Core.Create.UniqueReference(building2D)?.ToString();

            return new ScatteringObject(reference, mesh3D);
        }

        /// <summary>
        /// Converts the specified <see cref="Building2D"/> collection into <see cref="ScatteringObject"/> instances (see <see cref="ToCommunication_ScatteringObject(Building2D, double, double)"/>).
        /// </summary>
        /// <param name="building2Ds">The <see cref="Building2D"/> collection to be converted. This value can be null.</param>
        /// <param name="storeyHeight">The height of a single storey in meters used for the extrusions.</param>
        /// <param name="tolerance">The distance tolerance used during triangulation.</param>
        /// <returns>A list of <see cref="ScatteringObject"/> instances for all convertible buildings, or null if <paramref name="building2Ds"/> is null.</returns>
        public static List<ScatteringObject>? ToCommunication_ScatteringObjects(this IEnumerable<Building2D>? building2Ds, double storeyHeight = Constants.Default.StoreyHeight, double tolerance = DiGi.Core.Constants.Tolerance.Distance)
        {
            if (building2Ds is null)
            {
                return null;
            }

            List<ScatteringObject> result = [];
            foreach (Building2D building2D in building2Ds)
            {
                ScatteringObject? scatteringObject = ToCommunication_ScatteringObject(building2D, storeyHeight, tolerance);
                if (scatteringObject is not null)
                {
                    result.Add(scatteringObject);
                }
            }

            return result;
        }
    }
}
