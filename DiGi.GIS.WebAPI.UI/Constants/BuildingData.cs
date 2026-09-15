namespace DiGi.GIS.WebAPI.UI.Constants
{
    /// <summary>
    /// Provides the canonical names and unique identifiers of the building data table's built-in columns, as the deployed GIS Web API carries them on the wire.
    /// <para>The names are the <c>Name</c> of the columns the GIS Web API's own <c>DiGi.GIS.IO.Constants.Column</c> defines (reference, county, database identifier, internal point), and are the keys a fetched table is addressed by. The unique identifiers are the catalog's <c>UniqueId</c> slugs of the same columns - the keys a projection is requested by, and what a Typology definition level names. Both are spelled here, once, because this application reaches the GIS Web API over the wire only and cannot reference <c>DiGi.GIS.IO</c> for them.</para>
    /// </summary>
    public static class BuildingData
    {
        /// <summary>
        /// The name of the building data table's county part column: the partition the row is filed under, always projected by the upstream paging endpoint whether asked for or not.
        /// </summary>
        public const string CountyIdName = "County Id";

        /// <summary>
        /// The name of the building data table's database identifier column: the <c>Building2DReference.Id</c> the 2D details and the 3D viewer routes address a building by.
        /// </summary>
        public const string DatabaseIdName = "Database Id";

        /// <summary>
        /// The unique identifier (projection slug) of the <see cref="DatabaseIdName"/> column.
        /// </summary>
        public const string DatabaseIdUniqueId = "database_id";

        /// <summary>
        /// The name of the building data table's internal point X column, read by the clip when the area is below county level.
        /// </summary>
        public const string InternalPointXName = "Internal Point X";

        /// <summary>
        /// The unique identifier (projection slug) of the <see cref="InternalPointXName"/> column.
        /// </summary>
        public const string InternalPointXUniqueId = "internal_point_x";

        /// <summary>
        /// The name of the building data table's internal point Y column, read by the clip when the area is below county level.
        /// </summary>
        public const string InternalPointYName = "Internal Point Y";

        /// <summary>
        /// The unique identifier (projection slug) of the <see cref="InternalPointYName"/> column.
        /// </summary>
        public const string InternalPointYUniqueId = "internal_point_y";

        /// <summary>
        /// The name of the building data table's reference column: it names the buildings the solve files into buckets, and it is the keyset cursor that pages a part. Always projected by the upstream paging endpoint whether asked for or not.
        /// </summary>
        public const string ReferenceName = "Reference";

        /// <summary>
        /// The unique identifier (projection slug) of the <see cref="ReferenceName"/> column.
        /// </summary>
        public const string ReferenceUniqueId = "reference";
    }
}
