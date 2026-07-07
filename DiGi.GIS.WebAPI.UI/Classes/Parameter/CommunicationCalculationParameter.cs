using System.Collections.Generic;

namespace DiGi.GIS.WebAPI.UI.Classes
{
    /// <summary>
    /// Represents the payload of a communication calculation request sent by the communication 3D view: the analyzed circular area (used to fetch the buildings on the fly) and the antennas placed by the user.
    /// </summary>
    public class CommunicationCalculationParameter
    {
        /// <summary> Gets or sets the antennas placed by the user in the 3D view. </summary>
        public List<AntennaParameter>? Antennas { get; set; }

        /// <summary> Gets or sets the X coordinate of the center of the analyzed circular area. </summary>
        public double CenterX { get; set; }

        /// <summary> Gets or sets the Y coordinate of the center of the analyzed circular area. </summary>
        public double CenterY { get; set; }

        /// <summary> Gets or sets the radius of the analyzed circular area in meters. </summary>
        public double Radius { get; set; }

        /// <summary> Gets or sets the storey height in meters used for the building extrusions. </summary>
        public double? StoreyHeight { get; set; }
    }
}
