using DiGi.Analytical.Building.Classes;
using DiGi.GLTF.Classes;
using System.Collections.Generic;

namespace DiGi.GIS.WebAPI.UI.Classes
{
    /// <summary>
    /// Converts a <see cref="BuildingModel"/> into <see cref="GLTFNode"/> instances by converting all of its components (see <see cref="Convert.ToGLTF_GLTFNodes(BuildingModel?, double)"/>).
    /// <para>Registered automatically at startup by assembly scanning (see Program.cs).</para>
    /// </summary>
    public class BuildingModelGLTFNodeConverter : GLTFNodeConverter<BuildingModel>
    {
        /// <summary>
        /// Converts the specified <see cref="BuildingModel"/> into <see cref="GLTFNode"/> instances holding geometry in world coordinates.
        /// </summary>
        /// <param name="serializableObject">The <see cref="BuildingModel"/> to be converted.</param>
        /// <param name="tolerance">The distance tolerance used during triangulation.</param>
        /// <returns>A list of <see cref="GLTFNode"/> instances, or null if the model has no convertible components.</returns>
        public override List<GLTFNode>? Convert(BuildingModel serializableObject, double tolerance)
        {
            return serializableObject.ToGLTF_GLTFNodes(tolerance);
        }
    }
}
