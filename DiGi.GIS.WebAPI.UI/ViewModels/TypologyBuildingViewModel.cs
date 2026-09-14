using System.Collections.Generic;

namespace DiGi.GIS.WebAPI.UI.ViewModels
{
    /// <summary>
    /// One building entry of the Typology area view: the reference that identifies it in the GIS Web API, the county part it was fetched from, and the path of the typology bucket it was solved into.
    /// <para>Coordinates are intentionally absent: the dot positions come from the area-scoped centroid endpoint in <c>DiGi.GIS.WebAPI</c>, joined in the view by <c>(Reference, CountyId)</c>. The <see cref="Path"/> is the filing index chain of the typology bucket, one integer per level, which the view uses to locate the node in the tree for the centroid join.</para>
    /// </summary>
    public class TypologyBuildingViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TypologyBuildingViewModel"/> class.
        /// </summary>
        public TypologyBuildingViewModel() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="TypologyBuildingViewModel"/> class.
        /// </summary>
        /// <param name="reference">The building reference key.</param>
        /// <param name="countyId">The county part identifier the building was fetched from.</param>
        /// <param name="path">The typology path, one filing index per level.</param>
        public TypologyBuildingViewModel(string reference, int countyId, List<int> path)
        {
            Reference = reference;
            CountyId = countyId;
            Path = path;
        }

        /// <summary>
        /// Gets the building reference key, as addressed by the GIS Web API.
        /// </summary>
        public string Reference { get; set; } = "";

        /// <summary>
        /// Gets the county part identifier the building was fetched from.
        /// </summary>
        public int CountyId { get; set; }

        /// <summary>
        /// Gets the typology path, one filing index per level, identifying the bucket the building was solved into.
        /// </summary>
        public List<int> Path { get; set; } = [];
    }
}
