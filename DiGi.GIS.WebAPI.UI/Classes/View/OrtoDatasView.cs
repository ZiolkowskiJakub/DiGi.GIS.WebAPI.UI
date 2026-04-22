using DiGi.GIS.PostgreSQL.Classes;

namespace DiGi.GIS.WebAPI.UI.Classes
{
    public class OrtoDatasView
    {
        public OrtoDatasView()
        {
        }

        public OrtoDatasView(Building2DReference? building2DReference, GIS.Classes.OrtoDatas? ortoDatas)
        {
            Building2DReference = building2DReference;
            OrtoDatas = ortoDatas;
        }

        public GIS.Classes.OrtoDatas? OrtoDatas { get; }

        public Building2DReference? Building2DReference { get; }
    }
}