using DiGi.Core.Interfaces;
using DiGi.GIS.Classes;
using DiGi.GLTF.Classes;
using System.Collections.Generic;

namespace DiGi.GIS.WebAPI.UI
{
    public static partial class Create
    {
        // Generic ISerializableObject scenes are created directly by the DiGi.GLTF engine
        // (GLTF.Create.GLTFScene): the registered converter classes handle the domain types.

        /// <summary>
        /// Creates a <see cref="GLTF.Classes.GLTFScene"/> from the specified <see cref="Building2D"/> collection by extruding each footprint by its storeys and translating all geometry to a local origin (0, 0, 0).
        /// <para>The world offset removed from the geometry is stored in <see cref="GLTF.Classes.GLTFScene.ReferencePoint"/> so the camera can automatically frame the whole area. Default lighting and an automatically framing camera are added.</para>
        /// </summary>
        /// <param name="building2Ds">The <see cref="Building2D"/> collection to be displayed. This value can be null.</param>
        /// <param name="name">The display name of the scene.</param>
        /// <param name="storeyHeight">The height of a single storey in meters used for the extrusions.</param>
        /// <param name="tolerance">The distance tolerance used during triangulation.</param>
        /// <returns>A <see cref="GLTF.Classes.GLTFScene"/> holding the converted buildings, or null if <paramref name="building2Ds"/> is null or contains no convertible buildings.</returns>
        public static GLTFScene? GLTFScene(this IEnumerable<Building2D>? building2Ds, string? name = null, double storeyHeight = Constants.Default.StoreyHeight, double tolerance = DiGi.Core.Constants.Tolerance.Distance)
        {
            List<GLTFNode>? gLTFNodes = Convert.ToGLTF_GLTFNodes(building2Ds, storeyHeight, tolerance);
            if (gLTFNodes is null || gLTFNodes.Count == 0)
            {
                return null;
            }

            return GLTF.Create.GLTFScene(gLTFNodes, name);
        }
    }
}
