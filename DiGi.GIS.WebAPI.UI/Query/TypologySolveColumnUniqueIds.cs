using DiGi.GIS.WebAPI.UI.Classes;
using System.Collections.Generic;

namespace DiGi.GIS.WebAPI.UI
{
    public static partial class Query
    {
        /// <summary>
        /// Lists the building data columns a Typology solve projects, by unique identifier: the definition's chain columns, then the built-in columns the solve and the view read.
        /// <para>Every level of the definition names its column by the catalog's unique identifier, so the chain contributes its levels' identifiers in chain order, once each. The reference column is always added: it names the buildings the solve files into buckets, and it is the keyset cursor that pages a part. The database identifier is added for the details and 3D viewer links the view offers per building. With <paramref name="clip"/> set, the internal point columns are added too, because a municipality or subdivision is cut out of its county's rows by each row's internal point. The county part column is never asked for - the upstream projects it on its own.</para>
        /// <para>Only identifiers the catalog lists are asked for: an identifier the upstream does not know would be a projection the fetch cannot honour, and a catalog missing a built-in column simply leaves the view without what that column carries (an identifier of 0, no clip).</para>
        /// </summary>
        /// <param name="typologyDefinitionParameter">The definition to solve, as the page holds it. This value can be null.</param>
        /// <param name="columns">The live column catalog of the building data table. This value can be null.</param>
        /// <param name="clip">A value indicating whether the rows are clipped to an area boundary, which needs the internal point columns.</param>
        /// <returns>The unique identifiers to project, in chain order followed by the built-in columns; empty when the definition or the catalog is null.</returns>
        public static List<string> TypologySolveColumnUniqueIds(TypologyDefinitionParameter? typologyDefinitionParameter, IEnumerable<DiGi.PostgreSQL.Table.Classes.Column>? columns, bool clip)
        {
            List<string> result = [];

            Dictionary<string, DiGi.PostgreSQL.Table.Classes.Column>? dictionary = ColumnDictionary(columns);
            if (typologyDefinitionParameter?.Levels is null || dictionary is null)
            {
                return result;
            }

            HashSet<string> uniqueIds = [];

            foreach (TypologyDefinitionLevelParameter level in typologyDefinitionParameter.Levels)
            {
                Add(level?.UniqueId);
            }

            Add(Constants.BuildingData.ReferenceUniqueId);
            Add(Constants.BuildingData.DatabaseIdUniqueId);

            if (clip)
            {
                Add(Constants.BuildingData.InternalPointXUniqueId);
                Add(Constants.BuildingData.InternalPointYUniqueId);
            }

            return result;

            void Add(string? uniqueId)
            {
                if (!string.IsNullOrWhiteSpace(uniqueId) && dictionary.ContainsKey(uniqueId) && uniqueIds.Add(uniqueId))
                {
                    result.Add(uniqueId);
                }
            }
        }
    }
}
