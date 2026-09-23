using DiGi.Core.IO.Table.Classes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
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
        /// <para>The upstream <c>tablebybuildingdatabypagingparameter</c> caps a single page at 10 000 rows, and a county part routinely holds more - part 76453 of code 2404 holds 100 543, part 16580 of code 0620 holds 93 672 - so one request would silently return only the first 10 000 buildings of such a part. The part is therefore read page by page.</para>
        /// <para>The pages are asked for in <b>physical order</b> (<c>PhysicalOrder = true</c>, DiGi.GIS.WebAPI#40): the part is read sequentially from the heap instead of one random heap read per row in reference order, which is what bounded the solve - 368-654 s for a cold 155 307-row part, all of it spent in the database (DiGi.GIS.WebAPI.UI#29). The next page's cursor arrives in the <see cref="Constants.Default.NextCursorHeaderName"/> response header, and a response without it ends the part. A building rewritten while its part is paged can come back twice, so rows are deduplicated on <c>Reference</c> within the part (<see cref="Modify.Append(Table?, Table?, string?, HashSet{string}?)"/>).</para>
        /// <para>Each page is parsed by <see cref="Create.TableAsync(Stream, CancellationToken)"/> straight from the response stream, so the columns arrive as the solver's <see cref="Column"/> and every cell typed to the column's declared type - the table is already the type the Typology solver classifies, no bridge needed. The pages are read through their live enumeration, never through <c>Rows</c>, which clones every row it hands out.</para>
        /// <para>An upstream 404 is the part holding no building data partition, not a failure: it answers an empty table at once, so a county with nothing stored reads as "no buildings" rather than "service unavailable". Every other failure status, and a transient exception, is retried once: the known cause is a command timeout on a cold partition, and the retry succeeds because the partition is warm by then. A second failure is not retried - it is a genuine defect, not a cold start (Coding - Deployed WebAPI, section 4).</para>
        /// <para>The read stops early once the rows fetched so far exceed <paramref name="maxRowCount"/>: the caller refuses such a part anyway, so the remaining pages would only cost time. The table returned then carries more rows than the ceiling, which is how the caller tells.</para>
        /// <para>A page's column order is not guaranteed to repeat, so successive pages are merged by column name, not by index.</para>
        /// </summary>
        /// <param name="httpClient">The HTTP client used for the request. This value can be null.</param>
        /// <param name="countyId">The county part identifier that scopes the building data.</param>
        /// <param name="columnUniqueIds">The column slugs to project: the definition's chain columns plus <c>reference</c>.</param>
        /// <param name="log">The callback that receives the part's progress: its page count and row count on success, its failure on the way out. This value can be null, in which case nothing is logged.</param>
        /// <param name="maxRowCount">The number of rows above which the read stops early. Defaults to no limit.</param>
        /// <param name="commandTimeout">The upstream command timeout in seconds. Defaults to 600.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by the caller to cancel the asynchronous operation.</param>
        /// <returns>The part's complete table (empty when the part holds no building data), or <see langword="null"/> when a page fails even after its retry, when the part cannot be paged, or when the upstream is unreachable.</returns>
        public static async Task<Table?> BuildingDataTableAsync(this HttpClient? httpClient, int countyId, List<string> columnUniqueIds, Action<string>? log = null, int maxRowCount = int.MaxValue, int commandTimeout = 600, CancellationToken cancellationToken = default)
        {
            if (httpClient is null || columnUniqueIds is null || columnUniqueIds.Count == 0)
            {
                return null;
            }

            int pageSize = Constants.Default.BuildingDataPageSize;
            string requestUri = $"{Constants.Default.BuildingDataTableUri}?commandtimeout={commandTimeout}";

            Table? result = null;
            string? cursor = null;
            int pages = 0;

            // A county part's rows share one county_id, so the reference alone identifies a building within it.
            HashSet<string> references = [];

            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();

                (Table? page, string? cursor_Next) = await PageAsync(cursor);
                if (page is null)
                {
                    log?.Invoke($"Building data part {countyId}: the upstream answered nothing; the part is skipped.");
                    return null;
                }

                pages++;

                int rowCount = page.RowCount;
                if (rowCount == 0)
                {
                    result ??= page;
                    break;
                }

                result = Modify.Append(result, page, Constants.BuildingData.ReferenceName, references);

                if (result!.RowCount > maxRowCount)
                {
                    log?.Invoke($"Building data part {countyId}: {result.RowCount} row(s) after {pages} page(s) exceed the ceiling of {maxRowCount}; the read is stopped.");
                    return result;
                }

                // Physical order: the upstream names the next page's position. A position that does not move would
                // re-read the same page for ever, so the read is failed instead.
                if (cursor_Next is not null)
                {
                    if (cursor_Next == cursor)
                    {
                        log?.Invoke($"Building data part {countyId}: the physical cursor did not advance ('{cursor_Next}'); the part is skipped.");
                        return null;
                    }

                    cursor = cursor_Next;
                    continue;
                }

                // No DiGi-Next-Cursor header: the physical-order read has reached the end of the part.
                break;
            }

            log?.Invoke($"Building data part {countyId}: {pages} page(s), {result!.RowCount} row(s), physical order.");
            return result;

            // One page, parsed, with the next physical cursor when the upstream sent one; an empty table for a part with
            // no partition; a null table after a failed retry.
            async Task<(Table? Page, string? Cursor)> PageAsync(string? cursor_Page)
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
                            ["Cursor"] = cursor_Page,
                            ["PhysicalOrder"] = true
                        };

                        using HttpRequestMessage request = new(HttpMethod.Post, requestUri);
                        request.Content = JsonContent.Create(body, options: JsonSerializerOptions.Default);

                        using HttpResponseMessage response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);

                        if (response.StatusCode == HttpStatusCode.NotFound)
                        {
                            return (new Table(), null);
                        }

                        if (response.IsSuccessStatusCode)
                        {
                            string? cursor_Next = response.Headers.TryGetValues(Constants.Default.NextCursorHeaderName, out IEnumerable<string>? values) ? values.FirstOrDefault() : null;

                            using Stream stream = await response.Content.ReadAsStreamAsync(cancellationToken);
                            return (await Create.TableAsync(stream, cancellationToken), string.IsNullOrEmpty(cursor_Next) ? null : cursor_Next);
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

                return (null, null);
            }
        }

        /// <summary>
        /// Fetches the building data rows of every county part of an area into one table, one part after another.
        /// <para>The parts are read sequentially - the deployed host is queried one request at a time, never fanned out (Coding - Deployed WebAPI, section 4); reading parts at once was measured not to help, because the cost is the database reading its heap and parallel reads only queue more of it on the same disk (DiGi.GIS.WebAPI.UI#29) - and merged by column name through <see cref="Modify.Append(Table?, Table?)"/>. A part listed twice is read once. One part failing does not fail the area: the rest are collected and the failure is logged; every part failing is the upstream answering nothing, which the caller reads as unavailable. A part holding no building data contributes nothing and still counts as read, so an area whose parts are all empty answers an empty table rather than <see langword="null"/>.</para>
        /// <para>The read stops as soon as the merged rows exceed <paramref name="maxRowCount"/> - the returned table then carries more rows than the ceiling and the caller refuses it - so an area far above the ceiling costs one part and a page, not the whole area.</para>
        /// </summary>
        /// <param name="httpClient">The HTTP client used for the request. This value can be null.</param>
        /// <param name="countyIds">The county part identifiers that scope the building data. This value can be null.</param>
        /// <param name="columnUniqueIds">The column slugs to project: the definition's chain columns plus <c>reference</c>.</param>
        /// <param name="log">The callback that receives each part's progress and the area's summary. This value can be null, in which case nothing is logged.</param>
        /// <param name="maxRowCount">The number of merged rows above which the read stops early. Defaults to no limit.</param>
        /// <param name="commandTimeout">The upstream command timeout in seconds. Defaults to 600.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by the caller to cancel the asynchronous operation.</param>
        /// <returns>The merged table (empty when every part read holds nothing), or <see langword="null"/> when there are no parts or none could be read.</returns>
        public static async Task<Table?> BuildingDataTableAsync(this HttpClient? httpClient, IEnumerable<int>? countyIds, List<string> columnUniqueIds, Action<string>? log = null, int maxRowCount = int.MaxValue, int commandTimeout = 600, CancellationToken cancellationToken = default)
        {
            if (httpClient is null || countyIds is null || columnUniqueIds is null || columnUniqueIds.Count == 0)
            {
                return null;
            }

            Table? result = null;
            int parts = 0;
            int parts_Read = 0;
            HashSet<int> countyIds_Read = [];

            foreach (int countyId in countyIds)
            {
                if (!countyIds_Read.Add(countyId))
                {
                    continue;
                }

                parts++;
                cancellationToken.ThrowIfCancellationRequested();

                Table? table_Part = await httpClient.BuildingDataTableAsync(countyId, columnUniqueIds, log, maxRowCount, commandTimeout, cancellationToken);
                if (table_Part is null)
                {
                    continue;
                }

                parts_Read++;
                if (table_Part.RowCount == 0)
                {
                    continue;
                }

                result = Modify.Append(result, table_Part);

                if (result!.RowCount > maxRowCount)
                {
                    log?.Invoke($"Building data: {result.RowCount} row(s) after {parts} part(s) exceed the ceiling of {maxRowCount}; the read is stopped.");
                    return result;
                }
            }

            if (parts_Read == 0)
            {
                log?.Invoke($"Building data: none of the {parts} part(s) could be read.");
                return null;
            }

            result ??= new Table();
            log?.Invoke($"Building data: {parts_Read} of {parts} part(s) read, {result.RowCount} row(s).");
            return result;
        }
    }
}
