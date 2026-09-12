using DiGi.Geometry.Visual.Core.Classes;

namespace DiGi.GIS.WebAPI.UI
{
    public static partial class Create
    {
        /// <summary>
        /// Creates the bucket appearance a Typology definition document carries for one color of the page.
        /// <para>The page edits one color per bucket; the document needs an appearance per shape kind, so the color is written into a point, a face and a mesh appearance with <see cref="Constants.Default.TypologyAppearanceThickness"/> on every kind - the point kind because the typology is displayed as points, and the face and mesh kinds for the viewers that render shapes. <see cref="Query.Color(Typology.Visual.Classes.TypologyAppearance)"/> reads the color back.</para>
        /// </summary>
        /// <param name="color">The bucket color.</param>
        /// <returns>The appearance.</returns>
        public static Typology.Visual.Classes.TypologyAppearance TypologyAppearance(this Core.Classes.Color color)
        {
            double thickness = Constants.Default.TypologyAppearanceThickness;

            return new Typology.Visual.Classes.TypologyAppearance([new PointAppearance(color, thickness), new FaceAppearance(color, color, thickness), new MeshAppearance(color, color, thickness)]);
        }
    }
}
