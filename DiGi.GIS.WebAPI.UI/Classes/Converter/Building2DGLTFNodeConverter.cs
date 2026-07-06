using DiGi.GIS.Classes;
using DiGi.GLTF.Classes;
using System.Collections.Generic;

namespace DiGi.GIS.WebAPI.UI.Classes
{
    /// <summary>
    /// Converts a <see cref="Building2D"/> into <see cref="GLTFNode"/> instances by extruding its 2D footprint by the number of storeys (see <see cref="Convert.ToGLTF_GLTFNodes(Building2D?, double, double)"/>).
    /// <para>Registered automatically at startup by assembly scanning (see Program.cs); the generic DiGi.GLTF engine consults it when converting <see cref="Core.Interfaces.ISerializableObject"/> instances.</para>
    /// </summary>
    public class Building2DGLTFNodeConverter : GLTFNodeConverter<Building2D>
    {
        /// <summary>
        /// Converts the specified <see cref="Building2D"/> into <see cref="GLTFNode"/> instances holding geometry in world coordinates.
        /// </summary>
        /// <param name="serializableObject">The <see cref="Building2D"/> to be converted.</param>
        /// <param name="tolerance">The distance tolerance used during triangulation.</param>
        /// <returns>A list of <see cref="GLTFNode"/> instances, or null if the building has no geometry.</returns>
        public override List<GLTFNode>? Convert(Building2D serializableObject, double tolerance)
        {
            return serializableObject.ToGLTF_GLTFNodes(Constants.Default.StoreyHeight, tolerance);
        }
    }
}
