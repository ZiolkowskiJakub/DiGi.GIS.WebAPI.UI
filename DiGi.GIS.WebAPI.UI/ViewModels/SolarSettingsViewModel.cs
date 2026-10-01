namespace DiGi.GIS.WebAPI.UI.ViewModels
{
    /// <summary>
    /// Represents what the Building Viewer's "Solar radiation" panel needs to calculate the building of the page: the query of the solar routes and the neighbour radius, both the one used and the one the page asked for.
    /// <para>A page renders the panel only when it carries these settings, i.e. when its building is known by identifier (DiGi.GIS.WebAPI.UI#66).</para>
    /// </summary>
    public class SolarSettingsViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SolarSettingsViewModel"/> class.
        /// </summary>
        /// <param name="query">The query string of the solar routes (<c>id</c>, <c>countyid</c> and <c>radius</c>, invariant culture, without the leading <c>?</c>).</param>
        /// <param name="radius">The neighbour radius the calculation uses, in metres (<see cref="Query.SolarRadius(double?)"/>).</param>
        /// <param name="radiusRequested">The scene radius of the page, in metres, from which <paramref name="radius"/> was clamped.</param>
        public SolarSettingsViewModel(string query, double radius, double radiusRequested)
        {
            Query = query;
            Radius = radius;
            RadiusRequested = radiusRequested;
        }

        /// <summary> Gets the query string of the solar routes, without the leading <c>?</c>. </summary>
        public string Query { get; }

        /// <summary> Gets the neighbour radius the calculation uses, in metres. </summary>
        public double Radius { get; }

        /// <summary> Gets the scene radius of the page, in metres, from which <see cref="Radius"/> was clamped. </summary>
        public double RadiusRequested { get; }
    }
}
