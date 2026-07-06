using DiGi.Analytical.Building.Interfaces;
using DiGi.Core.Interfaces;
using DiGi.GLTF.Classes;
using DiGi.GLTF.Interfaces;
using System.Collections.Generic;

namespace DiGi.GIS.WebAPI.UI.Classes
{
    /// <summary>
    /// Converts a standalone building <see cref="IComponent"/> (for example a wall or a roof) into <see cref="GLTFNode"/> instances using its surface geometry (see <see cref="Convert.ToGLTF_GLTFNodes(IComponent?, double)"/>).
    /// <para>Implemented against <see cref="IGLTFNodeConverter"/> directly because <see cref="IComponent"/> is an interface matched by type test rather than a concrete serializable class. Registered automatically at startup by assembly scanning (see Program.cs).</para>
    /// </summary>
    public class ComponentGLTFNodeConverter : IGLTFNodeConverter
    {
        /// <summary>
        /// Determines whether this converter can convert the specified object.
        /// </summary>
        /// <param name="serializableObject">The domain object to be checked.</param>
        /// <returns>True if the object is an <see cref="IComponent"/>; otherwise, false.</returns>
        public bool CanConvert(ISerializableObject serializableObject)
        {
            return serializableObject is IComponent;
        }

        /// <summary>
        /// Converts the specified building component into <see cref="GLTFNode"/> instances holding geometry in world coordinates.
        /// </summary>
        /// <param name="serializableObject">The <see cref="IComponent"/> to be converted.</param>
        /// <param name="tolerance">The distance tolerance used during triangulation.</param>
        /// <returns>A list of <see cref="GLTFNode"/> instances, or null if the component has no supported surface geometry.</returns>
        public List<GLTFNode>? Convert(ISerializableObject serializableObject, double tolerance)
        {
            if (serializableObject is not IComponent component)
            {
                return null;
            }

            return component.ToGLTF_GLTFNodes(tolerance);
        }
    }
}
