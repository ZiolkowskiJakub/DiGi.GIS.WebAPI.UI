namespace DiGi.GIS.WebAPI.UI.Constants
{
    /// <summary>
    /// Provides the canonical names of the building data table's built-in columns, as the deployed GIS Web API carries them on the wire.
    /// <para>The names are the <c>Name</c> of the columns the GIS Web API's own <c>DiGi.GIS.IO.Constants.Column</c> defines (reference, internal point), and are the keys the client addresses them by - the projection slugs (<c>reference</c>, <c>internal_point_x</c>, ...) are the catalog's <c>UniqueId</c>s and name the same columns.</para>
    /// </summary>
    public static class BuildingData
    {
        /// <summary>
        /// The name of the building data table's reference column: it names the buildings the solve files into buckets, and it is the keyset cursor that pages a part.
        /// </summary>
        public const string ReferenceName = "Reference";

        /// <summary>
        /// The name of the building data table's internal point X column, read by the clip when the area is below county level.
        /// </summary>
        public const string InternalPointXName = "Internal Point X";

        /// <summary>
        /// The name of the building data table's internal point Y column, read by the clip when the area is below county level.
        /// </summary>
        public const string InternalPointYName = "Internal Point Y";
    }
}
