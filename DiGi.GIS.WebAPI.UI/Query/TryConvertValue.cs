using DiGi.Core.Enums;
using System.Text.Json;

namespace DiGi.GIS.WebAPI.UI
{
    public static partial class Query
    {
        /// <summary>
        /// Attempts to convert a value of a Typology definition row to the CLR type of its column.
        /// <para>A value bound from a request body is a <see cref="JsonElement"/>, which <c>Core.Query.TryConvert(object, out object, DataType)</c> does not unwrap, so this dispatches to the <see cref="JsonElement"/> overload first. A JSON null and a CLR null both convert to <see langword="null"/>, the NULL bucket of a unique-value rule; a column whose <paramref name="dataType"/> is <see cref="DataType.Undefined"/> converts nothing.</para>
        /// </summary>
        /// <param name="value">The value to convert: a CLR primitive, a <see cref="JsonElement"/> or null.</param>
        /// <param name="dataType">The data type of the column the value belongs to.</param>
        /// <param name="result">When this method returns, the converted value, or null for a null input or a failed conversion.</param>
        /// <returns><see langword="true"/> when the value converts, or is null; otherwise <see langword="false"/>.</returns>
        public static bool TryConvertValue(object? value, DataType dataType, out object? result)
        {
            result = null;

            if (value is JsonElement jsonElement)
            {
                if (jsonElement.ValueKind == JsonValueKind.Null || jsonElement.ValueKind == JsonValueKind.Undefined)
                {
                    return true;
                }

                if (dataType == DataType.Undefined)
                {
                    return false;
                }

                return Core.Query.TryConvert(jsonElement, out result, Core.Query.Type(dataType)!);
            }

            if (value == null)
            {
                return true;
            }

            return Core.Query.TryConvert(value, out result, dataType);
        }
    }
}
