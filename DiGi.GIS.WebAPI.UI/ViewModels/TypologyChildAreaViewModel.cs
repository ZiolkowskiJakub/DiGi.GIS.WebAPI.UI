namespace DiGi.GIS.WebAPI.UI.ViewModels
{
    /// <summary>
    /// One area of <c>GET /typology/childareas</c>: an area one level below one too large for a Typology solve, which the area view offers to open instead (DiGi.GIS.WebAPI.UI#29, B2).
    /// </summary>
    public class TypologyChildAreaViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TypologyChildAreaViewModel"/> class.
        /// </summary>
        public TypologyChildAreaViewModel() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="TypologyChildAreaViewModel"/> class.
        /// </summary>
        /// <param name="id">The identifier the area view opens the area with - the lowest of its polygon parts.</param>
        /// <param name="code">The TERYT code of the area.</param>
        /// <param name="name">The name of the area.</param>
        /// <param name="administrativeArealType">The administrative level of the area, as the integer the area view carries.</param>
        /// <param name="count">The number of buildings of the area, or null when it is not counted.</param>
        public TypologyChildAreaViewModel(int id, string? code, string? name, int administrativeArealType, long? count)
        {
            Id = id;
            Code = code;
            Name = name;
            AdministrativeArealType = administrativeArealType;
            Count = count;
        }

        /// <summary>
        /// Gets the identifier the area view opens the area with: the lowest of its polygon parts, since a solve resolves every part from the code.
        /// </summary>
        public int Id { get; }

        /// <summary>
        /// Gets the TERYT code of the area.
        /// </summary>
        public string? Code { get; }

        /// <summary>
        /// Gets the name of the area.
        /// </summary>
        public string? Name { get; }

        /// <summary>
        /// Gets the administrative level of the area, as the integer of <c>AdministrativeArealType</c> the area view carries in its query string.
        /// </summary>
        public int AdministrativeArealType { get; }

        /// <summary>
        /// Gets the number of buildings of the area: exact for a county (the sum over its parts), null for a voivodeship (not counted) or when the count could not be read.
        /// </summary>
        public long? Count { get; }
    }
}
