using DiGi.Core.IO.Table.Classes;
using System.Collections.Generic;

namespace DiGi.GIS.WebAPI.UI
{
    public static partial class Modify
    {
        /// <summary>
        /// Appends every row of one building data table to another, aligning the columns by name.
        /// <para>The two tables carry the same column set (the same projection), but a page's column order is not guaranteed to repeat - the upstream does not promise the columns arrive in the same positions on every page - so each appended row is re-aligned by column name rather than trusted to its position on the page. A cell the canonical table does not carry, or that holds no value, is not filed: the solver's <c>row[column.Index]</c> answers null for it, the same answer a range rule gives for a missing value.</para>
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
            Dictionary<int, int> index_Page_ByIndex_Into = [];
            foreach (Column column in page.Columns)
            {
                int index_Into = into.GetColumnIndex(column.Name);
                if (index_Into != -1)
                {
                    index_Page_ByIndex_Into[column.Index] = index_Into;
                }
            }

            foreach (Row row in page.Rows)
            {
                Dictionary<int, object?> values = [];
                foreach (KeyValuePair<int, int> keyValuePair in index_Page_ByIndex_Into)
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
