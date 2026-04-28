using DiGi.Analytical.Building.HVAC.Interfaces;

namespace DiGi.GIS.WebAPI.UI.Classes
{
    public class RegulatedHeatTransferCoefficientsView
    {
        public RegulatedHeatTransferCoefficientsView()
        {
        }

        public RegulatedHeatTransferCoefficientsView(short year, IRegulatedHeatTransferCoefficients? regulatedHeatTransferCoefficients)
        {
            Year = year;
            RegulatedHeatTransferCoefficients = regulatedHeatTransferCoefficients;
        }

        public short Year{ get; }

        public IRegulatedHeatTransferCoefficients? RegulatedHeatTransferCoefficients { get; }
    }
}