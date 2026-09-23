using DiGi.Core.IO.Table.Classes;
using System.Collections.Generic;

namespace DiGi.GIS.WebAPI.UI
{
    public static partial class Modify
    {
        /// <summary>
        /// Appends every row of one building data table to another, aligning the columns by name.
        /// <para>The two tables carry the same column set (the same projection), but a page's column order is not guaranteed to repeat - the upstream does not promise the columns arrive in the same positions on every page - so each appended row is re-aligned by column name rather than trusted to its position on the page. A cell the canonical table does not carry, or that holds no value, is not filed: the solver's <c>row[column.Index]</c> answers null for it, the same answer a range rule gives for a missing value.</para>
        /// <para>The page is read through its live enumeration, never through <c>Rows</c>, which clones every row it hands out; the merge is the one reader of a page, and the page is discarded once merged. The rows are filed by value rather than handed over: <c>AddRow(Row)</c> keeps a row's own index when the table has room for it, so a page row, indexed from 0, would overwrite the canonical table's row of the same index instead of being appended.</para>
        /// <para>When <paramref name="into"/> is <see langword="null"/> the <paramref name="page"/> is returned as the new table - it is the caller's own page, freshly parsed, so nothing needs cloning.</para>
        /// </summary>
        /// <param name="into">The table the rows are appended to. This value can be null, in which case the page is returned as the new table.</param>
        /// <param name="page">The table whose rows are appended. This value can be null, in which case <paramref name="into"/> is returned unchanged.</param>
        /// <returns>The table carrying the appended rows, or <paramref name="into"/> when <paramref name="page"/> is null or carries no rows.</returns>
        public static Table? Append(this Table? into, Table? page)
        {
            if (into is null)
            {
                return page;
            }

            if (page is null || page.RowCount == 0)
            {
                return into;
            }

            // Map each of the page's columns, by name, to the position it occupies in the canonical (into) order.
            Dictionary<int, int> index_Into_ByIndex_Page = [];
            foreach (Column column in page.Columns)
            {
                int index_Into = into.GetColumnIndex(column.Name);
                if (index_Into != -1)
                {
                    index_Into_ByIndex_Page[column.Index] = index_Into;
                }
            }

            foreach (Row row in page)
            {
                Dictionary<int, object?> values = [];
                foreach (KeyValuePair<int, int> keyValuePair in index_Into_ByIndex_Page)
                {
                    object? value = row[keyValuePair.Key];
                    if (value is null)
                    {
                        continue;
                    }

                    values[keyValuePair.Value] = value;
                }

                into.AddRow(values);
            }

            return into;
        }

        /// <summary>
        /// Appends the rows of a page to a table, merging the columns by name, and skips every row whose key is already in <paramref name="keys"/>.
        /// <para>A physical-order read of a county part can hand the same building over twice: a row rewritten while the part is paged moves to a new heap position, and a new position ahead of the walk is read again (DiGi.GIS.WebAPI#40). Within one part the building's key is its reference, so the caller passes the part's reference column and one set per part; the first copy read is kept. A row without a key value is always appended - nothing can say it repeats.</para>
        /// <para>The set is filled as rows are appended, so a key repeated inside one page is dropped as well. With no key column, or a page that lacks it, this is <see cref="Append(Table?, Table?)"/>.</para>
        /// </summary>
        /// <param name="into">The table to append to. This value can be null, in which case a table with the page's columns is started.</param>
        /// <param name="page">The page whose rows are appended. This value can be null.</param>
        /// <param name="keyColumnName">The name of the column whose value identifies a row. This value can be null.</param>
        /// <param name="keys">The keys already appended; updated with every key appended. This value can be null.</param>
        /// <returns>The table the rows were appended to, or <paramref name="into"/> when the page holds no rows.</returns>
        public static Table? Append(this Table? into, Table? page, string? keyColumnName, HashSet<string>? keys)
        {
            if (page is null || page.RowCount == 0)
            {
                return into;
            }

            int index_Key = string.IsNullOrEmpty(keyColumnName) || keys is null ? -1 : page.GetColumnIndex(keyColumnName);
            if (index_Key == -1)
            {
                return Append(into, page);
            }

            into ??= new Table(page.Columns);

            // Map each of the page's columns, by name, to the position it occupies in the canonical (into) order.
            Dictionary<int, int> index_Into_ByIndex_Page = [];
            foreach (Column column in page.Columns)
            {
                int index_Into = into.GetColumnIndex(column.Name);
                if (index_Into != -1)
                {
                    index_Into_ByIndex_Page[column.Index] = index_Into;
                }
            }

            foreach (Row row in page)
            {
                if (row[index_Key]?.ToString() is string key && !keys!.Add(key))
                {
                    continue;
                }

                Dictionary<int, object?> values = [];
                foreach (KeyValuePair<int, int> keyValuePair in index_Into_ByIndex_Page)
                {
                    object? value = row[keyValuePair.Key];
                    if (value is null)
                    {
                        continue;
                    }

                    values[keyValuePair.Value] = value;
                }

                into.AddRow(values);
            }

            return into;
        }
    }
}
