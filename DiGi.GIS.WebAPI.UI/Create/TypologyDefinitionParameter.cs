using DiGi.Core.Classes;
using DiGi.Core.Enums;
using DiGi.Typology.Visual.Classes;
using System.Collections.Generic;

namespace DiGi.GIS.WebAPI.UI
{
    public static partial class Create
    {
        /// <summary>
        /// Creates the page state of the Typology definition page from a document: one level per link of the <see cref="VisualColumnTypologyFilter{UColumn}"/> chain, with the column described from the live catalog and the rule's colors rendered as the page holds them.
        /// <para>The document is checked by <see cref="Query.TypologyDefinitionErrors(VisualColumnTypologyFilter{Core.IO.Table.Classes.Column}, IEnumerable{DiGi.PostgreSQL.Table.Classes.Column})"/> first and nothing is built when it reports anything, so an import never replaces the page with a level it cannot show. Range rows come from the rule in ascending order with the color filed under the range; unique-value rows come from the collection keys, each converted back to the column's CLR type (<c>"null"</c> to the NULL bucket) so the page compares them with the values the unique-values endpoint lists. The inactive row list of a level is left empty rather than null, the shape <c>typology.js</c> keeps.</para>
        /// </summary>
        /// <param name="visualColumnTypologyFilter">The root of the document chain. This value can be null.</param>
        /// <param name="columns">The live column catalog of the building data table. This value can be null.</param>
        /// <returns>The page state, or <see langword="null"/> when the document is invalid or the catalog is unavailable.</returns>
        public static Classes.TypologyDefinitionParameter? TypologyDefinitionParameter(this VisualColumnTypologyFilter<Core.IO.Table.Classes.Column>? visualColumnTypologyFilter, IEnumerable<DiGi.PostgreSQL.Table.Classes.Column>? columns)
        {
            if (visualColumnTypologyFilter == null || Query.TypologyDefinitionErrors(visualColumnTypologyFilter, columns).Count != 0)
            {
                return null;
            }

            Dictionary<string, DiGi.PostgreSQL.Table.Classes.Column>? dictionary = Query.ColumnDictionary(columns);
            if (dictionary == null)
            {
                return null;
            }

            List<Classes.TypologyDefinitionLevelParameter> levels = [];

            VisualColumnTypologyFilter<Core.IO.Table.Classes.Column>? filter = visualColumnTypologyFilter;
            while (filter != null)
            {
                DiGi.PostgreSQL.Table.Classes.Column column = dictionary[Core.IO.Query.UniqueId(filter.Value)!];
                DataType dataType = column.DataType ?? DataType.Undefined;

                Classes.TypologyDefinitionLevelParameter level = new()
                {
                    UniqueId = column.UniqueId,
                    Name = column.Name,
                    DataType = (int)dataType,
                    IsNumeric = Core.Query.IsNumeric(dataType),
                    RuleType = filter.Rule?.GetType().Name,
                    Ranges = [],
                    UniqueValueColors = []
                };

                switch (filter.Rule)
                {
                    case VisualIntegerRangeFilterRule visualIntegerRangeFilterRule:
                        foreach (Range<int> range in visualIntegerRangeFilterRule.Ranges)
                        {
                            level.Ranges.Add(new Classes.TypologyDefinitionRangeParameter() { Min = range.Min, Max = range.Max, Color = Query.Hex(Query.Color(visualIntegerRangeFilterRule.TypologyAppearanceCollection[range])) });
                        }
                        break;

                    case VisualDoubleRangeFilterRule visualDoubleRangeFilterRule:
                        foreach (Range<double> range in visualDoubleRangeFilterRule.Ranges)
                        {
                            level.Ranges.Add(new Classes.TypologyDefinitionRangeParameter() { Min = range.Min, Max = range.Max, Color = Query.Hex(Query.Color(visualDoubleRangeFilterRule.TypologyAppearanceCollection[range])) });
                        }
                        break;

                    case VisualUniqueValueFilterRule visualUniqueValueFilterRule:
                        foreach (string key in visualUniqueValueFilterRule.TypologyAppearanceCollection.Keys)
                        {
                            object? value = null;
                            if (key != Core.Constants.UniqueId.Null)
                            {
                                Core.Query.TryConvert(key, out value, dataType);
                            }

                            level.UniqueValueColors.Add(new Classes.TypologyDefinitionUniqueValueParameter() { Value = value, Color = Query.Hex(Query.Color(visualUniqueValueFilterRule.TypologyAppearanceCollection[key])) });
                        }
                        break;
                }

                levels.Add(level);
                filter = filter.Filter;
            }

            return new Classes.TypologyDefinitionParameter() { Levels = levels };
        }
    }
}
