namespace DiGi.GIS.WebAPI.UI.ViewModels
{
    /// <summary>
    /// Represents a view model for a PostgreSQL table, providing access to the underlying table data and structure.
    /// </summary>
    public class TableViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TableViewModel"/> class.
        /// </summary>
        public TableViewModel()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TableViewModel"/> class.
        /// </summary>
        /// <param name="table">The PostgreSQL table to be associated with this view model.</param>
        public TableViewModel(DiGi.PostgreSQL.Table.Classes.Table? table)
        {
            Table = table;
        }

        /// <summary>
        /// Gets the table associated with the table view.
        /// </summary>
        public DiGi.PostgreSQL.Table.Classes.Table? Table { get; }
    }
}