using DiGi.Analytical.Building.Classes;
using DiGi.Geometry.Planar.Classes;
using DiGi.Geometry.Spatial.Classes;
using System.Collections.Generic;

namespace DiGi.GIS.WebAPI.UI
{
    public static partial class Query
    {
        /// <summary>
        /// Calculates the dynamic terrain coverage circle encompassing the specified <see cref="BuildingModel"/> with appropriate margin padding.
        /// </summary>
        /// <param name="buildingModel">The building model to calculate terrain coverage for. This value can be null.</param>
        /// <param name="padding">The margin in meters extending beyond the building bounding envelope.</param>
        /// <param name="minimumRadius">The minimum allowable terrain radius in meters.</param>
        /// <returns>A <see cref="Circle2D"/> centered on the building bounding centroid with expanded radius, or <see langword="null"/> if the building model or its bounds are invalid.</returns>
        public static Circle2D? TerrainCircle(this BuildingModel? buildingModel, double padding = Constants.Default.TerrainPadding, double minimumRadius = Constants.Default.TerrainRadius)
        {
            if (buildingModel is null)
            {
                return null;
            }

            BoundingBox3D? boundingBox3D = buildingModel.GetBoundingBox();
            if (boundingBox3D is null)
            {
                return null;
            }

            Point3D? centroid = boundingBox3D.GetCentroid();
            if (centroid is null || !double.IsFinite(centroid.X) || !double.IsFinite(centroid.Y))
            {
                return null;
            }

            Point2D center = new(centroid.X, centroid.Y);
            double halfWidth = boundingBox3D.Width / 2;
            double halfDepth = boundingBox3D.Depth / 2;
            double boundingRadius = System.Math.Sqrt((halfWidth * halfWidth) + (halfDepth * halfDepth));
            double radius = System.Math.Max(boundingRadius + padding, minimumRadius);

            return new Circle2D(center, radius);
        }

        /// <summary>
        /// Calculates the dynamic terrain coverage circle encompassing a collection of <see cref="BuildingModel"/> instances, optionally anchoring to a search circle.
        /// </summary>
        /// <param name="buildingModels">The collection of building models to calculate terrain coverage for. This value can be null.</param>
        /// <param name="searchCircle">The original search area circle. When provided, its center is used as the terrain circle anchor. This value can be null.</param>
        /// <param name="padding">The margin in meters extending beyond the furthest building corner.</param>
        /// <param name="minimumRadius">The minimum allowable terrain radius in meters.</param>
        /// <returns>A <see cref="Circle2D"/> encompassing all building footprints and search area, or <see langword="null"/> if no valid geometry is available.</returns>
        public static Circle2D? TerrainCircle(this IEnumerable<BuildingModel>? buildingModels, Circle2D? searchCircle = null, double padding = Constants.Default.TerrainPadding, double minimumRadius = Constants.Default.TerrainRadius)
        {
            if (buildingModels is null)
            {
                return searchCircle is null ? null : new Circle2D(searchCircle);
            }

            List<BuildingModel> buildingModels_List = [];
            foreach (BuildingModel buildingModel in buildingModels)
            {
                if (buildingModel is not null)
                {
                    buildingModels_List.Add(buildingModel);
                }
            }

            if (buildingModels_List.Count == 0)
            {
                return searchCircle is null ? null : new Circle2D(searchCircle);
            }

            Point2D? center = searchCircle?.Center;
            if (center is null)
            {
                List<BoundingBox3D> boundingBox3Ds = [];
                foreach (BuildingModel buildingModel in buildingModels_List)
                {
                    BoundingBox3D? boundingBox3D_Temp = buildingModel.GetBoundingBox();
                    if (boundingBox3D_Temp is not null)
                    {
                        boundingBox3Ds.Add(boundingBox3D_Temp);
                    }
                }

                if (boundingBox3Ds.Count == 0)
                {
                    return null;
                }

                BoundingBox3D boundingBox3D_Combined = new(boundingBox3Ds[0]);
                for (int i = 1; i < boundingBox3Ds.Count; i++)
                {
                    boundingBox3D_Combined.Add(boundingBox3Ds[i]);
                }

                Point3D? centroid = boundingBox3D_Combined.GetCentroid();
                if (centroid is null || !double.IsFinite(centroid.X) || !double.IsFinite(centroid.Y))
                {
                    return null;
                }

                center = new Point2D(centroid.X, centroid.Y);
            }

            double maxDistance = 0;
            foreach (BuildingModel buildingModel in buildingModels_List)
            {
                BoundingBox3D? boundingBox3D = buildingModel.GetBoundingBox();
                if (boundingBox3D is null)
                {
                    continue;
                }

                Point2D[] corners =
                [
                    new(boundingBox3D.MinX, boundingBox3D.MinY),
                    new(boundingBox3D.MinX, boundingBox3D.MaxY),
                    new(boundingBox3D.MaxX, boundingBox3D.MinY),
                    new(boundingBox3D.MaxX, boundingBox3D.MaxY)
                ];

                foreach (Point2D corner in corners)
                {
                    if (double.IsFinite(corner.X) && double.IsFinite(corner.Y))
                    {
                        double distance = center.Distance(corner);
                        if (distance > maxDistance)
                        {
                            maxDistance = distance;
                        }
                    }
                }
            }

            double searchRadius = searchCircle?.Radius ?? minimumRadius;
            double radius_Effective = System.Math.Max(searchRadius, maxDistance + padding);

            return new Circle2D(center, radius_Effective);
        }
    }
}
