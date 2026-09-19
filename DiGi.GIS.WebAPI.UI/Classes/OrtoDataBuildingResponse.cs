using System.Collections.Generic;

namespace DiGi.GIS.WebAPI.UI.Classes
{
    /// <summary>
    /// The building the Orto Data page is to verify: where to find it, the photo years it holds, and the answer already on record.
    /// <para>An instance with no <see cref="Reference"/> is the page's empty state - nothing left to verify - which is also what the relay answers while the upstream endpoints are not deployed. Serialized to the page script with the framework's web defaults, so the property names reach the browser camelCased (<c>countyId</c>, <c>reference</c>, <c>years</c>, <c>selected</c>).</para>
    /// </summary>
    public partial class OrtoDataBuildingResponse
    {
        /// <summary> Gets or sets the identifier of the <c>building_2d</c> part the building is filed under, or null when no building was drawn. </summary>
        public int? CountyId { get; set; }

        /// <summary> Gets or sets the reference of the building, or null when no building was drawn. </summary>
        public string? Reference { get; set; }

        /// <summary>
        /// Gets or sets the years that hold a photo of the building - the only years a card is rendered for.
        /// <para>Sorted ascending upstream; the bound answers read their oldest and newest ends.</para>
        /// </summary>
        public List<short> Years { get; set; } = [];

        /// <summary> Gets or sets the answer already recorded for the building, or null when none is. </summary>
        public Selection? Selected { get; set; }
    }
}
