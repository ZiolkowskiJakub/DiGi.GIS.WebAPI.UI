using System.Collections.Generic;

namespace DiGi.GIS.WebAPI.UI.ViewModels
{
    /// <summary>
    /// Represents the annual solar radiation of one building as the Building Viewer's "Solar radiation" panel applies it: one entry per receiving surface, recoloured in the existing scene, and the inputs the Results card reports.
    /// <para>Answered by <c>solar/viewbybuildingmodelid</c> and <c>solar/jobs/{jobId}/view</c>. It carries no geometry: the surfaces are matched to the scene by <see cref="SolarSurfaceViewModel.Reference"/>.</para>
    /// </summary>
    public class SolarRadiationViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SolarRadiationViewModel"/> class.
        /// </summary>
        /// <param name="radius">The neighbour radius the calculation used, in metres.</param>
        /// <param name="stationName">The name of the EPW weather station, or null when the file names none.</param>
        /// <param name="stationUrl">The application relative URL of the EPW file page of the station, or null.</param>
        /// <param name="surfaces">The results of the receiving surfaces.</param>
        public SolarRadiationViewModel(double radius, string? stationName, string? stationUrl, List<SolarSurfaceViewModel> surfaces)
        {
            Radius = radius;
            StationName = stationName;
            StationUrl = stationUrl;
            Surfaces = surfaces;
        }

        /// <summary> Gets the neighbour radius the calculation used, in metres. </summary>
        public double Radius { get; }

        /// <summary> Gets the name of the EPW weather station, or null when the file names none. </summary>
        public string? StationName { get; }

        /// <summary> Gets the application relative URL of the EPW file page of the station, or null. </summary>
        public string? StationUrl { get; }

        /// <summary> Gets the results of the receiving surfaces. </summary>
        public List<SolarSurfaceViewModel> Surfaces { get; }
    }
}
