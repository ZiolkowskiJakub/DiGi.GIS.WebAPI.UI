using DiGi.Analytical.Building.Classes;
using DiGi.Analytical.Building.Enums;
using DiGi.Analytical.Building.Interfaces;
using DiGi.Core.Classes;
using DiGi.Core.Interfaces;
using DiGi.Geometry.Spatial.Interfaces;
using DiGi.GIS.WebAPI.UI.Classes;
using DiGi.GLTF.Analytical;
using DiGi.GLTF.Classes;
using System;
using System.Collections.Generic;

namespace DiGi.GIS.WebAPI.UI
{
    public static partial class Create
    {
        /// <summary>
        /// Builds the glTF nodes of the solar radiation viewer from already calculated results: the analysed building component by component, each receiving surface coloured by its annual irradiation (<see cref="Query.SolarIrradiationColor(double)"/>) and carrying its <see cref="SurfaceSolarRadiationResult"/> as the node properties, every other component neutral grey, and the neighbours as grey semi-transparent envelopes for context.
        /// <para>It takes results rather than solving, so a stored calculation can be displayed again without a new solve. No terrain is added: the solve ignores it.</para>
        /// </summary>
        /// <param name="buildingModel">The analysed building model. This value can be null.</param>
        /// <param name="buildingModels_Surrounding">The neighbouring building models shown for context, or null for none. A copy of <paramref name="buildingModel"/> among them is skipped.</param>
        /// <param name="surfaceSolarRadiationResults">The results of the receiving surfaces, matched to the components by <see cref="SurfaceSolarRadiationResult.Reference"/>. This value can be null.</param>
        /// <param name="reference">The optional root reference of the building model (a county + building <see cref="ComplexReference"/>, as the 3D building viewer uses); each component node extends it by the component's step.</param>
        /// <param name="tolerance">The distance tolerance used by the triangulation.</param>
        /// <returns>The nodes in world coordinates, or <see langword="null"/> when the building model is null or has no convertible component.</returns>
        public static List<GLTFNode>? SolarGLTFNodes(this BuildingModel? buildingModel, IEnumerable<BuildingModel>? buildingModels_Surrounding, IEnumerable<SurfaceSolarRadiationResult>? surfaceSolarRadiationResults, IReference? reference, double tolerance = Core.Constants.Tolerance.Distance)
        {
            List<IComponent>? components = buildingModel?.GetComponents<IComponent>();
            if (buildingModel is null || components is null)
            {
                return null;
            }

            Dictionary<string, SurfaceSolarRadiationResult> surfaceSolarRadiationResults_ByReference = [];
            if (surfaceSolarRadiationResults is not null)
            {
                foreach (SurfaceSolarRadiationResult surfaceSolarRadiationResult in surfaceSolarRadiationResults)
                {
                    string? reference_Result = surfaceSolarRadiationResult?.Reference;
                    if (reference_Result is not null)
                    {
                        surfaceSolarRadiationResults_ByReference[reference_Result] = surfaceSolarRadiationResult!;
                    }
                }
            }

            Color color_Component = new(byte.MaxValue, 200, 200, 200);
            Color color_Surrounding = new(byte.MaxValue, 160, 160, 160);

            List<GLTFNode> result = [];
            foreach (IComponent component in components)
            {
                // DiGi qualifier: DiGi.GIS.Analytical captures a bare Analytical in this namespace.
                ISurface3D? surface3D = DiGi.Analytical.Building.Query.Surface3D(component);
                if (surface3D is null)
                {
                    continue;
                }

                IReference? reference_Component = reference is null ? Core.Create.UniqueReference(component) : Core.Create.Reference(reference, Core.Create.UniqueReference(component));

                string? name = GLTF.Analytical.Query.Name(reference_Component);
                if (string.IsNullOrWhiteSpace(name))
                {
                    name = component.GetType().Name;
                }

                GLTFNode? gLTFNode;
                string? reference_Guid = new GuidReference(component).ToString();
                if (reference_Guid is not null && surfaceSolarRadiationResults_ByReference.TryGetValue(reference_Guid, out SurfaceSolarRadiationResult? surfaceSolarRadiationResult))
                {
                    gLTFNode = GLTF.Create.GLTFNode(surface3D, name, reference_Component?.ToString(), Query.SolarIrradiationColor(surfaceSolarRadiationResult.Irradiation), 1, Core.Convert.ToSystem_String(surfaceSolarRadiationResult), tolerance);
                }
                else
                {
                    gLTFNode = GLTF.Create.GLTFNode(surface3D, name, reference_Component?.ToString(), color_Component, 1, component is ISerializableObject serializableObject ? Core.Convert.ToSystem_String(serializableObject) : null, tolerance);
                }

                if (gLTFNode is not null)
                {
                    result.Add(gLTFNode);
                }
            }

            if (result.Count == 0)
            {
                return null;
            }

            if (buildingModels_Surrounding is not null)
            {
                HashSet<Guid> guids = [buildingModel.Guid];
                foreach (BuildingModel? buildingModel_Surrounding in buildingModels_Surrounding)
                {
                    if (buildingModel_Surrounding is null || !guids.Add(buildingModel_Surrounding.Guid))
                    {
                        continue;
                    }

                    List<GLTFNode>? gLTFNodes_Surrounding = buildingModel_Surrounding.ToGLTF_GLTFNodes(null, tolerance, BuildingModelDetailLevel.Envelope);
                    if (gLTFNodes_Surrounding is null)
                    {
                        continue;
                    }

                    foreach (GLTFNode gLTFNode_Surrounding in gLTFNodes_Surrounding)
                    {
                        result.Add(new GLTFNode(gLTFNode_Surrounding.Name, gLTFNode_Surrounding.Reference, gLTFNode_Surrounding.Mesh3D, color_Surrounding, 0.35, gLTFNode_Surrounding.Properties));
                    }
                }
            }

            return result;
        }
    }
}
