using System.Collections.Generic;

namespace DiGi.GIS.WebAPI.UI.ViewModels
{
    /// <summary>
    /// The response of <c>POST /typology/buildings</c>: the solved Typology tree and the flat building list the view renders as dots.
    /// <para>The <see cref="Root"/> is the recursive tree the view renders as the typology panel: each node carries its name, description, color and children. The <see cref="Buildings"/> is the flat list of building entries, one per reference the solve classified, each filed under the deepest node holding it - a leaf bucket, or the bucket whose next level dropped the row; the view joins each entry to its dot position by <c>(Reference, CountyId)</c> against the area-scoped centroid endpoint.</para>
    /// </summary>
    public class TypologyBuildingsViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TypologyBuildingsViewModel"/> class.
        /// </summary>
        public TypologyBuildingsViewModel() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="TypologyBuildingsViewModel"/> class.
        /// </summary>
        /// <param name="root">The root of the solved typology tree, or null when the solve produced no nodes.</param>
        /// <param name="buildings">The flat list of building entries, one per classified reference, filed under the deepest node holding it.</param>
        public TypologyBuildingsViewModel(TypologyTreeNodeViewModel? root, List<TypologyBuildingViewModel> buildings)
        {
            Root = root;
            Buildings = buildings;
        }

        /// <summary>
        /// Gets the root of the solved typology tree.
        /// </summary>
        public TypologyTreeNodeViewModel? Root { get; set; }

        /// <summary>
        /// Gets the flat list of building entries, one per classified reference, filed under the deepest node holding it.
        /// </summary>
        public List<TypologyBuildingViewModel> Buildings { get; set; } = [];
    }
}
