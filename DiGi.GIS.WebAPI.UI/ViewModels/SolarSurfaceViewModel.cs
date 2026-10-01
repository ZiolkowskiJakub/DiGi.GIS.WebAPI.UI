namespace DiGi.GIS.WebAPI.UI.ViewModels
{
    /// <summary>
    /// Represents the solar radiation result of one receiving surface (an external wall or roof) as the Building Viewer applies it to the scene in place: the node it recolours, the colour and the properties it shows.
    /// </summary>
    public class SolarSurfaceViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SolarSurfaceViewModel"/> class.
        /// </summary>
        /// <param name="reference">The reference of the Building Viewer node of the component, exactly as the scene carries it.</param>
        /// <param name="color">The irradiation colour of the surface as a <c>#rrggbb</c> string (<see cref="Query.SolarIrradiationColor(double)"/>).</param>
        /// <param name="properties">The <see cref="Classes.SurfaceSolarRadiationResult"/> of the surface as DiGi JSON, shown in the Properties panel in place of the component's own data.</param>
        public SolarSurfaceViewModel(string reference, string color, string? properties)
        {
            Reference = reference;
            Color = color;
            Properties = properties;
        }

        /// <summary> Gets the irradiation colour of the surface as a <c>#rrggbb</c> string. </summary>
        public string Color { get; }

        /// <summary> Gets the <see cref="Classes.SurfaceSolarRadiationResult"/> of the surface as DiGi JSON. </summary>
        public string? Properties { get; }

        /// <summary> Gets the reference of the Building Viewer node of the component, exactly as the scene carries it. </summary>
        public string Reference { get; }
    }
}
