using System.Collections.Generic;

namespace DiGi.GIS.WebAPI.UI.Classes
{
    /// <summary>
    /// Represents a single antenna sent by the communication 3D view: its world location and the communication functions selected by the user (mapped to <see cref="Communication.Enums.Function"/>).
    /// </summary>
    public class AntennaParameter
    {
        /// <summary> Gets or sets the names of the selected <see cref="Communication.Enums.Function"/> values (e.g. Transmitter, Receiver). </summary>
        public List<string>? Functions { get; set; }

        /// <summary> Gets or sets the X coordinate of the antenna in world coordinates. </summary>
        public double X { get; set; }

        /// <summary> Gets or sets the Y coordinate of the antenna in world coordinates. </summary>
        public double Y { get; set; }

        /// <summary> Gets or sets the absolute elevation (Z coordinate in world coordinates) of the antenna top in meters. </summary>
        public double Z { get; set; }
    }
}