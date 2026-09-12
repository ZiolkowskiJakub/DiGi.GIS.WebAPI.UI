using DiGi.Core.Classes;
using DiGi.Core.Enums;
using DiGi.GIS.WebAPI.UI.Classes;
using DiGi.Typology.Visual.Classes;
using System.Collections.Generic;
using System.Globalization;

namespace DiGi.GIS.WebAPI.UI
{
    public static partial class Query
    {
        /// <summary>
        /// Lists everything that stops a Typology definition page state from becoming a document.
        /// <para>Every level must name a column of <paramref name="columns"/> exactly once and carry a rule type the column admits: <c>VisualIntegerRangeFilterRule</c> needs an integer column, <c>VisualDoubleRangeFilterRule</c> a floating point one (the split <c>typology.js</c> makes), <c>VisualUniqueValueFilterRule</c> any column. Range rows need both bounds (whole numbers on an integer rule), an ascending order that does not overlap - closed intervals keyed by their minimum, so two rows sharing a minimum would silently replace each other in the rule - and a parseable color; unique-value rows need a value the column type admits, listed once, and a parseable color. The rule kind is never inferred from the column: a numeric column may be classified by unique value.</para>
        /// <para>Messages name the level and the row so the page can show them as they are.</para>
        /// </summary>
        /// <param name="typologyDefinitionParameter">The page state to check. This value can be null.</param>
        /// <param name="columns">The live column catalog of the building data table. This value can be null.</param>
        /// <returns>The errors, empty when the page state is valid.</returns>
        public static List<string> TypologyDefinitionErrors(this TypologyDefinitionParameter? typologyDefinitionParameter, IEnumerable<DiGi.PostgreSQL.Table.Classes.Column>? columns)
        {
            List<string> errors = [];

            if (typologyDefinitionParameter?.Levels == null || typologyDefinitionParameter.Levels.Count == 0)
            {
                errors.Add("The definition holds no levels.");
                return errors;
            }

            Dictionary<string, DiGi.PostgreSQL.Table.Classes.Column>? dictionary = ColumnDictionary(columns);
            if (dictionary == null)
            {
                errors.Add("The building data column catalog is unavailable.");
                return errors;
            }

            HashSet<string> uniqueIds = [];

            for (int i = 0; i < typologyDefinitionParameter.Levels.Count; i++)
            {
                TypologyDefinitionLevelParameter? level = typologyDefinitionParameter.Levels[i];
                string prefix = string.Format(CultureInfo.InvariantCulture, "Level {0} ({1}): ", i + 1, level?.UniqueId ?? "no column");

                if (level == null || string.IsNullOrWhiteSpace(level.UniqueId))
                {
                    errors.Add(prefix + "no column selected.");
                    continue;
                }

                if (!dictionary.TryGetValue(level.UniqueId, out DiGi.PostgreSQL.Table.Classes.Column? column))
                {
                    errors.Add(prefix + "not a building data column.");
                    continue;
                }

                if (!uniqueIds.Add(level.UniqueId))
                {
                    errors.Add(prefix + "the column is used by an earlier level.");
                }

                DataType dataType = column.DataType ?? DataType.Undefined;
                bool isNumeric = Core.Query.IsNumeric(dataType, out bool isInteger);

                switch (level.RuleType)
                {
                    case nameof(VisualIntegerRangeFilterRule):
                        if (!isNumeric || !isInteger)
                        {
                            errors.Add(prefix + string.Format(CultureInfo.InvariantCulture, "{0} needs an integer column, the column is {1}.", nameof(VisualIntegerRangeFilterRule), dataType));
                        }

                        RangeErrors(level, true, prefix, errors);
                        break;

                    case nameof(VisualDoubleRangeFilterRule):
                        if (!isNumeric || isInteger)
                        {
                            errors.Add(prefix + string.Format(CultureInfo.InvariantCulture, "{0} needs a floating point column, the column is {1}.", nameof(VisualDoubleRangeFilterRule), dataType));
                        }

                        RangeErrors(level, false, prefix, errors);
                        break;

                    case nameof(VisualUniqueValueFilterRule):
                        UniqueValueErrors(level, dataType, prefix, errors);
                        break;

                    case null:
                    case "":
                        errors.Add(prefix + "no rule type chosen.");
                        break;

                    default:
                        errors.Add(prefix + string.Format(CultureInfo.InvariantCulture, "unknown rule type '{0}'.", level.RuleType));
                        break;
                }
            }

            return errors;

            static void RangeErrors(TypologyDefinitionLevelParameter level, bool integer, string prefix, List<string> errors)
            {
                if (level.Ranges == null || level.Ranges.Count == 0)
                {
                    errors.Add(prefix + "a range rule needs at least one range.");
                    return;
                }

                List<TypologyDefinitionRangeParameter> ranges = [];

                for (int i = 0; i < level.Ranges.Count; i++)
                {
                    TypologyDefinitionRangeParameter? range = level.Ranges[i];
                    string prefix_Range = prefix + string.Format(CultureInfo.InvariantCulture, "range {0}: ", i + 1);

                    if (range?.Min == null || range.Max == null)
                    {
                        errors.Add(prefix_Range + "both bounds are required.");
                        continue;
                    }

                    if (integer && (range.Min.Value != System.Math.Floor(range.Min.Value) || range.Max.Value != System.Math.Floor(range.Max.Value) || range.Min.Value < int.MinValue || range.Max.Value > int.MaxValue))
                    {
                        errors.Add(prefix_Range + "an integer rule needs whole number bounds.");
                        continue;
                    }

                    if (range.Min.Value > range.Max.Value)
                    {
                        errors.Add(prefix_Range + "the minimum exceeds the maximum.");
                        continue;
                    }

                    if (Core.Convert.ToDrawing(range.Color).IsEmpty)
                    {
                        errors.Add(prefix_Range + string.Format(CultureInfo.InvariantCulture, "'{0}' is not a color.", range.Color));
                    }

                    ranges.Add(range);
                }

                ranges.Sort((x, y) => x.Min!.Value.CompareTo(y.Min!.Value));

                for (int i = 1; i < ranges.Count; i++)
                {
                    if (ranges[i - 1].Min!.Value == ranges[i].Min!.Value)
                    {
                        errors.Add(prefix + string.Format(CultureInfo.InvariantCulture, "two ranges start at {0}.", Typology.Visual.Query.Key(ranges[i].Min!.Value)));
                    }
                    else if (ranges[i - 1].Max!.Value >= ranges[i].Min!.Value)
                    {
                        errors.Add(prefix + string.Format(CultureInfo.InvariantCulture, "ranges {0} and {1} overlap.", RangeKey(ranges[i - 1]), RangeKey(ranges[i])));
                    }
                }
            }

            static void UniqueValueErrors(TypologyDefinitionLevelParameter level, DataType dataType, string prefix, List<string> errors)
            {
                if (level.UniqueValueColors == null || level.UniqueValueColors.Count == 0)
                {
                    errors.Add(prefix + "a unique value rule needs at least one value.");
                    return;
                }

                HashSet<string> keys = [];

                for (int i = 0; i < level.UniqueValueColors.Count; i++)
                {
                    TypologyDefinitionUniqueValueParameter? uniqueValue = level.UniqueValueColors[i];
                    string prefix_Value = prefix + string.Format(CultureInfo.InvariantCulture, "value {0}: ", i + 1);

                    if (uniqueValue == null || !TryConvertValue(uniqueValue.Value, dataType, out object? value))
                    {
                        errors.Add(prefix_Value + string.Format(CultureInfo.InvariantCulture, "the value is not a {0}.", dataType));
                        continue;
                    }

                    if (!keys.Add(Typology.Visual.Query.Key(value)))
                    {
                        errors.Add(prefix_Value + string.Format(CultureInfo.InvariantCulture, "'{0}' is listed twice.", Typology.Visual.Query.Key(value)));
                    }

                    if (Core.Convert.ToDrawing(uniqueValue.Color).IsEmpty)
                    {
                        errors.Add(prefix_Value + string.Format(CultureInfo.InvariantCulture, "'{0}' is not a color.", uniqueValue.Color));
                    }
                }
            }

            static string RangeKey(TypologyDefinitionRangeParameter range)
            {
                return Typology.Visual.Query.Key(new Range<double>(range.Min!.Value, range.Max!.Value));
            }
        }

