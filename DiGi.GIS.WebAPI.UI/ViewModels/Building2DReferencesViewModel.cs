using DiGi.GIS.PostgreSQL.Classes;
using System.Collections.Generic;
using System.Linq;

namespace DiGi.GIS.WebAPI.UI.ViewModels
{
    /// <summary>
    /// Represents a view model containing a collection of 2D building references.
    /// </summary>
    public class Building2DReferencesViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Building2DReferencesViewModel"/> class.
        /// </summary>
        public Building2DReferencesViewModel()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Building2DReferencesViewModel"/> class with a specified collection of 2D building references.
        /// </summary>
        /// <param name="building2DReferences">The collection of <see cref="Building2DReference"/> objects to initialize the view model with.</param>
        public Building2DReferencesViewModel(IEnumerable<Building2DReference>? building2DReferences)
        {
            Building2DReferences = building2DReferences?.ToList();
        }

        /// <summary>
        /// Gets the list of 2D building references associated with this view.
        /// </summary>
        public List<Building2DReference>? Building2DReferences { get; }
    }
}