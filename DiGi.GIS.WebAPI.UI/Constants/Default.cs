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

        /// <summary>
        /// TERRAIN. Whether the ground surface is added to the scenes that display <see cref="DiGi.Analytical.Building.Classes.BuildingModel"/> geometry (the 3D viewer and the communication view).
        /// <para>Off, because neither half of the pairing is ready. The GIS Web API terrain endpoints are not deployed and the elevation table they read does not exist yet; and the building models those scenes show are extruded from Building2D footprints at elevation 0, while a terrain surface carries its true height (around 110 m over Warsaw), so the two would not meet. Real stored building models are tracked by DiGi.GIS.PostgreSQL issue 2.</para>
        /// <para>Turn on when both are true. Everything the feature adds to those scenes is behind a TERRAIN note naming this constant, so it can be found in one sweep and promoted or removed. The standalone terrain feature (the Terrain controller and its own pages) is not gated by this - it shows the surface on its own, where the elevation is correct as stored.</para>
        /// </summary>
        public const bool TerrainEnabled = false;

        /// <summary>
        /// The name given to the terrain node of a scene.
        /// </summary>
        public const string TerrainName = "Terrain";

        /// <summary>
        /// The radius of the ground surface, in metres, shown around a scene that holds a single building.
        /// <para>A single building has no requested area of its own to borrow, so this is how much ground it is given for context.</para>
        /// </summary>
        public const double TerrainRadius = 100.0;

        /// <summary>
        /// The longest a terrain request to the GIS Web API may take, in seconds, before it is abandoned.
        /// <para>Terrain is an optional overlay, so a stalled terrain query must not hold a page request open for the 100 second <see cref="System.Net.Http.HttpClient"/> default. Abandoning it is answered exactly like an area with no stored elevation points.</para>
        /// </summary>
        public const double TerrainRequestTimeout = 30.0;

        /// <summary>
        /// Base URI of the GIS Web API terrain endpoints.
        /// </summary>
        public const string TerrainUri = "https://api.digiproject.uk/gis/terrain";
    }
}