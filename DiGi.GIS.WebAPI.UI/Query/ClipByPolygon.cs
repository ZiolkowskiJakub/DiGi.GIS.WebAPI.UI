using DiGi.Core.IO.Table.Classes;
using DiGi.Geometry.Planar.Classes;

namespace DiGi.GIS.WebAPI.UI
{
    public static partial class Query
    {
        /// <summary>
        /// Returns a new building data table carrying only the rows whose internal point falls inside the given boundary polygon, leaving the input table untouched.
        /// <para>The row's <c>Internal Point X</c>/<c>Internal Point Y</c> are read by their column names, so the test holds whatever order the columns arrived in, and a row missing either coordinate is dropped: a building that cannot be located cannot be filed inside the area. A table that carries neither column is returned unchanged instead: dropping every such row would report an area as empty for a gap in the projection rather than in the data.</para>
        /// </summary>
        /// <param name="table">The building data table to clip. This value can be null, in which case it is returned unchanged.</param>
        /// <param name="polygonalFace2D">The area boundary the rows are kept inside. This value can be null, in which case the table is returned unchanged.</param>
        /// <param name="tolerance">The distance tolerance applied by the point-in-polygon test.</param>
        /// <returns>A new table carrying only the rows inside the polygon, or the input unchanged when it is null or carries no internal-point columns.</returns>
        public static Table? ClipByPolygon(this Table? table, PolygonalFace2D? polygonalFace2D, double tolerance = DiGi.Core.Constants.Tolerance.Distance)
        {
            if (table is null || polygonalFace2D is null)
            {
                return table;
            }

            int index_X = table.GetColumnIndex(Constants.BuildingData.InternalPointXName);
            int index_Y = table.GetColumnIndex(Constants.BuildingData.InternalPointYName);
            if (index_X == -1 || index_Y == -1)
            {
                return table;
            }

            // A new table rather than an in-place edit: the caller's table must not change under it, and the no-columns path already returns the input as it came.
            Table result = new(table.Columns);
            foreach (Row row in table.Rows)
            {
                if (row[index_X] is double x && row[index_Y] is double y && polygonalFace2D.Inside(new Point2D(x, y), tolerance))
                {
                    result.AddRow(row);
                }
            }

            return result;
        }
    }
}
