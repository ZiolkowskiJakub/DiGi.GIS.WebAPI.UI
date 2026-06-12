using DiGi.Analytical.Building.HVAC.Interfaces;

namespace DiGi.GIS.WebAPI.UI.Classes
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

        /// <summary>
        /// Gets the year associated with the regulated heat transfer coefficients.
        /// </summary>
        /// <returns>The year as a <see cref="short"/>.</returns>
        public short Year { get; }

        /// <summary>
        /// Gets a value indicating whether the building is residential.
        /// </summary>
        /// <returns><c>true</c> if the building is residential; <c>false</c> otherwise; or <c>null</c> if the value is not specified.</returns>
        public bool? IsResidential { get; }

        /// <summary>
        /// Gets the regulated heat transfer coefficients.
        /// </summary>
        /// <returns>The <see cref="IRegulatedHeatTransferCoefficients"/> instance, or <c>null</c> if not specified.</returns>
        public IRegulatedHeatTransferCoefficients? RegulatedHeatTransferCoefficients { get; }
    }
}