using DiGi.Core.IO.Table.Classes;
using System.Text.Json;

namespace DiGi.GIS.WebAPI.UI
{
    public static partial class Create
    {
        // One options instance for every page, shared with TableAsync: System.Text.Json caches its type metadata per
        // options instance, so an instance built per call would rebuild that cache on every page of every solve.
        private static readonly JsonSerializerOptions jsonSerializerOptions_Table = new() { Converters = { new TableConverter<Table, Column, Row>() } };

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

            return JsonSerializer.Deserialize<Table>(json, jsonSerializerOptions_Table);
        }
    }
}
