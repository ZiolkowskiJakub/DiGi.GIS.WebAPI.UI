using DiGi.Analytical.Building.Classes;
using DiGi.Geometry.Planar.Classes;
using DiGi.Geometry.Spatial.Classes;
using DiGi.GLTF.Classes;
using System.Collections.Generic;

namespace DiGi.GIS.WebAPI.UI
{
    public static partial class Create
    {
        /// <summary>
        /// Creates the <see cref="GLTFNode"/> holding the ground surface of a scene with the outlines of the given buildings cut out of it.
        /// <para>The surface and the buildings both carry real elevations, so an uncut surface runs straight through the ground floors and basements standing on it. Cutting the outline of every building out of it leaves the interiors clear and the ground meeting the walls from outside.</para>
        /// <para>A small outward offset can be applied to expand the cut opening beyond the building footprint to prevent visual z-fighting or ground clipping against the wall base.</para>
        /// </summary>
        /// <remarks>
        /// The node is returned untouched whenever there is nothing to cut - no node, no surface, or no building covering any ground - so a caller can apply this to whatever the terrain service answered without checking first.
        /// </remarks>
        /// <param name="gLTFNode">The node holding the ground surface, as created by <see cref="TerrainGLTFNodeAsync(System.Net.Http.HttpClient?, string?, string?, System.Threading.CancellationToken)"/>. This value can be null.</param>
        /// <param name="buildingModels">The buildings standing on the surface. This value can be null.</param>
        /// <param name="offset">The outward offset distance in meters applied to the building footprints before cutting the terrain.</param>
        /// <param name="tolerance">The distance tolerance used for the outlines and for the subtraction.</param>
        /// <returns>A new node holding the surface with the buildings cut out, the given node when there is nothing to cut, or <see langword="null"/> when the buildings cover the whole surface.</returns>
        public static GLTFNode? TerrainGLTFNode(this GLTFNode? gLTFNode, IEnumerable<BuildingModel>? buildingModels, double offset = 0.05, double tolerance = Core.Constants.Tolerance.Distance)
        {
            return TerrainGLTFNode(gLTFNode, buildingModels, (Circle2D?)null, offset, tolerance);
        }

        /// <summary>
        /// Creates the <see cref="GLTFNode"/> holding the ground surface of a scene with a regular circular boundary applied and the outlines of the given buildings cut out of it.
        /// </summary>
        /// <param name="gLTFNode">The node holding the ground surface. This value can be null.</param>
        /// <param name="buildingModels">The buildings standing on the surface. This value can be null.</param>
        /// <param name="boundaryCircle">The circular boundary to clip the ground surface to. When null, no boundary clipping is performed.</param>
        /// <param name="offset">The outward offset distance in meters applied to the building footprints before cutting the terrain.</param>
        /// <param name="tolerance">The distance tolerance used for the outlines and for the subtraction.</param>
        /// <returns>A new node holding the clipped surface with building footprints cut out, or <see langword="null"/> if no surface remains.</returns>
        public static GLTFNode? TerrainGLTFNode(this GLTFNode? gLTFNode, IEnumerable<BuildingModel>? buildingModels, Circle2D? boundaryCircle, double offset = 0.05, double tolerance = Core.Constants.Tolerance.Distance)
        {
            Mesh3D? mesh3D = gLTFNode?.Mesh3D;
            if (gLTFNode is null || mesh3D is null)
            {
                return gLTFNode;
            }

            if (boundaryCircle is not null)
            {
                mesh3D = Modify.Clip(mesh3D, boundaryCircle, tolerance: tolerance);
                if (mesh3D is null)
                {
                    return null;
                }
            }

            if (buildingModels is null)
            {
                return new GLTFNode(gLTFNode.Name, gLTFNode.Reference, mesh3D, gLTFNode.Color, gLTFNode.Opacity, gLTFNode.Properties);
            }

            List<PolygonalFace2D>? polygonalFace2Ds = DiGi.Analytical.Building.Query.Footprints(buildingModels, tolerance);
            if (polygonalFace2Ds is null || polygonalFace2Ds.Count == 0)
            {
                return new GLTFNode(gLTFNode.Name, gLTFNode.Reference, mesh3D, gLTFNode.Color, gLTFNode.Opacity, gLTFNode.Properties);
            }

            if (offset != 0 && !double.IsNaN(offset))
            {
                polygonalFace2Ds = Geometry.Planar.Query.Offset(polygonalFace2Ds, offset);
                if (polygonalFace2Ds is null || polygonalFace2Ds.Count == 0)
                {
                    return new GLTFNode(gLTFNode.Name, gLTFNode.Reference, mesh3D, gLTFNode.Color, gLTFNode.Opacity, gLTFNode.Properties);
                }
            }

            Mesh3D? mesh3D_Cut = Geometry.Spatial.Query.Difference(mesh3D, polygonalFace2Ds, tolerance);
            if (mesh3D_Cut is null)
            {
                return null;
            }

            return new GLTFNode(gLTFNode.Name, gLTFNode.Reference, mesh3D_Cut, gLTFNode.Color, gLTFNode.Opacity, gLTFNode.Properties);
        }

