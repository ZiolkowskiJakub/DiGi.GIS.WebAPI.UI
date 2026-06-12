using DiGi.GIS.PostgreSQL.Classes;

namespace DiGi.GIS.WebAPI.UI.Classes
{
    /// <summary>
    /// Represents a view that provides combined access to <see cref="Building2DReference"/> and <see cref="Interfaces.IOccupancyData"/>.
    /// </summary>
    public class Building2DOccupancyDataView
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Building2DOccupancyDataView"/> class.
        /// </summary>
        public Building2DOccupancyDataView()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Building2DOccupancyDataView"/> class.
        /// </summary>
        /// <param name="building2DReference">The <see cref="DiGi.GIS.PostgreSQL.Classes.Building2DReference"/> reference to the 2D building, or <c>null</c>.</param>
        /// <param name="occupancyData">The <see cref="DiGi.GIS.Interfaces.IOccupancyData"/> containing occupancy data, or <c>null</c>.</param>
        public Building2DOccupancyDataView(Building2DReference? building2DReference, Interfaces.IOccupancyData? occupancyData)
        {
            Building2DReference = building2DReference;
            OccupancyData = occupancyData;
        }

        /// <summary>
        /// Gets the occupancy data associated with this building 2D occupancy data view.
        /// </summary>
        /// <returns>The <see cref="DiGi.GIS.Interfaces.IOccupancyData"/> for the occupancy, or <c>null</c> if not specified.</returns>
        public Interfaces.IOccupancyData? OccupancyData { get; }

        /// <summary>
        /// Gets the reference to the 2D building associated with this occupancy data view.
        /// </summary>
        /// <returns>The <see cref="DiGi.GIS.PostgreSQL.Classes.Building2DReference"/> for the building, or <c>null</c> if not specified.</returns>
        public Building2DReference? Building2DReference { get; }
    }
}