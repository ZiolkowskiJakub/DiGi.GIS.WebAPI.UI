using DiGi.Analytical.Building;
using DiGi.Analytical.Building.Classes;
using DiGi.Analytical.Building.Interfaces;
using DiGi.Communication.Classes;
using DiGi.Core.Interfaces;
using DiGi.Geometry.Spatial.Classes;
using DiGi.Geometry.Spatial.Interfaces;
using System.Collections.Generic;

namespace DiGi.GIS.WebAPI.UI
{
    public static partial class Convert
    {
        // Building -> Mesh3D -> ScatteringObject conversion keeps the strict dependency rule intact:
        // DiGi.Communication and DiGi.Communication.WebAPI never reference any GIS library, so the
        // GIS domain objects are reduced to plain triangulated geometry before leaving this project.

        /// <summary>
        /// Converts the specified <see cref="BuildingModel"/> into a list of <see cref="ScatteringObject"/> instances (one per building component) by gathering and triangulating the surface of each component.
        /// </summary>
        /// <param name="buidlingModel">The <see cref="BuildingModel"/> to be converted. This value can be null.</param>
        /// <param name="tolerance">The distance tolerance used during triangulation.</param>
        /// <returns>A list of <see cref="ScatteringObject"/> instances (one per component), or null if the building model or its components are null.</returns>
        public static List<ScatteringObject>? ToCommunication(this BuildingModel? buidlingModel, double tolerance = Core.Constants.Tolerance.Distance)
        {
            if (buidlingModel is null)
            {
                return null;
            }

            string? referenceBuildingModel = buidlingModel.UniqueId ?? Core.Create.UniqueReference(buidlingModel)?.ToString();
            if (string.IsNullOrWhiteSpace(referenceBuildingModel))
            {
                return null;
            }

            List<IComponent>? components = buidlingModel.GetComponents<IComponent>();
            if (components is null)
            {
                return null;
            }

            List<ScatteringObject> result = [];
            foreach (IComponent component in components)
            {
                ISurface3D? surface3D = component.Surface3D();
                if (surface3D is null)
                {
                    continue;
                }

                Mesh3D? mesh3DComponent = GLTF.Create.Mesh3D(surface3D, tolerance);
                if (mesh3DComponent is null)
                {
                    continue;
                }

                string? reference;

                IReference? reference_Temp = PostgreSQL.Create.Reference(buidlingModel, component);
                if(reference_Temp is null)
                {
                    string? referenceComponent = Core.Create.UniqueReference(component)?.ToString();
                    if (string.IsNullOrWhiteSpace(referenceComponent))
                    {
                        continue;
                    }

                    reference = $"{referenceBuildingModel}::{referenceComponent}";
                }
                else
                {
                    reference = reference_Temp?.ToString();
                }

                result.Add(new ScatteringObject(reference, mesh3DComponent, Communication.Constants.ElectricalProperties.Concrete));
            }

            return result;
        }

    }
}