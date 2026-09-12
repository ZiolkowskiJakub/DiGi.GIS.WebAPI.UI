using DiGi.PostgreSQL.Table.Classes;
using System.Collections.Generic;

namespace DiGi.GIS.WebAPI.UI
{
    public static partial class Query
    {
        /// <summary>
        /// Indexes the building data columns by their unique identifier, the key a Typology definition addresses a column by.
        /// <para>A column without a unique identifier is skipped; when two columns share one the first is kept, as the deployed catalog never lists a duplicate (194 columns, 194 distinct identifiers on 2026-09-12) and a later duplicate would otherwise throw on insertion.</para>
        /// </summary>
        /// <param name="columns">The columns. This value can be null.</param>
        /// <returns>The columns by unique identifier, or <see langword="null"/> when there are none.</returns>
        public static Dictionary<string, Column>? ColumnDictionary(IEnumerable<Column>? columns)
        {
            if (columns == null)
            {
                return null;
            }

            Dictionary<string, Column> result = [];
            foreach (Column column in columns)
            {
                if (column?.UniqueId != null && !result.ContainsKey(column.UniqueId))
                {
                    result[column.UniqueId] = column;
                }
            }

            return result.Count == 0 ? null : result;
        }
    }
}
