using DiGi.Core.IO.Table.Classes;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace DiGi.GIS.WebAPI.UI
{
    public static partial class Create
    {
        /// <summary>
        /// Asynchronously creates the building data table one page of <c>POST /gis/BuildingData/tablebybuildingdatabypagingparameter</c> answers, read straight from the response stream.
        /// <para>The variant the page fetch uses: a 10 000-row page is 600 KB of JSON, and reading it into a string first only doubles the bytes held before the same converter parses them. Same converter, same options and same result as <see cref="Table(string)"/>.</para>
        /// </summary>
        /// <param name="stream">The response stream of the page. This value can be null, in which case null is returned.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by the caller to cancel the asynchronous operation.</param>
        /// <returns>The page as the solver's table, or <see langword="null"/> when the input is null or not a table.</returns>
        public static async Task<Table?> TableAsync(Stream? stream, CancellationToken cancellationToken = default)
        {
            if (stream is null)
            {
                return null;
            }

            return await JsonSerializer.DeserializeAsync<Table>(stream, jsonSerializerOptions_Table, cancellationToken);
        }
    }
}
