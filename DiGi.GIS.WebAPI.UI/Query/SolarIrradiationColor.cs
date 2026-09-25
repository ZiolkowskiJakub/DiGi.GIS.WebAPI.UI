namespace DiGi.GIS.WebAPI.UI
{
    public static partial class Query
    {
        /// <summary>
        /// Maps an annual irradiation to the colour the solar radiation viewer paints a surface with, on a fixed ramp from 0 (dark blue) through pale yellow to <see cref="Constants.Default.SolarIrradiationScaleMax"/> (dark red).
        /// <para>The ramp is fixed rather than fitted to each building, so two buildings read the same colour for the same irradiation. Values outside the range take the end colours, and a value that is not finite takes the bottom one. The same ramp draws the viewer's legend, so the two cannot drift apart.</para>
        /// </summary>
        /// <param name="irradiation">The annual irradiation, in kWh/m² per year.</param>
        /// <returns>The opaque colour of the irradiation.</returns>
        public static Core.Classes.Color SolarIrradiationColor(double irradiation)
        {
            // Five evenly spaced stops of a diverging blue-yellow-red ramp, low to high.
            System.Drawing.Color[] colors =
            [
                System.Drawing.Color.FromArgb(49, 54, 149),
                System.Drawing.Color.FromArgb(116, 173, 209),
                System.Drawing.Color.FromArgb(255, 255, 191),
                System.Drawing.Color.FromArgb(244, 109, 67),
                System.Drawing.Color.FromArgb(165, 0, 38),
            ];

            double value = double.IsFinite(irradiation) ? irradiation / Constants.Default.SolarIrradiationScaleMax : 0;
            value = System.Math.Clamp(value, 0, 1) * (colors.Length - 1);

            int index = System.Math.Min((int)value, colors.Length - 2);

            return new Core.Classes.Color(Core.Query.Lerp(colors[index], colors[index + 1], value - index));
        }
    }
}
