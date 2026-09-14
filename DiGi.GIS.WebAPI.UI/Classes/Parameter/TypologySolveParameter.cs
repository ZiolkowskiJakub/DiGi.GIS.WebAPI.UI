using DiGi.GIS.PostgreSQL.Enums;

namespace DiGi.GIS.WebAPI.UI.Classes
{
    /// <summary>
    /// The request body of <c>POST /typology/buildings</c>: the Typology definition page state to solve, and the administrative area it is solved for.
    /// <para>The <see cref="Definition"/> is the same page state the definition page exports, resolved server-side against the live column catalog before the solve. The <see cref="Id"/>, <see cref="Code"/> and <see cref="AdministrativeArealType"/> carry the area context the Load modal selected, and are used to resolve the county part identifiers that scope the building data fetch.</para>
    /// <para><see cref="AdministrativeArealType"/> is nullable on the wire: the <c>Undefined</c> sentinel is -1 and not 0, so a non-nullable binding would silently keep <c>Country</c> for an omitted parameter (Coding - WebAPI Contracts, section 2). The action rejects <see langword="null"/> and <see cref="AdministrativeArealType.Undefined"/> explicitly.</para>
    /// </summary>
    public class TypologySolveParameter
    {
        /// <summary>
        /// Gets or sets the Typology definition page state: the chain of selected building-data columns with rule types, ranges and colors.
        /// </summary>
        public TypologyDefinitionParameter? Definition { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the selected administrative area, as carried by the Load modal.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the code of the selected administrative area.
        /// </summary>
        public string? Code { get; set; }

        /// <summary>
        /// Gets or sets the type of the selected administrative area, bound as nullable so an omitted value is refused rather than read as <see cref="AdministrativeArealType.Country"/>.
        /// </summary>
        public AdministrativeArealType? AdministrativeArealType { get; set; }
    }
}
