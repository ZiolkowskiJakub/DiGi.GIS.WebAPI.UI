using DiGi.Analytical.Building.Classes;
using DiGi.Analytical.Building.Interfaces;
using DiGi.Analytical.Urban.Classes;
using DiGi.Core;
using DiGi.Core.Interfaces;
using DiGi.Geometry.Planar.Classes;
using DiGi.Geometry.Spatial;
using DiGi.Geometry.Spatial.Classes;
using DiGi.Geometry.Spatial.Interfaces;
using DiGi.GIS.Classes;
using DiGi.GLTF.Classes;
using System.Collections.Generic;

namespace DiGi.GIS.WebAPI.UI
{
    public static partial class Convert
    {
        // The generic ISerializableObject dispatch lives in the DiGi.GLTF engine
        // (DiGi.GLTF.Convert.ToGLTF_GLTFNodes): the converter classes under /Classes/Converter
        // plug the typed methods below into the engine registry (see Program.cs).

        /// <summary>
        /// Converts the specified <see cref="Building2D"/> into <see cref="GLTFNode"/> instances by extruding its 2D polygonal footprint by the number of storeys multiplied by the storey height.
        /// </summary>
        /// <param name="building2D">The <see cref="Building2D"/> to be converted. This value can be null.</param>
        /// <param name="storeyHeight">The height of a single storey in meters used for the extrusion.</param>
        /// <param name="tolerance">The distance tolerance used during triangulation.</param>
        /// <returns>A list with a single <see cref="GLTFNode"/> representing the extruded building, or null if the building or its geometry is null.</returns>
        public static List<GLTFNode>? ToGLTF_GLTFNodes(this Building2D? building2D, double storeyHeight = Constants.Default.StoreyHeight, double tolerance = DiGi.Core.Constants.Tolerance.Distance)
        {
            PolygonalFace2D? polygonalFace2D = building2D?.PolygonalFace2D;
            if (building2D is null || polygonalFace2D is null)
            {
                return null;
            }

            Plane? plane = Geometry.Spatial.Create.Plane(0);
            if (plane is null)
            {
                return null;
            }

            PolygonalFace3D? polygonalFace3D = plane.Convert(polygonalFace2D);
            if (polygonalFace3D is null)
            {
                return null;
            }

            int storeys = building2D.Storeys < 1 ? 1 : building2D.Storeys;

            PolygonalFaceExtrusion polygonalFaceExtrusion = new(polygonalFace3D, new Vector3D(0, 0, storeys * storeyHeight));

            string? reference = building2D.Reference ?? Core.Create.UniqueReference(building2D)?.ToString();

            GLTFNode? gLTFNode = GLTF.Create.GLTFNode(polygonalFaceExtrusion, $"Building2D {reference}", reference, Query.Color(building2D), 1, building2D.ToSystem_String(), tolerance);
            if (gLTFNode is null)
            {
                return null;
            }

            return [gLTFNode];
        }

        /// <summary>
        /// Converts the specified <see cref="Building2D"/> collection into <see cref="GLTFNode"/> instances by extruding each 2D polygonal footprint by its number of storeys multiplied by the storey height.
        /// </summary>
        /// <param name="building2Ds">The <see cref="Building2D"/> collection to be converted. This value can be null.</param>
        /// <param name="storeyHeight">The height of a single storey in meters used for the extrusions.</param>
        /// <param name="tolerance">The distance tolerance used during triangulation.</param>
        /// <returns>A list of <see cref="GLTFNode"/> instances for all convertible buildings, or null if <paramref name="building2Ds"/> is null.</returns>
        public static List<GLTFNode>? ToGLTF_GLTFNodes(this IEnumerable<Building2D>? building2Ds, double storeyHeight = Constants.Default.StoreyHeight, double tolerance = DiGi.Core.Constants.Tolerance.Distance)
        {
            if (building2Ds is null)
            {
                return null;
            }

            List<GLTFNode> result = [];
            foreach (Building2D building2D in building2Ds)
            {
                List<GLTFNode>? gLTFNodes = ToGLTF_GLTFNodes(building2D, storeyHeight, tolerance);
                if (gLTFNodes is not null)
                {
                    result.AddRange(gLTFNodes);
                }
            }

            return result;
        }

