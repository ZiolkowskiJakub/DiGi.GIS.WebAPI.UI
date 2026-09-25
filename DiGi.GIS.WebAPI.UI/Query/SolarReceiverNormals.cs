using DiGi.Analytical.Building.Classes;
using DiGi.Analytical.Building.Interfaces;
using DiGi.Analytical.Classes;
using DiGi.Core.Classes;
using DiGi.Core.Interfaces;
using DiGi.Geometry.Core.Enums;
using DiGi.Geometry.Spatial.Classes;
using System;
using System.Collections.Generic;

namespace DiGi.GIS.WebAPI.UI
{
    public static partial class Query
    {
        /// <summary>
        /// Collects the receiving surfaces of a solar radiation calculation - the external walls and roofs of a building - with the outward unit normal of each.
        /// <para>A component's stored normal is its drawing orientation, not the outward one, while the irradiance of a surface depends on which way it faces: with a stored normal pointing into the building, a north wall would receive the south wall's beam. The outward normals are therefore taken from <see cref="BuildingModel.GetExternalShell(Side?, Geometry.Core.Enums.Orientation?, Geometry.Core.Enums.Orientation?, double)"/> with <see cref="Side.External"/>, whose faces are oriented over the whole envelope.</para>
        /// <para>Floors are excluded because the ground floor faces the soil. Internal partitions are not part of the envelope and are excluded with them.</para>
        /// </summary>
        /// <param name="buildingModel">The building model. This value can be null.</param>
        /// <returns>The outward unit normal of each receiving surface, keyed by the <see cref="GuidReference"/> string of its component - the string <c>ToSolar</c> writes to the shading element - or <see langword="null"/> when the model has no external envelope (no space relation, fewer than four external faces, or a component with more than one geometry). A degenerate model, such as a sliver footprint with walls and no roof or floor, is answered with <see langword="null"/>.</returns>
        public static Dictionary<string, Vector3D>? SolarReceiverNormals(this BuildingModel? buildingModel)
        {
            if (buildingModel is null)
            {
                return null;
            }

            Shell? shell;
            try
            {
                shell = buildingModel.GetExternalShell(Side.External);
            }
            catch (NotImplementedException)
            {
                // Query.Geometry3D does not support a component carrying more than one geometry.
                shell = null;
            }

            if (shell is null)
            {
                return null;
            }

            Dictionary<Guid, IComponent> components_ByGuid = [];
            List<IComponent>? components = buildingModel.GetComponents<IComponent>();
            if (components is not null)
            {
                foreach (IComponent component in components)
                {
                    if (component is IWall || component is IRoof)
                    {
                        components_ByGuid[component.Guid] = component;
                    }
                }
            }

            Dictionary<string, Vector3D> result = [];

            // The getter clones on access, so the list is read once.
            List<Face>? faces = shell.PolygonalFaces;
            if (faces is null)
            {
                return result;
            }

            foreach (Face face in faces)
            {
                // Face.UniqueReference is a fresh clone on every call, so it is read once.
                IUniqueReference? uniqueReference = face.UniqueReference;
                if (uniqueReference is not GuidReference guidReference || !components_ByGuid.TryGetValue(guidReference.Guid, out IComponent? component))
                {
                    continue;
                }

                Vector3D? normal = face.Plane?.Normal?.Unit;
                if (normal is null || !double.IsFinite(normal.X) || !double.IsFinite(normal.Y) || !double.IsFinite(normal.Z))
                {
                    continue;
                }

                string? reference = new GuidReference(component).ToString();
                if (reference is not null)
                {
                    result.TryAdd(reference, normal);
                }
            }

            return result;
        }
    }
}
