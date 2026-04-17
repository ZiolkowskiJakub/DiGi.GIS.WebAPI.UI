using DiGi.GIS.PostgreSQL.Classes;

namespace DiGi.GIS.WebAPI.UI.Classes
{
    public class YearBuiltDataView
    {
        public YearBuiltDataView()
        {
        }

        public YearBuiltDataView(Building2DReference? building2DReference, Interfaces.IYearBuiltData? yearBuiltData)
        {
            Building2DReference = building2DReference;
            YearBuiltData = yearBuiltData;
        }

        public Interfaces.IYearBuiltData? YearBuiltData { get; }

        public Building2DReference? Building2DReference { get; }
    }
}