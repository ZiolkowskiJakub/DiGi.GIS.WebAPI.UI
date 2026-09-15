namespace DiGi.GIS.WebAPI.UI.Classes
{
    /// <summary>
    /// The body the <c>histogramsummary</c> relay posts to the upstream <c>gis/BuildingData/histogramsummary</c> endpoint.
    /// <para>The property names are the wire contract: the upstream binds its own <c>HistogramRequestParameter</c> from the body, and <c>Query.PostJsonAsync</c> sends the declared names as-is (<c>JsonSerializerOptions.Default</c>), so nothing relies on the receiver binding names case-insensitively (see Coding - WebAPI Contracts, section 2). The upstream's <c>FilterGroup</c> is deliberately absent — the definition page never filters a histogram.</para>
    /// </summary>
    public class TypologyHistogramParameter
    {
        /// <summary>Gets or sets the unique identifier (slug) of the column to histogram, as listed by the <c>columns</c> endpoint.</summary>
        public string ColumnUniqueId { get; set; } = string.Empty;

        /// <summary>Gets or sets the county part identifier scoping the histogram; <see langword="null"/> asks for the whole table.</summary>
        public int? CountyId { get; set; }

        /// <summary>Gets or sets the number of equal-width buckets; the relay fixes it to <see cref="Constants.Default.HistogramBucketCount"/>.</summary>
        public int BucketCount { get; set; }
    }
}
