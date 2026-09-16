namespace DiGi.GIS.WebAPI.UI.Classes
{
    /// <summary>
    /// The body the <c>histogramsummary</c> relay posts to the upstream <c>gis/BuildingData/histogramsummary</c> endpoint.
    /// <para>The property names are the wire contract: the upstream binds its own <c>HistogramRequestParameter</c> from the body, and <c>Query.PostJsonAsync</c> sends the declared names as-is (<c>JsonSerializerOptions.Default</c>), so nothing relies on the receiver binding names case-insensitively (see Coding - WebAPI Contracts, section 2). The upstream's <c>FilterGroup</c> is deliberately absent — the definition page never filters a histogram (the county-part scope is #38).</para>
    /// </summary>
    public class TypologyHistogramParameter
    {
        /// <summary>Gets or sets the unique identifier (slug) of the column to histogram, as listed by the <c>columns</c> endpoint.</summary>
        public string ColumnUniqueId { get; set; } = string.Empty;

        /// <summary>Gets or sets the county part identifier scoping the histogram; <see langword="null"/> asks for the whole table.</summary>
        public int? CountyId { get; set; }

        /// <summary>Gets or sets the number of buckets; the relay fixes it to <see cref="Constants.Default.HistogramBucketCount"/>.</summary>
        public int BucketCount { get; set; }

        /// <summary>Gets or sets the bucketing rule, sent as its integer value; the relay asks for <see cref="DiGi.PostgreSQL.Table.Enums.HistogramBucketing.EqualCount"/> (issue #37), so every bucket holds the same number of buildings and the Load's quantile bounds follow the buildings rather than the value span. A host without ZiolkowskiJakub/DiGi.GIS.WebAPI#35 ignores the property and answers equal-width buckets.</summary>
        public DiGi.PostgreSQL.Table.Enums.HistogramBucketing HistogramBucketing { get; set; }
    }
}
