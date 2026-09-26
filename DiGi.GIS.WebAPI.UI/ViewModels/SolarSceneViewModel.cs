namespace DiGi.GIS.WebAPI.UI.ViewModels
{
    /// <summary>
    /// Represents a view model for the solar radiation 3D scene view: the building and its neighbours, streamed as a binary glTF payload with the receiving surfaces coloured by their annual irradiation, plus what the legend states about the calculation.
    /// </summary>
    public class SolarSceneViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SolarSceneViewModel"/> class.
        /// </summary>
        /// <param name="title">The title displayed above the viewer.</param>
        /// <param name="gLBUrl">The application relative URL of the binary glTF (.glb) endpoint.</param>
        /// <param name="jobsUrl">The application relative URL of the background job routes (<c>solar/jobs</c>), offered when the scene request is refused as too large.</param>
        /// <param name="jobQuery">The query string (<c>id</c>, <c>countyid</c>, <c>radius</c>) a background job is posted with.</param>
        /// <param name="radius">The neighbour radius in metres, measured from the edge of the footprint.</param>
        /// <param name="stationName">The name of the EPW weather station, or null when the file does not state it.</param>
        /// <param name="stationUrl">The application relative URL of the EPW weather file page of the station.</param>
        public SolarSceneViewModel(string? title, string? gLBUrl, string? jobsUrl, string? jobQuery, double radius, string? stationName, string? stationUrl)
        {
            Title = title;
            GLBUrl = gLBUrl;
            JobsUrl = jobsUrl;
            JobQuery = jobQuery;
            Radius = radius;
            StationName = stationName;
            StationUrl = stationUrl;
        }

        /// <summary> Gets the application relative URL of the binary glTF (.glb) endpoint. </summary>
        public string? GLBUrl { get; }

        /// <summary> Gets the query string (<c>id</c>, <c>countyid</c>, <c>radius</c>) a background job is posted with. </summary>
        public string? JobQuery { get; }

        /// <summary> Gets the application relative URL of the background job routes (<c>solar/jobs</c>), offered when the scene request is refused as too large. </summary>
        public string? JobsUrl { get; }

        /// <summary> Gets the neighbour radius in metres, measured from the edge of the footprint. </summary>
        public double Radius { get; }

        /// <summary> Gets the name of the EPW weather station, or null when the file does not state it. </summary>
        public string? StationName { get; }

        /// <summary> Gets the application relative URL of the EPW weather file page of the station. </summary>
        public string? StationUrl { get; }

        /// <summary> Gets the title displayed above the viewer. </summary>
        public string? Title { get; }
    }
}
