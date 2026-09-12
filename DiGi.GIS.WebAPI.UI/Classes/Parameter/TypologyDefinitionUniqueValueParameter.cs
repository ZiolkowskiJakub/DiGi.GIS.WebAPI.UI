namespace DiGi.GIS.WebAPI.UI.Classes
{
    /// <summary>
    /// One unique-value row of a Typology definition level: a value of the column and the color of the bucket it maps to.
    /// </summary>
    public class TypologyDefinitionUniqueValueParameter
    {
        /// <summary>
        /// Gets or sets the value, a JSON primitive or null for the NULL bucket. Bound from a request body it arrives as a <see cref="System.Text.Json.JsonElement"/>; read it through <see cref="Query.TryConvertValue(object, Core.Enums.DataType, out object)"/>, never by pattern matching on a CLR primitive.
        /// </summary>
        public object? Value { get; set; }

        /// <summary>
        /// Gets or sets the bucket color as the color picker holds it, <c>#rrggbb</c>.
        /// </summary>
        public string? Color { get; set; }
    }
}
