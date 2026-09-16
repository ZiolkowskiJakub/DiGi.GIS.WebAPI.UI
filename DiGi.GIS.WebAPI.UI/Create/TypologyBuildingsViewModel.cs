using DiGi.Core.IO.Table.Classes;
using DiGi.GIS.WebAPI.UI.ViewModels;
using DiGi.Typology.Visual.Classes;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace DiGi.GIS.WebAPI.UI
{
    public static partial class Create
    {
        /// <summary>
        /// Flattens a solved <see cref="VisualTypology"/> tree into the view DTO the area view renders: the recursive node tree and the flat building list.
        /// <para>The tree is walked depth-first, children before their parent. Each node carries its name, description, color (read from the <see cref="VisualTypologyItem.Appearance"/> via <see cref="Query.Color(TypologyAppearance?)"/>), the count of its reference set and its children. Every reference becomes one <see cref="ViewModels.TypologyBuildingViewModel"/> entry, filed under the <b>deepest</b> node that holds it: a leaf's references are its own, and a bucket's references that none of its children holds - the rows the solver dropped at the next level because their value resolved to no bucket there - are filed under the bucket itself. So a building the tree counts under a node is always listed under that node or one below it, and the map, the grid and the building card agree with the tree; the pie still charts what the children hold against the node's own count.</para>
        /// <para>The siblings of every node come back sorted ascending by bucket value - a range by its Min, a unique value by the value itself, a multi-word string value by its whole text - read from the name the solver gives the node, and the building list is emitted in that same tree order, so the view renders the buckets in value order rather than in the solver's data-encounter order (issue #36).</para>
        /// <para>The <paramref name="countyId_ByReference"/> maps a building reference to the county part it was fetched from, so the flat entry carries the correct <c>CountyId</c>. When the map is null or a reference is absent from it, the entry's <c>CountyId</c> is 0 - the view treats that as "part unknown" and skips the centroid join for that building.</para>
        /// <para>The <paramref name="id_ByReference"/> maps a building reference to its database identifier, so the flat entry carries the <c>Id</c> the details and 3D viewer links address. When the map is null or a reference is absent from it, the entry's <c>Id</c> is 0 - the view treats that as "identifier unknown" and hides the links that need it.</para>
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
            HashSet<string> references_Emitted = [];
            TypologyTreeNodeViewModel? root = Flatten(visualTypology);

            return new ViewModels.TypologyBuildingsViewModel(root, buildings);

            TypologyTreeNodeViewModel? Flatten(VisualTypology visualTypology_Node)
            {
                VisualTypologyItem? item = visualTypology_Node.TypologyItem;
                string? name = item?.Name;
                string? description = item?.Description;
                string? color = Query.Hex(Query.Color(item?.Appearance));
                List<int> path = visualTypology_Node.TypologyPath?.Values?.ToList() ?? [];

                // Children first, so that every reference a child holds is already filed by the time the node's own set is read.
                List<TypologyTreeNodeViewModel>? children = null;
                int count_Children = 0;
                List<VisualTypology>? subTypologies = visualTypology_Node.SubTypologies;
                if (subTypologies is not null && subTypologies.Count > 1)
                {
                    // Siblings in ascending bucket value - a range by its Min, a unique value by the value itself - read
                    // from the name the solver gives the node ("column name + rule data text"). A local copy, so the
                    // solved tree stays unmodified; the key is a comparable tuple - a numeric kind before a text kind,
                    // then the number, then the name - and OrderBy is stable, so two siblings with an equal key keep
                    // their solved order.
                    subTypologies = [.. subTypologies.OrderBy(subTypology => SortKey(subTypology?.TypologyItem?.Name))];
                }

                if (subTypologies is not null && subTypologies.Count != 0)
                {
                    children = [];
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

                    if (children.Count == 0)
                    {
                        children = null;
                    }
                }

                // Read once: the property copies the set on every call. The solver files a reference on the bucket node
                // that matched it at every level, so a bucket's reference set is its own membership - including rows
                // dropped at a lower level, which no child filed and which are therefore filed here. The root is no
                // bucket and stores nothing, so it is counted as the sum of its children; taking the larger of the two
                // covers both without understating either.
                List<string>? references = visualTypology_Node.References;
                int count = references?.Count ?? 0;
                if (references is not null)
                {
                    foreach (string reference in references)
                    {
                        if (!references_Emitted.Add(reference))
                        {
                            continue;
                        }

                        int countyId = countyId_ByReference is not null && countyId_ByReference.TryGetValue(reference, out int partId) ? partId : 0;
                        long id = id_ByReference is not null && id_ByReference.TryGetValue(reference, out long databaseId) ? databaseId : 0;
                        buildings.Add(new TypologyBuildingViewModel(reference, id, countyId, path));
                    }
                }

                return new TypologyTreeNodeViewModel(name, description, color, path, System.Math.Max(count, count_Children), children);
            }

            // The sort key, comparable by its default comparer: the kind first (0 = a numeric key, 1 = a text key, so
            // a numeric key precedes a text one), then the number, then the name for the text compare.
            (int kind, double numeric, string text) SortKey(string? name)
            {
                if (string.IsNullOrWhiteSpace(name))
                {
                    return (1, 0.0, string.Empty);
                }

                int index_Bracket = name.LastIndexOf('[');
                if (index_Bracket != -1)
                {
                    // "[min, max)" / "[min, max]": the Min is what follows the last bracket.
                    int index_End = name.IndexOfAny([',', ' ', ']'], index_Bracket + 1);
                    string token_Min = name[(index_Bracket + 1)..(index_End == -1 ? name.Length : index_End)].Trim();
                    if (TrySortValue(token_Min, out double min))
                    {
                        return (0, min, string.Empty);
                    }

                    return (1, 0.0, name[index_Bracket..]);
                }

                int index_Space = name.LastIndexOf(' ');
                if (index_Space != -1)
                {
                    string token_Value = name[(index_Space + 1)..].Trim();
                    if (TrySortValue(token_Value, out double value))
                    {
                        return (0, value, string.Empty);
                    }
                }

                // A value the number parse does not take is text, possibly multi-word: the whole name is the key,
                // because every sibling shares the column prefix, so comparing the names orders the values.
                return (1, 0.0, name);
            }

            bool TrySortValue(string token, out double value)
            {
                value = 0.0;

                // A non-finite parse is the value text "NaN" or "Infinity" - text, not a number.
                if (double.TryParse(token, NumberStyles.Float, CultureInfo.InvariantCulture, out double result) && double.IsFinite(result))
                {
                    value = result;
                    return true;
                }

                // The name is rendered with the server's current culture (Coding - General, ToString is not a key):
                // a comma decimal separator still means a number.
                if (double.TryParse(token.Replace(',', '.'), NumberStyles.Float, CultureInfo.InvariantCulture, out double result_Comma) && double.IsFinite(result_Comma))
                {
                    value = result_Comma;
                    return true;
                }

                return false;
            }
        }

        /// <summary>
        /// Flattens a solved <see cref="VisualTypology"/> tree into the view DTO, reading each building's county part and database identifier from the table the tree was solved over.
        /// <para>The overload the solve action uses: the merged building data table carries the <c>Reference</c>, <c>County Id</c> and <c>Database Id</c> columns of every row (the first two the upstream projects whether asked for or not), so the two maps the other overload takes are read from it in one pass over its live rows and the caller keeps no bookkeeping of its own. A column the table does not carry leaves the corresponding value at 0 for every building.</para>
        /// </summary>
        /// <param name="visualTypology">The solved typology tree. This value can be null.</param>
        /// <param name="table">The building data table the tree was solved over. This value can be null, in which case no part or identifier is known.</param>
        /// <returns>The view DTO, or <see langword="null"/> when the tree is null.</returns>
        public static ViewModels.TypologyBuildingsViewModel? TypologyBuildingsViewModel(this VisualTypology? visualTypology, Table? table)
        {
            if (visualTypology is null)
            {
                return null;
            }

            Dictionary<string, int>? countyId_ByReference = null;
            Dictionary<string, long>? id_ByReference = null;

            int index_Reference = table?.GetColumnIndex(Constants.BuildingData.ReferenceName) ?? -1;
            if (table is not null && index_Reference != -1)
            {
                int index_CountyId = table.GetColumnIndex(Constants.BuildingData.CountyIdName);
                int index_Id = table.GetColumnIndex(Constants.BuildingData.DatabaseIdName);

                countyId_ByReference = index_CountyId == -1 ? null : new Dictionary<string, int>(table.RowCount);
                id_ByReference = index_Id == -1 ? null : new Dictionary<string, long>(table.RowCount);

                foreach (Row row in table)
                {
                    if (row[index_Reference] is not string reference)
                    {
                        continue;
                    }

                    // The table converter types each cell by its declared column (int and long); the other integer widths are a defensive fallback.
                    if (countyId_ByReference is not null)
                    {
                        object? value_CountyId = row[index_CountyId];
                        if (value_CountyId is int countyId)
                        {
                            countyId_ByReference[reference] = countyId;
                        }
                        else if (value_CountyId is long countyId_Long)
                        {
                            countyId_ByReference[reference] = (int)countyId_Long;
                        }
                    }

                    if (id_ByReference is not null)
                    {
                        object? value_Id = row[index_Id];
                        if (value_Id is long id)
                        {
                            id_ByReference[reference] = id;
                        }
                        else if (value_Id is int id_Int)
                        {
                            id_ByReference[reference] = id_Int;
                        }
                    }
                }
            }

            return visualTypology.TypologyBuildingsViewModel(countyId_ByReference, id_ByReference);
        }
    }
}
