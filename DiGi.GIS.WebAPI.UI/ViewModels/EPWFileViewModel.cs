namespace DiGi.GIS.WebAPI.UI.ViewModels
{
    /// <summary>
    /// Represents a view model for an EPW file, providing access to the underlying weather data and structure.
    /// </summary>
    public class EPWFileViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EPWFileViewModel"/> class.
        /// </summary>
        public EPWFileViewModel()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EPWFileViewModel"/> class.
        /// </summary>
        /// <param name="epwFile">The EPW file to be associated with this view model.</param>
        public EPWFileViewModel(DiGi.EPW.Classes.EPWFile? epwFile)
        {
            EPWFile = epwFile;
        }

        /// <summary>
        /// Gets the EPW file associated with the EPW file view.
        /// </summary>
        public DiGi.EPW.Classes.EPWFile? EPWFile { get; }
    }
}