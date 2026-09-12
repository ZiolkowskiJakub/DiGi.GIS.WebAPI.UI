using DiGi.Core.Classes;
using DiGi.Core.Enums;
using DiGi.GIS.WebAPI.UI.Classes;
using DiGi.Typology.Interfaces;
using DiGi.Typology.Visual.Classes;
using System.Collections.Generic;

namespace DiGi.GIS.WebAPI.UI
{
    public static partial class Create
    {
        /// <summary>
        /// Creates the Typology definition document from the page state: the <see cref="Typology.Visual.Classes.VisualColumnTypologyFilter"/> chain the page describes, one level per selected column.
        /// <para>The state is checked by <see cref="Query.TypologyDefinitionErrors(TypologyDefinitionParameter, IEnumerable{DiGi.PostgreSQL.Table.Classes.Column})"/> first and nothing is built when it reports anything, so the document never carries a partial level. Each level's column becomes a <see cref="Core.IO.Table.Classes.Column"/> carrying the catalog name, index and CLR type - the document identifies it by the slug of that name (<c>Core.IO.Query.UniqueId</c>), which is the catalog's <c>UniqueId</c>; the index is the catalog's projection index and is not trusted by any reader. Colors are filed through the rule's appearance collection indexer, so the <c>"[min, max]"</c> and value keys are rendered by <c>DiGi.Typology.Visual.Query.Key</c> and never spelled here; a unique value is converted to the column's CLR type before it is keyed, so <c>2010</c> bound as a JSON number files under the same key as the stored value.</para>
        /// </summary>
        /// <param name="typologyDefinitionParameter">The page state. This value can be null.</param>
        /// <param name="columns">The live column catalog of the building data table. This value can be null.</param>
        /// <returns>The root of the chain, or <see langword="null"/> when the page state is invalid or the catalog is unavailable.</returns>
        public static Typology.Visual.Classes.VisualColumnTypologyFilter? VisualColumnTypologyFilter(this TypologyDefinitionParameter? typologyDefinitionParameter, IEnumerable<DiGi.PostgreSQL.Table.Classes.Column>? columns)
        {
            if (typologyDefinitionParameter?.Levels == null || Query.TypologyDefinitionErrors(typologyDefinitionParameter, columns).Count != 0)
            {
                return null;
            }

            Dictionary<string, DiGi.PostgreSQL.Table.Classes.Column>? dictionary = Query.ColumnDictionary(columns);
            if (dictionary == null)
            {
                return null;
            }

            Typology.Visual.Classes.VisualColumnTypologyFilter? result = null;

            for (int i = typologyDefinitionParameter.Levels.Count - 1; i >= 0; i--)
            {
                TypologyDefinitionLevelParameter level = typologyDefinitionParameter.Levels[i];
                DiGi.PostgreSQL.Table.Classes.Column column = dictionary[level.UniqueId!];
                DataType dataType = column.DataType ?? DataType.Undefined;

                ITypologyFilterRule? rule = null;
                switch (level.RuleType)
                {
                    case nameof(VisualIntegerRangeFilterRule):
                        {
                            List<Range<int>> ranges = [];
                            foreach (TypologyDefinitionRangeParameter range in level.Ranges!)
                            {
                                ranges.Add(new Range<int>((int)range.Min!.Value, (int)range.Max!.Value));
                            }

                            VisualIntegerRangeFilterRule visualIntegerRangeFilterRule = new(ranges);
                            for (int j = 0; j < ranges.Count; j++)
                            {
                                visualIntegerRangeFilterRule.TypologyAppearanceCollection[ranges[j]] = TypologyAppearance(Core.Convert.ToDiGi(Core.Convert.ToDrawing(level.Ranges[j].Color)));
                            }

                            rule = visualIntegerRangeFilterRule;
                        }
                        break;

                    case nameof(VisualDoubleRangeFilterRule):
                        {
                            List<Range<double>> ranges = [];
                            foreach (TypologyDefinitionRangeParameter range in level.Ranges!)
                            {
                                ranges.Add(new Range<double>(range.Min!.Value, range.Max!.Value));
                            }

                            VisualDoubleRangeFilterRule visualDoubleRangeFilterRule = new(ranges);
                            for (int j = 0; j < ranges.Count; j++)
                            {
                                visualDoubleRangeFilterRule.TypologyAppearanceCollection[ranges[j]] = TypologyAppearance(Core.Convert.ToDiGi(Core.Convert.ToDrawing(level.Ranges[j].Color)));
                            }

                            rule = visualDoubleRangeFilterRule;
                        }
                        break;

                    case nameof(VisualUniqueValueFilterRule):
                        {
                            VisualUniqueValueFilterRule visualUniqueValueFilterRule = new();
                            foreach (TypologyDefinitionUniqueValueParameter uniqueValue in level.UniqueValueColors!)
                            {
                                Query.TryConvertValue(uniqueValue.Value, dataType, out object? value);
                                visualUniqueValueFilterRule.TypologyAppearanceCollection[value] = TypologyAppearance(Core.Convert.ToDiGi(Core.Convert.ToDrawing(uniqueValue.Color)));
                            }

                            rule = visualUniqueValueFilterRule;
                        }
                        break;
                }

                result = new Typology.Visual.Classes.VisualColumnTypologyFilter()
                {
                    Value = new Core.IO.Table.Classes.Column(column.Index, column.Name, Core.Query.Type(dataType)),
                    Rule = rule,
                    Filter = result
                };
            }

            return result;
        }
    }
}
