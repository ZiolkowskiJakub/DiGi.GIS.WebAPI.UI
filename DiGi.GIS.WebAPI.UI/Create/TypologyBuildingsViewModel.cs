using DiGi.GIS.WebAPI.UI.ViewModels;
using DiGi.Typology.Visual.Classes;
using System.Collections.Generic;
using System.Linq;

namespace DiGi.GIS.WebAPI.UI
{
    public static partial class Create
    {
        /// <summary>
        /// Flattens a solved <see cref="VisualTypology"/> tree into the view DTO the area view renders: the recursive node tree and the flat building list.
        /// <para>The tree is walked depth-first. Each node carries its name, description, color (read from the <see cref="VisualTypologyItem.Appearance"/> via <see cref="Query.Color(TypologyAppearance?)"/>), the count of its reference set and its children. A leaf node's references become <see cref="ViewModels.TypologyBuildingViewModel"/> entries in the flat list, each carrying the node's path so the view can join it to the dot position by <c>(Reference, CountyId)</c>.</para>
        /// <para>The <paramref name="countyId_ByReference"/> maps a building reference to the county part it was fetched from, so the flat entry carries the correct <c>CountyId</c>. When the map is null or a reference is absent from it, the entry's <c>CountyId</c> is 0 — the view treats that as "part unknown" and skips the centroid join for that building.</para>
        /// <para>The <paramref name="id_ByReference"/> maps a building reference to its database identifier, so the flat entry carries the <c>Id</c> the details and 3D viewer links address. When the map is null or a reference is absent from it, the entry's <c>Id</c> is 0 — the view treats that as "identifier unknown" and hides the links that need it.</para>
        /// </summary>
        /// <param name="visualTypology">The solved typology tree. This value can be null.</param>
        /// <param name="countyId_ByReference">A map from building reference to the county part identifier it was fetched from, or null when the part is not tracked.</param>
        /// <param name="id_ByReference">A map from building reference to its database identifier, or null when the identifier is not tracked.</param>
        /// <returns>The view DTO, or <see langword="null"/> when the input is null.</returns>
        public static ViewModels.TypologyBuildingsViewModel? TypologyBuildingsViewModel(this VisualTypology? visualTypology, Dictionary<string, int>? countyId_ByReference = null, Dictionary<string, long>? id_ByReference = null)
        {
            if (visualTypology is null)
            {
                return null;
            }

            List<TypologyBuildingViewModel> buildings = [];
            TypologyTreeNodeViewModel? root = Flatten(visualTypology);

            return new ViewModels.TypologyBuildingsViewModel(root, buildings);

            TypologyTreeNodeViewModel? Flatten(VisualTypology visualTypology_Node)
            {
                VisualTypologyItem? item = visualTypology_Node.TypologyItem;
                string? name = item?.Name;
                string? description = item?.Description;
                string? color = Query.Hex(Query.Color(item?.Appearance));
                List<int> path = visualTypology_Node.TypologyPath?.Values?.ToList() ?? [];

                // The solver files a reference on the bucket node that matched it at every level, so a bucket's reference set is its own membership - including rows dropped at a lower level. The root is no bucket and stores nothing, so it is counted as the sum of its children; taking the larger of the two covers both without understating either.
                int count = visualTypology_Node.References?.Count ?? 0;

                List<VisualTypology>? subTypologies = visualTypology_Node.SubTypologies;

                if (subTypologies is null || subTypologies.Count == 0)
                {
                    // Leaf node: emit the building entries.
                    List<string>? references = visualTypology_Node.References;
                    if (references is not null)
                    {
                        foreach (string reference in references)
                        {
                            int countyId = countyId_ByReference is not null && countyId_ByReference.TryGetValue(reference, out int partId) ? partId : 0;
                            long id = id_ByReference is not null && id_ByReference.TryGetValue(reference, out long databaseId) ? databaseId : 0;
                            buildings.Add(new TypologyBuildingViewModel(reference, id, countyId, path));
                        }
                    }

                    return new TypologyTreeNodeViewModel(name, description, color, path, count, null);
                }

                List<TypologyTreeNodeViewModel> children = [];
                int count_Children = 0;
                foreach (VisualTypology subTypology in subTypologies)
                {
                    if (subTypology is null)
                    {
                        continue;
                    }

                    TypologyTreeNodeViewModel? child = Flatten(subTypology);
                    if (child is not null)
                    {
                        children.Add(child);
                        count_Children += child.Count;
                    }
                }

                return new TypologyTreeNodeViewModel(name, description, color, path, System.Math.Max(count, count_Children), children.Count == 0 ? null : children);
            }
        }
    }
}
