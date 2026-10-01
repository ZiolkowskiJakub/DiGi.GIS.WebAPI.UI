using DiGi.Analytical.Building.Classes;
using DiGi.Analytical.Building.Interfaces;
using DiGi.Core.Classes;
using DiGi.Core.Interfaces;
using DiGi.GIS.WebAPI.UI.Classes;
using System.Collections.Generic;

namespace DiGi.GIS.WebAPI.UI
{
    public static partial class Create
    {
        /// <summary>
        /// Builds the solar radiation view of a building from already calculated results: one <see cref="ViewModels.SolarSurfaceViewModel"/> per receiving surface, referenced exactly like the Building Viewer node of its component, coloured by <see cref="Query.SolarIrradiationColor(double)"/> and carrying its <see cref="SurfaceSolarRadiationResult"/> as properties.
        /// <para>The node reference is built the way <c>DiGi.GLTF.Analytical.Convert.ToGLTF_GLTFNodes(BuildingModel, IReference)</c> builds it for the viewer scene: the root <paramref name="reference"/> extended by the component's unique reference, or that unique reference alone when the root is null. Components without a result are left out, so the viewer keeps their appearance.</para>
        /// </summary>
        /// <param name="buildingModel">The analysed building model. This value can be null.</param>
        /// <param name="surfaceSolarRadiationResults">The results of the receiving surfaces, matched to the components by <see cref="SurfaceSolarRadiationResult.Reference"/>. This value can be null.</param>
        /// <param name="reference">The root reference of the building model in the viewer scene (a county + building <see cref="ComplexReference"/>, see <c>PostgreSQL.Create.Reference</c>). This value can be null.</param>
        /// <param name="radius">The neighbour radius the calculation used, in metres.</param>
        /// <param name="stationName">The name of the EPW weather station. This value can be null.</param>
        /// <param name="stationUrl">The application relative URL of the EPW file page of the station. This value can be null.</param>
        /// <returns>The view, or <see langword="null"/> when the building model or the results are null, or no component has a result.</returns>
        public static ViewModels.SolarRadiationViewModel? SolarRadiationViewModel(this BuildingModel? buildingModel, IEnumerable<SurfaceSolarRadiationResult>? surfaceSolarRadiationResults, IReference? reference, double radius, string? stationName, string? stationUrl)
        {
            List<IComponent>? components = buildingModel?.GetComponents<IComponent>();
            if (components is null || surfaceSolarRadiationResults is null)
            {
                return null;
            }

            // Keyed by the result's reference text (GuidReference.ToString()), never by IReference equality.
            Dictionary<string, SurfaceSolarRadiationResult> surfaceSolarRadiationResults_ByReference = [];
            foreach (SurfaceSolarRadiationResult surfaceSolarRadiationResult in surfaceSolarRadiationResults)
            {
                string? reference_Result = surfaceSolarRadiationResult?.Reference;
                if (reference_Result is not null)
                {
                    surfaceSolarRadiationResults_ByReference[reference_Result] = surfaceSolarRadiationResult!;
                }
            }

            List<ViewModels.SolarSurfaceViewModel> solarSurfaceViewModels = [];
            foreach (IComponent component in components)
            {
                string? reference_Guid = new GuidReference(component).ToString();
                if (reference_Guid is null || !surfaceSolarRadiationResults_ByReference.TryGetValue(reference_Guid, out SurfaceSolarRadiationResult? surfaceSolarRadiationResult))
                {
                    continue;
                }

                IReference? reference_Component = reference is null ? Core.Create.UniqueReference(component) : Core.Create.Reference(reference, Core.Create.UniqueReference(component));
                string? reference_Node = reference_Component?.ToString();
                string? color = Query.SolarIrradiationColor(surfaceSolarRadiationResult.Irradiation).Hex();
                if (string.IsNullOrEmpty(reference_Node) || color is null)
                {
                    continue;
                }

                solarSurfaceViewModels.Add(new ViewModels.SolarSurfaceViewModel(reference_Node, color, Core.Convert.ToSystem_String(surfaceSolarRadiationResult)));
            }

            if (solarSurfaceViewModels.Count == 0)
            {
                return null;
            }

            return new ViewModels.SolarRadiationViewModel(radius, stationName, stationUrl, solarSurfaceViewModels);
        }
    }
}
