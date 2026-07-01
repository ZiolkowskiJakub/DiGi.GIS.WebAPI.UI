namespace DiGi.GIS.WebAPI.UI.ViewModels
{
    /// <summary>
    /// Represents a view for a PostgreSQL table, providing access to the underlying table data and structure.
    /// </summary>
    public class TableView
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TableView"/> class.
        /// </summary>
        public TableView()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TableView"/> class.
        /// </summary>
        /// <param name="table">The PostgreSQL table to be associated with this view.</param>
        public TableView(DiGi.PostgreSQL.Table.Classes.Table? table)
        {
            Table = table;
        }

        /// <summary>
        /// Gets the table associated with the table view.
        /// </summary>
        public DiGi.PostgreSQL.Table.Classes.Table? Table { get; }
    }
}