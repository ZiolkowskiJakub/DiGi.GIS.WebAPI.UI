namespace DiGi.GIS.WebAPI.UI
{
    public static partial class Create
    {
        /// <summary>
        /// Creates the view range of a single building scene: the slider starts at its minimum and can be moved to the right up to <see cref="Constants.Default.BuildingViewRangeFactor"/> times the minimum.
        /// </summary>
        /// <param name="radius">The requested minimum view range, in metres; null means <see cref="Constants.Default.TerrainRadius"/>. This value can be null.</param>
        /// <returns>A <see cref="Classes.ViewRange"/>, or null if the radius is not finite, not positive or its maximum exceeds <see cref="Constants.Default.TerrainRadiusMax"/>, which is as far as the ground can be loaded.</returns>
        public static Classes.ViewRange? BuildingViewRange(double? radius)
        {
            double minimum = radius ?? Constants.Default.TerrainRadius;
            if (!double.IsFinite(minimum) || minimum <= 0)
            {
                return null;
            }

            double maximum = minimum * Constants.Default.BuildingViewRangeFactor;
            if (maximum > Constants.Default.TerrainRadiusMax)
            {
                return null;
            }

            return new Classes.ViewRange(minimum, maximum, minimum);
        }
    }
}
