using DiGi.Core.IO.Table.Classes;
using System.Text.Json;

namespace DiGi.GIS.WebAPI.UI
{
    public static partial class Create
    {
        /// <summary>
        /// Creates the building data table one page of <c>POST /gis/BuildingData/tablebybuildingdatabypagingparameter</c> answers, from its wire JSON.
        /// <para>The page is parsed with the <see cref="TableConverter{UTable, UColumn, URow}"/> the deployed GIS Web API's own <c>Create.Table</c> uses: the columns resolve through their <c>_type</c> discriminator into the solver's <see cref="Column"/> (they arrive as <c>ExtendedColumn</c>, a <see cref="Column"/>), and every cell is converted to the column's declared type. Parsing the table any other way - for instance into <c>DiGi.PostgreSQL.Table.Classes.Table</c>, whose <c>Column</c> is an unrelated class - deserializes the rows but silently drops every column.</para>
        /// </summary>
        /// <param name="json">The wire JSON of the page. This value can be null, in which case null is returned.</param>
        /// <returns>The page as the solver's table, or <see langword="null"/> when the input is null or not a table.</returns>
        public static Table? Table(string? json)
        {
            if (string.IsNullOrWhiteSpace(json))
            {
                return null;
            }

            JsonSerializerOptions jsonSerializerOptions = new();
            jsonSerializerOptions.Converters.Add(new TableConverter<Table, Column, Row>());

            return JsonSerializer.Deserialize<Table>(json, jsonSerializerOptions);
        }
    }
}
