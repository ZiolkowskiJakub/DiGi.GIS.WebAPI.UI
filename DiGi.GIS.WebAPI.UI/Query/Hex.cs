using System.Globalization;

namespace DiGi.GIS.WebAPI.UI
{
    public static partial class Query
    {
        /// <summary>
        /// Renders a color as the <c>#rrggbb</c> string a color picker holds.
        /// <para>The alpha channel is dropped: the Typology definition page edits opaque colors only, and <c>Core.Convert.ToDrawing(string)</c> reads an eight digit form as RGBA rather than ARGB, so emitting it would round-trip wrongly. DiGi.Core ships only the parser; this is its emitter for the page.</para>
        /// </summary>
        /// <param name="color">The color to render. This value can be null.</param>
        /// <returns>The lower case <c>#rrggbb</c> string, or <see langword="null"/> when <paramref name="color"/> is null.</returns>
        public static string? Hex(this Core.Classes.Color? color)
        {
            if (color == null)
            {
                return null;
            }

            return string.Format(CultureInfo.InvariantCulture, "#{0:x2}{1:x2}{2:x2}", color.Red, color.Green, color.Blue);
        }
    }
}
