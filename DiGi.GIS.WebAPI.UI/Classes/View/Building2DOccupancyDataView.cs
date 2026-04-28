using DiGi.GIS.PostgreSQL.Classes;

namespace DiGi.GIS.WebAPI.UI.Classes
{
    public class Building2DOccupancyDataView
    {
        public Building2DOccupancyDataView()
        {
        }

        public Building2DOccupancyDataView(Building2DReference? building2DReference, Interfaces.IOccupancyData? occupancyData)
        {
            Building2DReference = building2DReference;
            OccupancyData = occupancyData;
        }

        public Interfaces.IOccupancyData? OccupancyData { get; }

        public Building2DReference? Building2DReference { get; }
    }
}