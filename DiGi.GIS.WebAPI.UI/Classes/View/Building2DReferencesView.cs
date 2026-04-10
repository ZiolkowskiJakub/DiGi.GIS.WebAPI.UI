using DiGi.GIS.PostgreSQL.Classes;
using System.Collections.Generic;
using System.Linq;

namespace DiGi.GIS.WebAPI.UI.Classes
{
    public class Building2DReferencesView
    {
        public Building2DReferencesView()
        {
        }

        public Building2DReferencesView(IEnumerable<Building2DReference>? building2DReferences)
        {
            Building2DReferences = building2DReferences?.ToList();
        }

        public List<Building2DReference>? Building2DReferences { get; }
    }
}