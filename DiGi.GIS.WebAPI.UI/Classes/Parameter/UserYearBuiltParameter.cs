namespace DiGi.GIS.WebAPI.UI.Classes
{
    /// <summary>
    /// The year built answer a reviewer submits on the Orto Data page, relayed to the GIS Web API.
    /// <para>These property names are the wire contract of <c>POST gis/yearbuiltdata/setuseryearbuilt</c> and must match the service's own <c>UserYearBuiltParameter</c>. This application reaches that service over HTTP only, so nothing checks them at compile time and a rename on either side fails silently - diff them by hand whenever either moves (Coding - WebAPI Contracts, section 5).</para>
    /// </summary>
    public class UserYearBuiltParameter
    {
        /// <summary>
        /// Gets or sets the identifier of the <c>building_2d</c> part the building is filed under.
        /// <para>Always the part the page received from the random draw or the direct-mode reference resolution, never one re-derived from a county code: a county code can name several parts, and the write is refused when the named part does not hold the reference.</para>
        /// </summary>
        public int? CountyId { get; set; }

        /// <summary> Gets or sets the reference of the building the answer is for. </summary>
        public string? Reference { get; set; }

        /// <summary>
        /// Gets or sets the photo year the answer names: the selected card's year, or the oldest or newest listed year for a bound answer.
        /// </summary>
        public short? Year { get; set; }

        /// <summary>
        /// Gets or sets how <see cref="Year"/> relates to the true construction year, as the integer value of <c>DiGi.GIS.Enums.YearBuiltRelation</c>: 0 exact, 1 at or before, 2 after.
        /// <para>The integer, never the member name: the service binds the value against its own enum spelling, and a renamed member turns a member-name string into a hard 400 while the integer never moves (Coding - WebAPI Contracts, section 2). The service treats null as 0.</para>
        /// </summary>
        public int? Relation { get; set; }
    }
}
