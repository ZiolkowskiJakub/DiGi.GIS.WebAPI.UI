using DiGi.PostgreSQL.Table.Classes;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace DiGi.GIS.WebAPI.UI
{
    public static partial class Query
    {
        /// <summary>
        /// Asynchronously reads the columns of the building data table from the GIS Web API.
        /// <para>The catalog is read live on every call: it is the reference every Typology definition is resolved against, and a definition must never be validated against a column list older than the one the page offered.</para>
        /// </summary>
        /// <param name="httpClient">The HTTP client used for the request. This value can be null.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by the caller to cancel the asynchronous operation.</param>
        /// <returns>The columns, or <see langword="null"/> when the GIS Web API answers nothing or cannot be reached.</returns>
        public static async Task<List<Column>?> BuildingDataColumnsAsync(this HttpClient? httpClient, CancellationToken cancellationToken = default)
        {
            List<Column>? columns = await httpClient.ItemsAsync<Column>(Constants.Default.BuildingDataColumnsUri, cancellationToken);
            if (columns == null || columns.Count == 0)
            {
                return null;
            }

            return columns;
        }
    }
}
