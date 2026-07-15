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

        /// <summary>
        /// Gets or sets the default electrical conductivity applied to the scattering object mesh cells [S/m].
        /// </summary>
        public double? Conductivity { get; set; }

        /// <summary>
        /// Gets or sets the frequencies of the propagating electromagnetic wave [MHz].
        /// <para>AI-NOTE (multi-frequency extensibility): the payload is a list so the calculation can be executed for multiple frequencies in one request. The 3D view currently sends a single frequency; once the per frequency toggling UI is implemented, the additional values flow through this property without any backend change.</para>
        /// </summary>
        public List<double>? Frequencies { get; set; }

        /// <summary>
        /// Gets or sets the polarization type of the propagating electromagnetic wave (Vertical or Horizontal).
        /// </summary>
        public string? Polarization { get; set; }

        /// <summary>
        /// Gets or sets the default relative electrical permittivity applied to the scattering object mesh cells [-].
        /// </summary>
        public double? RelativePermittivity { get; set; }

        /// <summary> Gets or sets the X coordinate of the center of the analyzed circular area. </summary>
        public double CenterX { get; set; }

        /// <summary> Gets or sets the Y coordinate of the center of the analyzed circular area. </summary>
        public double CenterY { get; set; }

        /// <summary> Gets or sets the radius of the analyzed circular area in meters. </summary>
        public double Radius { get; set; }

        /// <summary> Gets or sets the default simple multipath power delay profile name. </summary>
        public string? DefaultSimpleMultipathPowerDelayProfile { get; set; }

        /// <summary> Gets or sets the storey height in meters used for the building extrusions. </summary>
        public double? StoreyHeight { get; set; }
    }
}
