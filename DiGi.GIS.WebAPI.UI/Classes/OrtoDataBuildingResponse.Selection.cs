using System;

namespace DiGi.GIS.WebAPI.UI.Classes
{
    public partial class OrtoDataBuildingResponse
    {
        /// <summary>
        /// The year built answer recorded for a building: the year it names, how that year relates to the true construction year, and who recorded it when.
        /// <para>There is at most one user entry per building - the stored year built data is keyed by source - so the page's selection is a single object rather than a list.</para>
        /// </summary>
        public class Selection
        {
            /// <summary> Gets or sets the photo year the answer names, or the bound year for an at-or-before or after answer. </summary>
            public short? Year { get; set; }

            /// <summary>
            /// Gets or sets how <see cref="Year"/> relates to the true construction year, as the integer value of <c>DiGi.GIS.Enums.YearBuiltRelation</c>: 0 exact, 1 at or before, 2 after.
            /// </summary>
            public int? Relation { get; set; }

            /// <summary> Gets or sets who recorded the answer, or null for legacy entries written before provenance was kept. </summary>
            public string? UserName { get; set; }

            /// <summary> Gets or sets when the answer was recorded (UTC), or null for legacy entries written before provenance was kept. </summary>
            public DateTimeOffset? DateTime { get; set; }
        }
    }
}
