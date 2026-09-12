namespace DiGi.GIS.WebAPI.UI.Classes
{
    /// <summary>
    /// One range row of a Typology definition level: a closed interval and the color of the bucket it maps to.
    /// </summary>
    public class TypologyDefinitionRangeParameter
    {
        /// <summary>
        /// Gets or sets the inclusive lower bound. Null when the row is still being typed.
        /// </summary>
        public double? Min { get; set; }

        /// <summary>
        /// Gets or sets the inclusive upper bound. Null when the row is still being typed.
        /// </summary>
        public double? Max { get; set; }

        /// <summary>
        /// Gets or sets the bucket color as the color picker holds it, <c>#rrggbb</c>.
        /// </summary>
        public string? Color { get; set; }
    }
}
