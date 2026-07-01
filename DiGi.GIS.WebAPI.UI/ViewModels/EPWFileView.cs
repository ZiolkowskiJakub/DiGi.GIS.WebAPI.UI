namespace DiGi.GIS.WebAPI.UI.ViewModels
{
    /// <summary>
    /// Represents a view for an EPW file, providing access to the underlying weather data and structure.
    /// </summary>
    public class EPWFileView
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EPWFileView"/> class.
        /// </summary>
        public EPWFileView()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EPWFileView"/> class.
        /// </summary>
        /// <param name="epwFile">The EPW file to be associated with this view.</param>
        public EPWFileView(DiGi.EPW.Classes.EPWFile? epwFile)
        {
            EPWFile = epwFile;
        }

        /// <summary>
        /// Gets the EPW file associated with the EPW file view.
        /// </summary>
        public DiGi.EPW.Classes.EPWFile? EPWFile { get; }
    }
}