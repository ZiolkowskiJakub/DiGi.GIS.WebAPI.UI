using DiGi.GIS.Classes;

namespace DiGi.GIS.WebAPI.UI.Classes
{
    public class Building2DView
    {
        public Building2DView()
        {
        }

        public Building2DView(PostgreSQL.Classes.Building2DReference? building2DReference, Building2D? building2D, PostgreSQL.Classes.AdministrativeAreal2DReferencePath? administrativeAreal2DReferencePath)
        {
            Building2DReference = building2DReference;
            Building2D = building2D;
            AdministrativeAreal2DReferencePath = administrativeAreal2DReferencePath;
        }

        public PostgreSQL.Classes.AdministrativeAreal2DReferencePath? AdministrativeAreal2DReferencePath { get; }

        public Building2D? Building2D { get; }

        public PostgreSQL.Classes.Building2DReference? Building2DReference { get; }
    }
}