using DiGi.GIS.Classes;

namespace DiGi.GIS.WebAPI.UI
{
    public static partial class Query
    {
        // The default colors of the DiGi.Analytical building components (walls, floors, roofs)
        // are owned by the shared DiGi.GLTF.Analytical library (DiGi.GLTF.Analytical.Query.Color).

        /// <summary>
        /// Gets the default display <see cref="Core.Classes.Color"/> for the specified <see cref="Building2D"/>.
        /// </summary>
        /// <param name="building2D">The <see cref="Building2D"/> to be styled. This value can be null.</param>
        /// <returns>A <see cref="Core.Classes.Color"/> representing the default styling of the building, or null if <paramref name="building2D"/> is null.</returns>
        public static Core.Classes.Color? Color(this Building2D? building2D)
        {
            if (building2D is null)
            {
                return null;
            }

            return new Core.Classes.Color(byte.MaxValue, 222, 184, 135);
        }
    }
}