        /// <summary>
        /// Lists everything that stops a Typology definition document from being loaded into the page.
        /// <para>Beyond the checks of the page state overload, a document may carry what the page cannot show: a level whose column is not in <paramref name="columns"/> (resolved by <c>Core.IO.Query.UniqueId</c>, the slug of the column name, exactly as the solver resolves it), a rule class the page has no editor for, a range without a color, an appearance filed under a key that matches no range (the orphan a client spelling the key itself would produce), an appearance carrying no color, or a unique-value key the column type cannot hold. The rule keeps its ranges sorted by minimum but rejects no overlap, so the overlap check is repeated here. A chain linking back on itself is refused.</para>
        /// </summary>
        /// <param name="visualColumnTypologyFilter">The root of the document chain. This value can be null.</param>
        /// <param name="columns">The live column catalog of the building data table. This value can be null.</param>
        /// <returns>The errors, empty when the document can be shown by the page.</returns>
        public static List<string> TypologyDefinitionErrors(this VisualColumnTypologyFilter<Core.IO.Table.Classes.Column>? visualColumnTypologyFilter, IEnumerable<DiGi.PostgreSQL.Table.Classes.Column>? columns)
        {
            List<string> errors = [];

            if (visualColumnTypologyFilter == null)
            {
                errors.Add("The definition holds no levels.");
                return errors;
            }

            Dictionary<string, DiGi.PostgreSQL.Table.Classes.Column>? dictionary = ColumnDictionary(columns);
            if (dictionary == null)
            {
                errors.Add("The building data column catalog is unavailable.");
                return errors;
            }

            HashSet<VisualColumnTypologyFilter<Core.IO.Table.Classes.Column>> visited = [];
            HashSet<string> uniqueIds = [];

            VisualColumnTypologyFilter<Core.IO.Table.Classes.Column>? filter = visualColumnTypologyFilter;
            int index = 0;

            while (filter != null)
            {
                index++;

                if (!visited.Add(filter))
                {
                    errors.Add(string.Format(CultureInfo.InvariantCulture, "Level {0}: the chain links back on itself.", index));
                    break;
                }

                string? uniqueId = Core.IO.Query.UniqueId(filter.Value);
                string prefix = string.Format(CultureInfo.InvariantCulture, "Level {0} ({1}): ", index, uniqueId ?? "no column");

                if (uniqueId == null)
                {
                    errors.Add(prefix + "no column.");
                    filter = filter.Filter;
                    continue;
                }

                if (!dictionary.TryGetValue(uniqueId, out DiGi.PostgreSQL.Table.Classes.Column? column))
                {
                    errors.Add(prefix + "not a building data column.");
                    filter = filter.Filter;
                    continue;
                }

                if (!uniqueIds.Add(uniqueId))
                {
                    errors.Add(prefix + "the column is used by an earlier level.");
                }

                DataType dataType = column.DataType ?? DataType.Undefined;
                bool isNumeric = Core.Query.IsNumeric(dataType, out bool isInteger);

                switch (filter.Rule)
                {
                    case VisualIntegerRangeFilterRule visualIntegerRangeFilterRule:
                        if (!isNumeric || !isInteger)
                        {
                            errors.Add(prefix + string.Format(CultureInfo.InvariantCulture, "{0} needs an integer column, the column is {1}.", nameof(VisualIntegerRangeFilterRule), dataType));
                        }

                        RangeErrors(visualIntegerRangeFilterRule.Ranges, visualIntegerRangeFilterRule.TypologyAppearanceCollection, prefix, errors);
                        break;

                    case VisualDoubleRangeFilterRule visualDoubleRangeFilterRule:
                        if (!isNumeric || isInteger)
                        {
                            errors.Add(prefix + string.Format(CultureInfo.InvariantCulture, "{0} needs a floating point column, the column is {1}.", nameof(VisualDoubleRangeFilterRule), dataType));
                        }

                        RangeErrors(visualDoubleRangeFilterRule.Ranges, visualDoubleRangeFilterRule.TypologyAppearanceCollection, prefix, errors);
                        break;

                    case VisualUniqueValueFilterRule visualUniqueValueFilterRule:
                        UniqueValueErrors(visualUniqueValueFilterRule.TypologyAppearanceCollection, dataType, prefix, errors);
                        break;

                    case null:
                        errors.Add(prefix + "no rule.");
                        break;

                    default:
                        errors.Add(prefix + string.Format(CultureInfo.InvariantCulture, "the page has no editor for a {0}.", filter.Rule.GetType().Name));
                        break;
                }

                filter = filter.Filter;
            }

            return errors;

            static void RangeErrors<T>(IEnumerable<Range<T>>? ranges, TypologyAppearanceCollection typologyAppearanceCollection, string prefix, List<string> errors) where T : System.IComparable<T>
            {
                List<Range<T>> ranges_Temp = ranges == null ? [] : [.. ranges];
                if (ranges_Temp.Count == 0)
                {
                    errors.Add(prefix + "a range rule needs at least one range.");
                    return;
                }

                HashSet<string> keys = [];

                for (int i = 0; i < ranges_Temp.Count; i++)
                {
                    Range<T> range = ranges_Temp[i];
                    string key = Typology.Visual.Query.Key(range);
                    keys.Add(key);

                    if (i > 0 && ranges_Temp[i - 1].Max.CompareTo(range.Min) >= 0)
                    {
                        errors.Add(prefix + string.Format(CultureInfo.InvariantCulture, "ranges {0} and {1} overlap.", Typology.Visual.Query.Key(ranges_Temp[i - 1]), key));
                    }

                    TypologyAppearance? typologyAppearance = typologyAppearanceCollection[range];
                    if (typologyAppearance == null)
                    {
                        errors.Add(prefix + string.Format(CultureInfo.InvariantCulture, "range {0} has no color.", key));
                    }
                    else if (Color(typologyAppearance) == null)
                    {
                        errors.Add(prefix + string.Format(CultureInfo.InvariantCulture, "the appearance of range {0} carries no color.", key));
                    }
                }

                foreach (string key in typologyAppearanceCollection.Keys)
                {
                    if (!keys.Contains(key))
                    {
                        errors.Add(prefix + string.Format(CultureInfo.InvariantCulture, "the color filed under '{0}' matches no range.", key));
                    }
                }
            }

            static void UniqueValueErrors(TypologyAppearanceCollection typologyAppearanceCollection, DataType dataType, string prefix, List<string> errors)
            {
                if (typologyAppearanceCollection.Count == 0)
                {
                    errors.Add(prefix + "a unique value rule needs at least one value.");
                    return;
                }

                foreach (string key in typologyAppearanceCollection.Keys)
                {
                    if (key != Core.Constants.UniqueId.Null && !Core.Query.TryConvert(key, out _, dataType))
                    {
                        errors.Add(prefix + string.Format(CultureInfo.InvariantCulture, "the value '{0}' is not a {1}.", key, dataType));
                    }

                    if (Color(typologyAppearanceCollection[key]) == null)
                    {
                        errors.Add(prefix + string.Format(CultureInfo.InvariantCulture, "the appearance of value '{0}' carries no color.", key));
                    }
                }
            }
        }
    }
}
