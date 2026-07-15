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
        /// <para>Points at the production service: no DiGi.WebAPI.WindowsService host runs locally by default, and a dead localhost URI made every V2 calculation fail with HTTP 500 (connection refused). Restore a localhost URI (matching the local host port) only when debugging the Communication extension locally.</para>
        /// </summary>
        public const string CommunicationWebAPIUri_Development = "https://api.digiproject.uk";

        /// <summary>
        /// Default storey height in meters used to extrude 2D building footprints.
        /// </summary>
        public const double StoreyHeight = 3.0;
    }
}