        /// <summary>
        /// Converts all components of the specified <see cref="BuildingModel"/> (walls, roofs, floors and other components with surface geometry) into <see cref="GLTFNode"/> instances.
        /// </summary>
        /// <param name="buildingModel">The <see cref="BuildingModel"/> to be converted. This value can be null.</param>
        /// <param name="tolerance">The distance tolerance used during triangulation.</param>
        /// <returns>A list of <see cref="GLTFNode"/> instances for all convertible components, or null if the building model is null or has no components.</returns>
        public static List<GLTFNode>? ToGLTF_GLTFNodes(this BuildingModel? buildingModel, double tolerance = DiGi.Core.Constants.Tolerance.Distance)
        {
            List<IComponent>? components = buildingModel?.GetComponents<IComponent>();
            if (components is null)
            {
                return null;
            }

            List<GLTFNode> result = [];
            foreach (IComponent component in components)
            {
                List<GLTFNode>? gLTFNodes = ToGLTF_GLTFNodes(component, tolerance);
                if (gLTFNodes is not null)
                {
                    result.AddRange(gLTFNodes);
                }
            }

            return result;
        }

        /// <summary>
        /// Converts the specified <see cref="UrbanModel"/> into <see cref="GLTFNode"/> instances by converting all contained <see cref="BuildingModel"/> instances.
        /// </summary>
        /// <param name="urbanModel">The <see cref="UrbanModel"/> to be converted. This value can be null.</param>
        /// <param name="tolerance">The distance tolerance used during triangulation.</param>
        /// <returns>A list of <see cref="GLTFNode"/> instances for all contained building models, or null if the urban model is null or empty.</returns>
        public static List<GLTFNode>? ToGLTF_GLTFNodes(this UrbanModel? urbanModel, double tolerance = DiGi.Core.Constants.Tolerance.Distance)
        {
            List<BuildingModel>? buildingModels = urbanModel?.GetBuildingModels();
            if (buildingModels is null)
            {
                return null;
            }

            List<GLTFNode> result = [];
            foreach (BuildingModel buildingModel in buildingModels)
            {
                List<GLTFNode>? gLTFNodes = ToGLTF_GLTFNodes(buildingModel, tolerance);
                if (gLTFNodes is not null)
                {
                    result.AddRange(gLTFNodes);
                }
            }

            return result;
        }

        /// <summary>
        /// Converts the specified building <see cref="IComponent"/> (for example a wall or a roof) into <see cref="GLTFNode"/> instances using its surface geometry and default component styling.
        /// </summary>
        /// <param name="component">The building component to be converted. This value can be null.</param>
        /// <param name="tolerance">The distance tolerance used during triangulation.</param>
        /// <returns>A list with a single <see cref="GLTFNode"/> representing the component, or null if the component has no supported surface geometry.</returns>
        public static List<GLTFNode>? ToGLTF_GLTFNodes(this IComponent? component, double tolerance = DiGi.Core.Constants.Tolerance.Distance)
        {
            if (component is not IBuildingGeometryObject<ISurface3D> buildingGeometryObject)
            {
                return null;
            }

            ISurface3D? surface3D = buildingGeometryObject.Geometry;
            if (surface3D is null)
            {
                return null;
            }

            double opacity = component is IWindow ? 0.5 : 1;

            string? properties = (component as ISerializableObject)?.ToSystem_String();

            GLTFNode? gLTFNode = GLTF.Create.GLTFNode(surface3D, component.GetType().Name, Core.Create.UniqueReference(component)?.ToString(), Query.Color(component as ISerializableObject), opacity, properties, tolerance);
            if (gLTFNode is null)
            {
                return null;
            }

            return [gLTFNode];
        }
    }
}
