using DiGi.Core.IO.Table.Classes;
using DiGi.Geometry.Planar.Classes;
using NetTopologySuite.Geometries;
using NetTopologySuite.Geometries.Prepared;

namespace DiGi.GIS.WebAPI.UI
{
    public static partial class Query
    {
        /// <summary>
        /// Returns a new building data table carrying only the rows whose internal point falls inside the given boundary polygon, leaving the input table untouched.
        /// <para>The row's <c>Internal Point X</c>/<c>Internal Point Y</c> are read by their column names, so the test holds whatever order the columns arrived in, and a row missing either coordinate is dropped: a building that cannot be located cannot be filed inside the area. A table that carries neither column is returned unchanged instead: dropping every such row would report an area as empty for a gap in the projection rather than in the data.</para>
        /// <para>The county's rows are many and the boundary's vertices are many, so the test is not the polygon's own <c>Inside</c> - that copies the vertex list and ray-casts every edge on every call. Each point is first compared with the boundary's bounding box, which settles most of a county in a few comparisons, and the rest are answered by a prepared NetTopologySuite geometry, whose edge index makes the containment test logarithmic in the vertex count. A point exactly on the boundary is not contained, where <c>Inside</c> would refuse it within its tolerance instead - the same answer for every building that is not sitting on the line.</para>
        /// </summary>
        /// <param name="table">The building data table to clip. This value can be null, in which case it is returned unchanged.</param>
        /// <param name="polygonalFace2D">The area boundary the rows are kept inside. This value can be null, in which case the table is returned unchanged.</param>
        /// <returns>A new table carrying only the rows inside the polygon, or the input unchanged when it is null or carries no internal-point columns.</returns>
        public static Table? ClipByPolygon(this Table? table, PolygonalFace2D? polygonalFace2D)
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

            Polygon? polygon = DiGi.Geometry.Planar.Convert.ToNTS(polygonalFace2D);
            BoundingBox2D? boundingBox2D = polygonalFace2D.GetBoundingBox();
            if (polygon is null || boundingBox2D is null)
            {
                return table;
            }

            Point2D min = boundingBox2D.Min;
            Point2D max = boundingBox2D.Max;
            IPreparedGeometry preparedGeometry = PreparedGeometryFactory.Prepare(polygon);
            GeometryFactory geometryFactory = polygon.Factory;

            // A new table rather than an in-place edit: the caller's table must not change under it, and the no-columns path already returns the input as it came.
            Table result = new(table.Columns);
            foreach (Row row in table)
            {
                if (row[index_X] is not double x || row[index_Y] is not double y)
                {
                    continue;
                }

                if (x < min.X || x > max.X || y < min.Y || y > max.Y)
                {
                    continue;
                }

                if (preparedGeometry.Contains(geometryFactory.CreatePoint(new Coordinate(x, y))))
                {
                    result.AddRow(row, tryConvert: false);
                }
            }

            return result;
        }
    }
}
