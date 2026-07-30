namespace DiGi.GIS.WebAPI.UI.ViewModels
{
    /// <summary>
    /// Represents a view model for the communication 3D scene view: the buildings of the analyzed circular area (streamed as a binary glTF payload) plus the input parameters required to send the communication calculation request back to the server.
    /// </summary>
    public class CommunicationSceneViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CommunicationSceneViewModel"/> class.
        /// </summary>
        /// <param name="title">The title displayed above the viewer.</param>
        /// <param name="gLBUrl">The application relative URL of the binary glTF (.glb) endpoint.</param>
        /// <param name="centerX">The X coordinate of the center of the analyzed circular area.</param>
        /// <param name="centerY">The Y coordinate of the center of the analyzed circular area.</param>
        /// <param name="radius">The radius of the analyzed circular area in meters.</param>
        public CommunicationSceneViewModel(string? title, string? gLBUrl, double centerX, double centerY, double radius)
        {
            Title = title;
            GLBUrl = gLBUrl;
            CenterX = centerX;
            CenterY = centerY;
            Radius = radius;
        }

        /// <summary> Gets the X coordinate of the center of the analyzed circular area. </summary>
        public double CenterX { get; }

        /// <summary> Gets the Y coordinate of the center of the analyzed circular area. </summary>
        public double CenterY { get; }

        /// <summary> Gets the application relative URL of the binary glTF (.glb) endpoint. </summary>
        public string? GLBUrl { get; }

        /// <summary> Gets the radius of the analyzed circular area in meters. </summary>
        public double Radius { get; }

        /// <summary> Gets the title displayed above the viewer. </summary>
        public string? Title { get; }
    }
}