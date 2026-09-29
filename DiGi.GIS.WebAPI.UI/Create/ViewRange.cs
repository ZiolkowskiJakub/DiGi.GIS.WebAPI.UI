using System;

namespace DiGi.GIS.WebAPI.UI
{
    public static partial class Create
    {
        /// <summary>
        /// Creates the view range of a scene loaded for a circular area: the radius is both the maximum and the initial value of the slider, so moving it to the left reduces the range shown.
        /// </summary>
        /// <param name="radius">The radius of the loaded area, in metres. This value can be null.</param>
        /// <returns>A <see cref="Classes.ViewRange"/> with the minimum <see cref="Constants.Default.ViewRangeMinimum"/> (or the radius when it is smaller), or null if the radius is null, not finite or not positive.</returns>
        public static Classes.ViewRange? ViewRange(double? radius)
        {
            if (radius is null || !double.IsFinite(radius.Value) || radius.Value <= 0)
            {
                return null;
            }

            return new Classes.ViewRange(Math.Min(Constants.Default.ViewRangeMinimum, radius.Value), radius.Value, radius.Value);
        }
    }
}
