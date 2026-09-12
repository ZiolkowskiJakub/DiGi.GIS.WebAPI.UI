using DiGi.PostgreSQL.Table.Classes;

namespace DiGi.GIS.WebAPI.UI.ViewModels
{
    /// <summary>
    /// Represents a single building-data column available for typology grouping, as listed in the Available Columns section.
    /// </summary>
    public class TypologyColumnViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TypologyColumnViewModel"/> class.
        /// </summary>
        public TypologyColumnViewModel() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="TypologyColumnViewModel"/> class from a deployed GIS Web API <see cref="Column"/>.
        /// </summary>
        /// <param name="column">The source column.</param>
        public TypologyColumnViewModel(Column column)
        {
            Name = column.Name;
            UniqueId = column.UniqueId;
            Category = column.Category;
            Description = column.Description;
            DataType = column.DataType.HasValue ? (int)column.DataType.Value : 0;
            IsNumeric = column.DataType.HasValue && column.DataType.Value >= Core.Enums.DataType.SByte && column.DataType.Value <= Core.Enums.DataType.Decimal;
        }

        /// <summary>
        /// Gets the display name of the column.
        /// </summary>
        public string? Name { get; }

        /// <summary>
        /// Gets the unique identifier (slug) of the column, as addressed by the deployed GIS Web API.
        /// </summary>
        public string? UniqueId { get; }

        /// <summary>
        /// Gets the category grouping of the column.
        /// </summary>
        public string? Category { get; }

        /// <summary>
        /// Gets the description of the column.
        /// </summary>
        public string? Description { get; }

        /// <summary>
        /// Gets the integer data type value (0 Undefined through 14 String).
        /// </summary>
        public int DataType { get; }

        /// <summary>
        /// Gets a value indicating whether the column is numeric and eligible for a Range rule.
        /// </summary>
        public bool IsNumeric { get; }
    }
}
