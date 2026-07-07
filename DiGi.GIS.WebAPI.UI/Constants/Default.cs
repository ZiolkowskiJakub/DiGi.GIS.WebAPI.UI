namespace DiGi.GIS.WebAPI.UI.Constants
{
    /// <summary>
    /// Provides default values used during conversion of GIS domain objects to glTF.
    /// </summary>
    public static class Default
    {
        /// <summary>
        /// Base URI of the DiGi.Communication.WebAPI extension (hosted by the generic DiGi.WebAPI.WindowsService) used in production.
        /// </summary>
        public const string CommunicationWebAPIUri = "https://api.digiproject.uk";

        /// <summary>
        /// Base URI of the DiGi.Communication.WebAPI extension (hosted by the generic DiGi.WebAPI.WindowsService) used during local development.
        /// </summary>
        public const string CommunicationWebAPIUri_Development = "http://localhost:5010";

        /// <summary>
        /// Default storey height in meters used to extrude 2D building footprints.
        /// </summary>
        public const double StoreyHeight = 3.0;
    }
}
