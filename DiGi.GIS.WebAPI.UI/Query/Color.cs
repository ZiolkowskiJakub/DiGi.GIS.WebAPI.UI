using DiGi.Geometry.Visual.Core.Classes;
using DiGi.Geometry.Visual.Core.Interfaces;
using DiGi.Typology.Visual.Classes;
using System;

namespace DiGi.GIS.WebAPI.UI
{
    public static partial class Query
    {
        /// <summary>
        /// Reads the single color the Typology definition page shows for a bucket appearance.
        /// <para>A <see cref="TypologyAppearance"/> holds one appearance per shape kind; the page edits one color per bucket and writes it into every kind (<see cref="Create.TypologyAppearance(Core.Classes.Color)"/>), so any of them answers. The point appearance is preferred because the typology is displayed as points; a document written elsewhere may carry only some kinds, so the first appearance carrying a color is the fallback.</para>
        /// </summary>
        /// <param name="typologyAppearance">The bucket appearance. This value can be null.</param>
        /// <returns>The color, or <see langword="null"/> when no appearance carries one.</returns>
        public static Core.Classes.Color? Color(this TypologyAppearance? typologyAppearance)
        {
            if (typologyAppearance == null)
            {
                return null;
            }

            if (typologyAppearance[typeof(PointAppearance)]?.Color is Core.Classes.Color color)
            {
                return color;
            }

            foreach (Type type in typologyAppearance.Types)
            {
                if (typologyAppearance[type] is IAppearance appearance && appearance.Color != null)
                {
                    return appearance.Color;
                }
            }

            return null;
        }
    }
}
