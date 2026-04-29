using DiGi.Analytical.Building.HVAC.Interfaces;

namespace DiGi.GIS.WebAPI.UI.Classes
{
    public class RegulatedHeatTransferCoefficientsView
    {
        public RegulatedHeatTransferCoefficientsView()
        {
        }

        public RegulatedHeatTransferCoefficientsView(short year, IRegulatedHeatTransferCoefficients? regulatedHeatTransferCoefficients, bool? isResidential)
        {
            Year = year;
            RegulatedHeatTransferCoefficients = regulatedHeatTransferCoefficients;
            IsResidential = isResidential;
        }

        public short Year{ get; }

        public bool? IsResidential { get; }

        public IRegulatedHeatTransferCoefficients? RegulatedHeatTransferCoefficients { get; }
    }
}