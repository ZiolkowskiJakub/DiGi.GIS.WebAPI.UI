using DiGi.GIS.Classes;

namespace DiGi.GIS.WebAPI.UI.ViewModels
{
    /// <summary>
    /// Represents a 2D view of a building, providing access to its reference, spatial data, and administrative areal path.
    /// </summary>
    public class Building2DView
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Building2DView"/> class.
        /// </summary>
        public Building2DView()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Building2DView"/> class.
        /// </summary>
        /// <param name="building2DReference">The <see cref="PostgreSQL.Classes.Building2DReference"/> for the building.</param>
        /// <param name="building2D">The <see cref="Building2D"/> associated with this view.</param>
        /// <param name="administrativeAreal2DReferencePath">The <see cref="PostgreSQL.Classes.AdministrativeAreal2DReferencePath"/> for the building.</param>
        public Building2DView(PostgreSQL.Classes.Building2DReference? building2DReference, Building2D? building2D, PostgreSQL.Classes.AdministrativeAreal2DReferencePath? administrativeAreal2DReferencePath)
        {
            Building2DReference = building2DReference;
            Building2D = building2D;
            AdministrativeAreal2DReferencePath = administrativeAreal2DReferencePath;
        }

        /// <summary> Gets the collection of administrative 2D area references for this building view. </summary>

        public PostgreSQL.Classes.AdministrativeAreal2DReferencePath? AdministrativeAreal2DReferencePath { get; }

        /// <summary> Gets the 2D building associated with this view. </summary>

        public Building2D? Building2D { get; }

        /// <summary> Gets the reference to the 2D building associated with this view. </summary>

        public PostgreSQL.Classes.Building2DReference? Building2DReference { get; }
    }
}