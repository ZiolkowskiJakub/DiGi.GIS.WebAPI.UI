using DiGi.Analytical.Urban.Classes;
using DiGi.GLTF.Classes;
using System.Collections.Generic;

namespace DiGi.GIS.WebAPI.UI.Classes
{
    /// <summary>
    /// Converts an <see cref="UrbanModel"/> into <see cref="GLTFNode"/> instances by converting all contained building models (see <see cref="Convert.ToGLTF_GLTFNodes(UrbanModel?, double)"/>).
    /// <para>Registered automatically at startup by assembly scanning (see Program.cs).</para>
    /// </summary>
    public class UrbanModelGLTFNodeConverter : GLTFNodeConverter<UrbanModel>
    {
        /// <summary>
        /// Converts the specified <see cref="UrbanModel"/> into <see cref="GLTFNode"/> instances holding geometry in world coordinates.
        /// </summary>
        /// <param name="serializableObject">The <see cref="UrbanModel"/> to be converted.</param>
        /// <param name="tolerance">The distance tolerance used during triangulation.</param>
        /// <returns>A list of <see cref="GLTFNode"/> instances, or null if the model is empty.</returns>
        public override List<GLTFNode>? Convert(UrbanModel serializableObject, double tolerance)
        {
            return serializableObject.ToGLTF_GLTFNodes(tolerance);
        }
    }
}
