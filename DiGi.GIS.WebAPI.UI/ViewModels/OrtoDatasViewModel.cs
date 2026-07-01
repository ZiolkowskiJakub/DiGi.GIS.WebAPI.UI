using DiGi.GIS.PostgreSQL.Classes;

namespace DiGi.GIS.WebAPI.UI.ViewModels
{
    /// <summary>
    /// Represents a view model that combines orthographic data and a 2D building reference.
    /// </summary>
    public class OrtoDatasViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OrtoDatasViewModel"/> class.
        /// </summary>
        public OrtoDatasViewModel()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OrtoDatasViewModel"/> class with the specified building reference and orthographic data.
        /// </summary>
        /// <param name="building2DReference">The reference to the 2D building.</param>
        /// <param name="ortoDatas">The orthographic data associated with the view model.</param>
        public OrtoDatasViewModel(Building2DReference? building2DReference, GIS.Classes.OrtoDatas? ortoDatas)
        {
            Building2DReference = building2DReference;
            OrtoDatas = ortoDatas;
        }

        /// <summary>
        /// Gets the orthographic data associated with this view.
        /// </summary>
        public GIS.Classes.OrtoDatas? OrtoDatas { get; }

        /// <summary>
        /// Gets the reference to the 2D building associated with this view.
        /// </summary>
        public Building2DReference? Building2DReference { get; }
    }
}