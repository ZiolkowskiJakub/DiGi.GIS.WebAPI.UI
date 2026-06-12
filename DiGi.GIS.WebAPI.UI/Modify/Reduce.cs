using DiGi.Geometry.Planar.Classes;
using System;
using System.Collections.Generic;

namespace DiGi.GIS.WebAPI.UI
{
    public static partial class Modify
    {
        /// <summary>
        /// Reduces the number of points in a list using a sampling factor, ensuring a minimum count is maintained.
        /// </summary>
        /// <param name="point2Ds">The list of <see cref="Point2D"/> objects to be reduced.</param>
        /// <param name="factor">The reduction factor used to determine the target number of points. A value between 0 and 1. If null or NaN, no reduction is performed.</param>
        /// <param name="minCount">The minimum number of points that should remain in the list after reduction. Defaults to 100.</param>
        public static void Reduce(this List<Point2D>? point2Ds, double? factor, int minCount = 100)
        {
            if (point2Ds is null || point2Ds.Count <= minCount)
            {
                return;
            }

            if (factor is null || factor is not double factor_Temp || double.IsNaN(factor_Temp))
            {
                return;
            }

            double factor_Clamp = Math.Clamp(factor_Temp, 0, 1.0);
            if (factor_Clamp >= 1.0)
            {
                return;
            }

            if (factor_Temp == 0)
            {
                point2Ds.Clear();
                return;
            }

            int originalCount = point2Ds.Count;
            int targetCount = (int)Math.Max(3, Math.Round(originalCount * factor_Clamp));
            if (targetCount <= minCount)
            {
                targetCount = minCount;
            }

            List<Point2D> point2Ds_Temp = [];

            for (int i = 0; i < targetCount; i++)
            {
                // Calculate the index in the original list using a linear mapping
                double mappingIndex = (double)i * (originalCount - 1) / (targetCount - 1);
                int indexToKeep = (int)Math.Round(mappingIndex);

                point2Ds_Temp.Add(point2Ds[indexToKeep]);
            }

            point2Ds.Clear();
            point2Ds.AddRange(point2Ds_Temp);
        }
    }
}