        /// <summary>
        /// Creates the <see cref="GLTFNode"/> holding the ground surface of a scene with a regular rectangular boundary applied and the outlines of the given buildings cut out of it.
        /// </summary>
        /// <param name="gLTFNode">The node holding the ground surface. This value can be null.</param>
        /// <param name="buildingModels">The buildings standing on the surface. This value can be null.</param>
        /// <param name="boundaryBoundingBox">The rectangular bounding box to clip the ground surface to. When null, no boundary clipping is performed.</param>
        /// <param name="offset">The outward offset distance in meters applied to the building footprints before cutting the terrain.</param>
        /// <param name="tolerance">The distance tolerance used for the outlines and for the subtraction.</param>
        /// <returns>A new node holding the clipped surface with building footprints cut out, or <see langword="null"/> if no surface remains.</returns>
        public static GLTFNode? TerrainGLTFNode(this GLTFNode? gLTFNode, IEnumerable<BuildingModel>? buildingModels, BoundingBox2D? boundaryBoundingBox, double offset = 0.05, double tolerance = Core.Constants.Tolerance.Distance)
        {
            Mesh3D? mesh3D = gLTFNode?.Mesh3D;
            if (gLTFNode is null || mesh3D is null)
            {
                return gLTFNode;
            }

            if (boundaryBoundingBox is not null)
            {
                mesh3D = Modify.Clip(mesh3D, boundaryBoundingBox, tolerance: tolerance);
                if (mesh3D is null)
                {
                    return null;
                }
            }

            if (buildingModels is null)
            {
                return new GLTFNode(gLTFNode.Name, gLTFNode.Reference, mesh3D, gLTFNode.Color, gLTFNode.Opacity, gLTFNode.Properties);
            }

            List<PolygonalFace2D>? polygonalFace2Ds = DiGi.Analytical.Building.Query.Footprints(buildingModels, tolerance);
            if (polygonalFace2Ds is null || polygonalFace2Ds.Count == 0)
            {
                return new GLTFNode(gLTFNode.Name, gLTFNode.Reference, mesh3D, gLTFNode.Color, gLTFNode.Opacity, gLTFNode.Properties);
            }

            if (offset != 0 && !double.IsNaN(offset))
            {
                polygonalFace2Ds = Geometry.Planar.Query.Offset(polygonalFace2Ds, offset);
                if (polygonalFace2Ds is null || polygonalFace2Ds.Count == 0)
                {
                    return new GLTFNode(gLTFNode.Name, gLTFNode.Reference, mesh3D, gLTFNode.Color, gLTFNode.Opacity, gLTFNode.Properties);
                }
            }

            Mesh3D? mesh3D_Cut = Geometry.Spatial.Query.Difference(mesh3D, polygonalFace2Ds, tolerance);
            if (mesh3D_Cut is null)
            {
                return null;
            }

            return new GLTFNode(gLTFNode.Name, gLTFNode.Reference, mesh3D_Cut, gLTFNode.Color, gLTFNode.Opacity, gLTFNode.Properties);
        }
    }
}
