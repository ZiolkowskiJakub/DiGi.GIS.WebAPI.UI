using DiGi.Analytical.Building.HVAC.Interfaces;

namespace DiGi.GIS.WebAPI.UI.ViewModels
{
    /// <summary>
    /// Represents a view of the regulated heat transfer coefficients.
    /// </summary>
    public class RegulatedHeatTransferCoefficientsView
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RegulatedHeatTransferCoefficientsView"/> class.
        /// </summary>
        public RegulatedHeatTransferCoefficientsView()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RegulatedHeatTransferCoefficientsView"/> class.
        /// </summary>
        /// <param name="year">The year associated with the regulated heat transfer coefficients.</param>
        /// <param name="regulatedHeatTransferCoefficients">The regulated heat transfer coefficients.</param>
        /// <param name="isResidential">A value indicating whether the building is residential.</param>
        public RegulatedHeatTransferCoefficientsView(short year, IRegulatedHeatTransferCoefficients? regulatedHeatTransferCoefficients, bool? isResidential)
        {
            Year = year;
            RegulatedHeatTransferCoefficients = regulatedHeatTransferCoefficients;
            IsResidential = isResidential;
        }

        /// <summary> Gets the year associated with the regulated heat transfer coefficients. </summary>

        public short Year { get; }

        /// <summary> Gets a value indicating whether the building is residential. </summary>

        public bool? IsResidential { get; }

        /// <summary> Gets the regulated heat transfer coefficients. </summary>

        public IRegulatedHeatTransferCoefficients? RegulatedHeatTransferCoefficients { get; }
    }
}