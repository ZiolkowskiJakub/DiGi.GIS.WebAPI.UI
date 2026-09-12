using System.Collections.Generic;

namespace DiGi.GIS.WebAPI.UI.Classes
{
    /// <summary>
    /// One level of the Typology definition page state: a selected building-data column, its rule type and the rows of the active rule editor.
    /// <para><see cref="Name"/>, <see cref="DataType"/> and <see cref="IsNumeric"/> describe the column as listed by the deployed GIS Web API; the server fills them from the live column catalog on import and ignores them on export, where the column is identified by <see cref="UniqueId"/> alone.</para>
    /// </summary>
    public class TypologyDefinitionLevelParameter
    {
        /// <summary>
        /// Gets or sets the unique identifier (slug) of the column, as addressed by the deployed GIS Web API.
        /// </summary>
        public string? UniqueId { get; set; }

        /// <summary>
        /// Gets or sets the display name of the column.
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// Gets or sets the integer <see cref="Core.Enums.DataType"/> value of the column (0 Undefined through 14 String).
        /// </summary>
        public int DataType { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the column is numeric and eligible for a range rule.
        /// </summary>
        public bool IsNumeric { get; set; }

        /// <summary>
        /// Gets or sets the rule class name: <c>VisualIntegerRangeFilterRule</c>, <c>VisualDoubleRangeFilterRule</c> or <c>VisualUniqueValueFilterRule</c>. Null when no rule has been chosen yet.
        /// </summary>
        public string? RuleType { get; set; }

        /// <summary>
        /// Gets or sets the range rows, read when <see cref="RuleType"/> names a range rule.
        /// </summary>
        public List<TypologyDefinitionRangeParameter>? Ranges { get; set; }

        /// <summary>
        /// Gets or sets the unique-value rows, read when <see cref="RuleType"/> names the unique-value rule.
        /// </summary>
        public List<TypologyDefinitionUniqueValueParameter>? UniqueValueColors { get; set; }
    }
}
