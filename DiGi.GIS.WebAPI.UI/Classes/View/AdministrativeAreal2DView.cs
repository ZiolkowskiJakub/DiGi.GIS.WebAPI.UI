using DiGi.GIS.Classes;
using System.Collections.Generic;

namespace DiGi.GIS.WebAPI.UI.Classes
{
    public class AdministrativeAreal2DView
    {
        public AdministrativeAreal2DView()
        {
        }

        public AdministrativeAreal2DView(PostgreSQL.Classes.AdministrativeAreal2DReference? administrativeAreal2DReference, AdministrativeAreal2D? administrativeAreal2D, PostgreSQL.Classes.AdministrativeAreal2DReferencePath? administrativeAreal2DReferencePath, IEnumerable<PostgreSQL.Classes.AdministrativeAreal2DReference>? administrativeAreal2DReferences)
        {
            AdministrativeAreal2DReference = administrativeAreal2DReference;
            AdministrativeAreal2D = administrativeAreal2D;
            AdministrativeAreal2DReferencePath = administrativeAreal2DReferencePath;
            AdministrativeAreal2DReferences = administrativeAreal2DReferences != null ? [.. administrativeAreal2DReferences] : [];
        }

        public PostgreSQL.Classes.AdministrativeAreal2DReference? AdministrativeAreal2DReference { get; }

        public AdministrativeAreal2D? AdministrativeAreal2D { get; }

        public PostgreSQL.Classes.AdministrativeAreal2DReferencePath? AdministrativeAreal2DReferencePath { get; }

        public List<PostgreSQL.Classes.AdministrativeAreal2DReference>? AdministrativeAreal2DReferences { get; }
    }
}