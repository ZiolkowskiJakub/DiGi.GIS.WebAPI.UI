using DiGi.GIS.PostgreSQL.Enums;

namespace DiGi.GIS.WebAPI.UI.ViewModels
{
    /// <summary>
    /// Represents the administrative-area context the Typology Load modal redirected with: the area the colour-coded building typology is solved for, shown in the Administrative Area card of the area view.
    /// </summary>
    public class TypologyViewViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TypologyViewViewModel"/> class.
        /// </summary>
        public TypologyViewViewModel() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="TypologyViewViewModel"/> class from the query context of the Load modal redirect.
        /// </summary>
        /// <param name="id">The unique identifier of the administrative area.</param>
        /// <param name="code">The code of the administrative area.</param>
        /// <param name="administrativeArealType">The type of the administrative area.</param>
        /// <param name="name">The name of the administrative area, or null when it could not be read.</param>
        public TypologyViewViewModel(int id, string? code, AdministrativeArealType administrativeArealType, string? name)
        {
            Id = id;
            Code = code;
            AdministrativeArealType = administrativeArealType;
            Name = name;
        }

        /// <summary>
        /// Gets the unique identifier of the administrative area.
        /// </summary>
        public int Id { get; }

        /// <summary>
        /// Gets the code of the administrative area.
        /// </summary>
        public string? Code { get; }

        /// <summary>
        /// Gets the type of the administrative area.
        /// </summary>
        public AdministrativeArealType AdministrativeArealType { get; }

        /// <summary>
        /// Gets the name of the administrative area, read from the GIS Web API by the area identifier, or null when the lookup answered nothing - the page then shows the context it carries on the query alone.
        /// </summary>
        public string? Name { get; }

        /// <summary>
        /// Gets the display name of the administrative area type.
        /// </summary>
        public string AdministrativeArealTypeName => AdministrativeArealType.ToString();
    }
}
