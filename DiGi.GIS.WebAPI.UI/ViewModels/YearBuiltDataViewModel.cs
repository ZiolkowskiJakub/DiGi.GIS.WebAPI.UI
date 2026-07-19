using DiGi.GIS.PostgreSQL.Classes;

namespace DiGi.GIS.WebAPI.UI.ViewModels
{
    /// <summary>
    /// Represents a view model that associates a 2D building reference with its corresponding year built data.
    /// </summary>
    public class YearBuiltDataViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="YearBuiltDataViewModel"/> class.
        /// </summary>
        public YearBuiltDataViewModel()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="YearBuiltDataViewModel"/> class with specified building reference and year built data.
        /// </summary>
        /// <param name="building2DReference">The reference to the 2D building.</param>
        /// <param name="yearBuiltData">The year built data associated with the building.</param>
        public YearBuiltDataViewModel(Building2DReference? building2DReference, DiGi.GIS.Interfaces.IYearBuiltData? yearBuiltData)
        {
            Building2DReference = building2DReference;
            YearBuiltData = yearBuiltData;
        }

        /// <summary>
        /// Gets the year built data associated with the building.
        /// </summary>
        public DiGi.GIS.Interfaces.IYearBuiltData? YearBuiltData { get; }

        /// <summary>
        /// Gets the reference to the 2D building.
        /// </summary>
        public Building2DReference? Building2DReference { get; }
    }
}