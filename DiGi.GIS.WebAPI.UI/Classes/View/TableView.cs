namespace DiGi.GIS.WebAPI.UI.Classes
{
    public class TableView
    {
        public TableView()
        {
        }

        public TableView(DiGi.PostgreSQL.Table.Classes.Table? table)
        {
            Table = table;
        }

        public DiGi.PostgreSQL.Table.Classes.Table? Table { get; }
    }
}