using System.Globalization;

namespace DiGi.GIS.WebAPI.UI
{
    public static partial class Create
    {
        /// <summary>
        /// Creates the settings of the Building Viewer's "Solar radiation" panel for a building known by identifier: the query of the solar routes with the neighbour radius clamped by <see cref="Query.SolarRadius(double?)"/>.
        /// </summary>
        /// <param name="id">The unique identifier of the building.</param>
        /// <param name="countyId">The optional unique identifier of the county associated with the building.</param>
        /// <param name="radius">The scene radius of the page, in metres; null means <see cref="Constants.Default.TerrainRadius"/>.</param>
        /// <returns>The settings.</returns>
        public static ViewModels.SolarSettingsViewModel SolarSettingsViewModel(long id, int? countyId, double? radius)
        {
            double radius_Requested = radius ?? Constants.Default.TerrainRadius;
            double radius_Solar = Query.SolarRadius(radius);

            string query = $"id={id.ToString(CultureInfo.InvariantCulture)}";
            if (countyId.HasValue)
            {
                query += $"&countyid={countyId.Value.ToString(CultureInfo.InvariantCulture)}";
            }

            query += $"&radius={radius_Solar.ToString("R", CultureInfo.InvariantCulture)}";

            return new ViewModels.SolarSettingsViewModel(query, radius_Solar, radius_Requested);
        }
    }
}
