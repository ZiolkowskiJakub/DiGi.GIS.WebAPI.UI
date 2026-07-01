using DiGi.GIS.Classes;
using System.Collections.Generic;

namespace DiGi.GIS.WebAPI.UI.ViewModels
{
    /// <summary>
    /// Represents a view model for an administrative areal in 2D, providing access to its references and associated data.
    /// </summary>
    public class AdministrativeAreal2DView
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AdministrativeAreal2DView"/> class.
        /// </summary>
        public AdministrativeAreal2DView()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AdministrativeAreal2DView"/> class with specified administrative areal data and references.
        /// </summary>
        /// <param name="administrativeAreal2DReference">The reference to the administrative areal 2D.</param>
        /// <param name="administrativeAreal2D">The administrative areal 2D object.</param>
        /// <param name="administrativeAreal2DReferencePath">The path reference for the administrative areal 2D.</param>
        /// <param name="administrativeAreal2DReferences">A collection of references to administrative areals 2D.</param>
        public AdministrativeAreal2DView(PostgreSQL.Classes.AdministrativeAreal2DReference? administrativeAreal2DReference, AdministrativeAreal2D? administrativeAreal2D, PostgreSQL.Classes.AdministrativeAreal2DReferencePath? administrativeAreal2DReferencePath, IEnumerable<PostgreSQL.Classes.AdministrativeAreal2DReference>? administrativeAreal2DReferences)
        {
            AdministrativeAreal2DReference = administrativeAreal2DReference;
            AdministrativeAreal2D = administrativeAreal2D;
            AdministrativeAreal2DReferencePath = administrativeAreal2DReferencePath;
            AdministrativeAreal2DReferences = administrativeAreal2DReferences != null ? [.. administrativeAreal2DReferences] : [];
        }

        /// <summary>
        /// Gets the reference to the administrative areal 2D.
        /// </summary>
        public PostgreSQL.Classes.AdministrativeAreal2DReference? AdministrativeAreal2DReference { get; }

        /// <summary>
        /// Gets the administrative areal 2D object.
        /// </summary>
        public AdministrativeAreal2D? AdministrativeAreal2D { get; }

        /// <summary>
        /// Gets the path reference for the administrative areal 2D.
        /// </summary>
        public PostgreSQL.Classes.AdministrativeAreal2DReferencePath? AdministrativeAreal2DReferencePath { get; }

        /// <summary>
        /// Gets a list of references to administrative areals 2D.
        /// </summary>
        public List<PostgreSQL.Classes.AdministrativeAreal2DReference>? AdministrativeAreal2DReferences { get; }
    }
}