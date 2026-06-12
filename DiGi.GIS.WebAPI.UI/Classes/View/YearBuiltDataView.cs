using DiGi.GIS.PostgreSQL.Classes;

namespace DiGi.GIS.WebAPI.UI.Classes
{
    /// <summary>
    /// Represents a view model that associates a 2D building reference with its corresponding year built data.
    /// </summary>
    public class YearBuiltDataView
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="YearBuiltDataView"/> class.
        /// </summary>
        public YearBuiltDataView()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="YearBuiltDataView"/> class with specified building reference and year built data.
        /// </summary>
        /// <param name="building2DReference">The reference to the 2D building.</param>
        /// <param name="yearBuiltData">The year built data associated with the building.</param>
        public YearBuiltDataView(Building2DReference? building2DReference, Interfaces.IYearBuiltData? yearBuiltData)
        {
            Building2DReference = building2DReference;
            YearBuiltData = yearBuiltData;
        }

        /// <summary>
        /// Gets the year built data associated with the building.
        /// </summary>
        public Interfaces.IYearBuiltData? YearBuiltData { get; }

        /// <summary>
        /// Gets the reference to the 2D building.
        /// </summary>
        public Building2DReference? Building2DReference { get; }
    }
}