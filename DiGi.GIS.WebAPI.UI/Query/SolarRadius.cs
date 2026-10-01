namespace DiGi.GIS.WebAPI.UI
{
    public static partial class Query
    {
        /// <summary>
        /// Gets the neighbour radius the Building Viewer's "Solar radiation" panel calculates with: the scene radius of the page, limited to what a solar calculation admits.
        /// <para>The Building Viewer admits a scene radius of up to a third of <see cref="Constants.Default.TerrainRadiusMax"/>, while a solar calculation refuses a neighbour radius above <see cref="Constants.Default.SolarSurroundingRadiusMax"/> with a 400, so the panel clamps rather than failing, and the Results card reports the limit.</para>
        /// </summary>
        /// <param name="radius">The scene radius of the page, in metres; null means <see cref="Constants.Default.TerrainRadius"/>, the scene default.</param>
        /// <returns>The neighbour radius in metres, at most <see cref="Constants.Default.SolarSurroundingRadiusMax"/>.</returns>
        public static double SolarRadius(double? radius)
        {
            return System.Math.Min(radius ?? Constants.Default.TerrainRadius, Constants.Default.SolarSurroundingRadiusMax);
        }
    }
}
