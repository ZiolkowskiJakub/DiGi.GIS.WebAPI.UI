using DiGi.Core.IO.Table.Classes;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace DiGi.GIS.WebAPI.UI
{
    public static partial class Query
    {
        /// <summary>
        /// Fetches every building data row for one county part from the GIS Web API, following the keyset cursor until the part is exhausted.
        /// <para>The upstream <c>tablebybuildingdatabypagingparameter</c> caps a single page at 10 000 rows, and a county part routinely holds more - part 76453 of code 2404 holds 100 543, part 16580 of code 0620 holds 93 672 - so one request would silently return only the first 10 000 buildings of such a part. The part is therefore read page by page: the page arrives ordered ascending by <c>Reference</c>, its last row is the next cursor, and the read ends when a page comes back short of the page size.</para>
        /// <para>Each page is parsed by <see cref="Create.Table(string)"/>, so the columns arrive as the solver's <see cref="Column"/> and every cell typed to the column's declared type - the table is already the type the Typology solver classifies, no bridge needed.</para>
        /// <para>Each page request retries once on a failure status: the known cause is a command timeout on a cold partition, and the retry succeeds because the partition is warm by then. A second failure is not retried - it is a genuine defect, not a cold start (Coding - Deployed WebAPI, section 4).</para>
        /// <para>A page's column order is not guaranteed to repeat, so successive pages are merged by column name, not by index (<see cref="Modify.Append(Table?, Table?)"/>).</para>
        /// </summary>
        /// <param name="httpClient">The HTTP client used for the request. This value can be null.</param>
        /// <param name="countyId">The county part identifier that scopes the building data.</param>
        /// <param name="columnUniqueIds">The column slugs to project: the definition's chain columns plus <c>reference</c>.</param>
        /// <param name="log">The callback that receives the part's progress: its page count and row count on success, its failure on the way out. This value can be null, in which case nothing is logged.</param>
        /// <param name="commandTimeout">The upstream command timeout in seconds. Defaults to 600.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by the caller to cancel the asynchronous operation.</param>
        /// <returns>The part's complete table, or <see langword="null"/> when a page fails even after its retry, when the part cannot be paged, or when the upstream is unreachable.</returns>
        public static async Task<Table?> BuildingDataTableAsync(this HttpClient? httpClient, int countyId, List<string> columnUniqueIds, Action<string>? log = null, int commandTimeout = 600, CancellationToken cancellationToken = default)
        {
            if (httpClient is null || columnUniqueIds is null || columnUniqueIds.Count == 0)
            {
                return null;
            }

            const int pageSize = 10000;
            string requestUri = $"{Constants.Default.BuildingDataTableUri}?commandtimeout={commandTimeout}";

            Table? result = null;
            string? lastReference = null;
            int pages = 0;

            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();

                Table? page = await RequestPageAsync(httpClient, requestUri, countyId, columnUniqueIds, pageSize, lastReference, cancellationToken);
                if (page is null)
                {
                    log?.Invoke($"Building data part {countyId}: the upstream answered nothing; the part is skipped.");
                    return null;
                }

                pages++;

                List<Row> rows = [.. page.Rows];
                if (rows.Count == 0)
                {
                    break;
                }

                result = Modify.Append(result, page);

                if (rows.Count < pageSize)
                {
                    break;
                }

                // Without the seek column the paging cannot advance. Stopping here would report part of the partition
                // as the whole of it, so the read is failed instead.
                int index_Reference = page.GetColumnIndex(Constants.BuildingData.ReferenceName);
                if (index_Reference == -1)
                {
                    log?.Invoke($"Building data part {countyId}: the page carries no 'Reference' column; the part cannot be paged and is skipped.");
                    return null;
                }

                // The page arrives ordered ascending by reference under the database's own collation, so its last row
                // is the cursor. A maximum computed here would be an ordinal one, which under any other collation names
                // a different row and silently steps over everything between the two.
                string? reference_Last = rows[^1][index_Reference]?.ToString();
                if (string.IsNullOrEmpty(reference_Last) || reference_Last == lastReference)
                {
                    log?.Invoke($"Building data part {countyId}: the cursor did not advance (last reference '{reference_Last}'); the part is skipped.");
                    return null;
                }

                lastReference = reference_Last;
            }

            if (result is null || result.RowCount == 0)
            {
                log?.Invoke($"Building data part {countyId}: no rows.");
                return null;
            }

            log?.Invoke($"Building data part {countyId}: {pages} page(s), {result.RowCount} row(s).");
            return result;
        }

        /// <summary>
        /// Requests one keyset page of building data and returns it parsed, retrying once on a failure status or transient exception.
        /// <para>The retry targets the known cold-partition timeout: the first attempt exceeds the upstream command timeout, the second succeeds because the partition is warm. A second failure is not retried - it is a genuine defect.</para>
        /// </summary>
        private static async Task<Table?> RequestPageAsync(HttpClient httpClient, string requestUri, int countyId, List<string> columnUniqueIds, int pageSize, string? cursor, CancellationToken cancellationToken)
        {
            for (int attempt = 0; attempt < 2; attempt++)
            {
                cancellationToken.ThrowIfCancellationRequested();

                try
                {
                    Dictionary<string, object?> body = new()
                    {
                        ["CountyId"] = countyId,
                        ["ColumnUniqueIds"] = columnUniqueIds,
                        ["PageSize"] = pageSize,
                        ["Cursor"] = cursor
                    };

                    using HttpRequestMessage request = new(HttpMethod.Post, requestUri);
                    request.Content = JsonContent.Create(body, options: JsonSerializerOptions.Default);

                    using HttpResponseMessage response = await httpClient.SendAsync(request, cancellationToken);

                    if (response.IsSuccessStatusCode)
                    {
                        string? responseJson = await response.Content.ReadAsStringAsync(cancellationToken);
                        return Create.Table(responseJson);
                    }
                }
                catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
                {
                    throw;
                }
                catch
                {
                    // Transient failure (connection reset, timeout): fall through to the retry.
                }

                if (attempt == 0)
                {
                    await Task.Delay(2000, cancellationToken);
                }
            }

            return null;
        }
    }
}
