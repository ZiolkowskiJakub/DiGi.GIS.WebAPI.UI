using DiGi.GIS.Classes;
using DiGi.GIS.PostgreSQL.Classes;
using System.Collections.Generic;

namespace DiGi.GIS.WebAPI.UI.Classes
{
    public class YearBuiltsView
    {
        public YearBuiltsView()
        {
        }

        public YearBuiltsView(Building2DReference? building2DReference, IEnumerable<Interfaces.IYearBuilt>? yearBuilts)
        {
            Building2DReference = building2DReference;
            YearBuilts = yearBuilts is null ? [] : [.. yearBuilts];
        }

        public List<Interfaces.IYearBuilt>? YearBuilts { get; }

        public Building2DReference? Building2DReference { get; }
    }
}