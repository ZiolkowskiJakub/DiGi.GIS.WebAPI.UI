namespace DiGi.GIS.WebAPI.UI.ViewModels
{
    /// <summary>
    /// One building dot of a 2D area view: the bounding-box centre of a <c>building_2d</c> row, keyed by the reference and the county part the row is filed under.
    /// <para>A building reference is unique only per county partition - a reference is stored once per county part it was imported under - so the county identifier travels with the reference for any caller-side join, such as the Typology area view's join with the solved typology assignment. <see cref="X"/> and <see cref="Y"/> are the centre of the row's bounding-box columns in the same coordinate reference system the <c>point2dsbyreferences</c> endpoint answers in (PL-1992, EPSG:2180, metres). Row order is not contractual.</para>
    /// <para>A relay of <c>DiGi.GIS.PostgreSQL.Classes.Building2DCentroid</c> without its <c>_type</c> discriminator: the browser never spells a type name, and the discriminator alone is most of the upstream payload for a county-sized area.</para>
    /// </summary>
    public class Building2DCentroidViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Building2DCentroidViewModel"/> class.
        /// </summary>
        public Building2DCentroidViewModel() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Building2DCentroidViewModel"/> class.
        /// </summary>
        /// <param name="reference">The building reference key.</param>
        /// <param name="countyId">The county part identifier the building row is filed under.</param>
        /// <param name="x">The X coordinate of the bounding-box centre.</param>
        /// <param name="y">The Y coordinate of the bounding-box centre.</param>
        public Building2DCentroidViewModel(string reference, int countyId, double x, double y)
        {
            Reference = reference;
            CountyId = countyId;
            X = x;
            Y = y;
        }

        /// <summary>
        /// Gets the building reference key, as addressed by the GIS Web API.
        /// </summary>
        public string Reference { get; set; } = "";

        /// <summary>
        /// Gets the county part identifier the building row is filed under.
        /// </summary>
        public int CountyId { get; set; }

        /// <summary>
        /// Gets the X coordinate of the bounding-box centre, in PL-1992 (EPSG:2180) metres.
        /// </summary>
        public double X { get; set; }

        /// <summary>
        /// Gets the Y coordinate of the bounding-box centre, in PL-1992 (EPSG:2180) metres.
        /// </summary>
        public double Y { get; set; }
    }
}
