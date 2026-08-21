namespace DiGi.GIS.WebAPI.UI.Constants
{
    /// <summary>
    /// Provides default values used during conversion of GIS domain objects to glTF.
    /// </summary>
    public static class Default
    {
        /// <summary>
        /// The radius, in metres, searched around a plan position to find the building standing there.
        /// <para>The 3D viewer knows a picked building by its centroid rather than by its identifier, so the building is recovered by asking for everything within this distance of that point. Small on purpose: it has to be forgiving of the difference between a footprint centroid and a model centroid without reaching a neighbouring building.</para>
        /// </summary>
        public const double BuildingSearchRadius = 5.0;

        /// <summary>
        /// The tolerance, in metres, applied to the spatial query behind <see cref="BuildingSearchRadius"/>.
        /// </summary>
        public const double BuildingSearchTolerance = 5.0;

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
        /// Base URI of the GIS Web API (DiGi.GIS.WebAPI, hosted by the generic DiGi.WebAPI.WindowsService) this application proxies.
        /// <para>Every outbound request this application makes is built on this value, so the whole application can be pointed at another host by changing it here. The service is deployed on a separate machine and is versioned independently of this application - query <c>GET /information/controllers</c> on it to learn which build is actually answering before relying on a recently added endpoint.</para>
        /// </summary>
        public const string GISWebAPIUri = "https://api.digiproject.uk";

        /// <summary>
        /// Base URI of the GIS Web API used during local development.
        /// <para>Points at the production service for the same reason <see cref="CommunicationWebAPIUri_Development"/> does: no host runs locally by default, and a dead localhost URI turns every page of this application into an error. Restore a localhost URI (matching the local host port) only when debugging the GIS Web API locally.</para>
        /// </summary>
        public const string GISWebAPIUri_Development = "https://api.digiproject.uk";

        /// <summary>
        /// The fewest points a reduced outline is allowed to keep, for an administrative area with no rule of its own and for a building footprint.
        /// </summary>
        public const int PolygonMinimumPointCount = 100;

        /// <summary>
        /// The fewest points a reduced country outline is allowed to keep.
        /// </summary>
        public const int PolygonMinimumPointCount_Country = 30;

        /// <summary>
        /// The fewest points a reduced voivodeship outline is allowed to keep.
        /// </summary>
        public const int PolygonMinimumPointCount_Voivodeship = 50;

        /// <summary>
        /// The reduction factor applied to an outline of an administrative area that has no rule of its own.
        /// <para>These outlines are drawn as an overview map a few hundred pixels across, so they are simplified before they are sent rather than after. The factor falls as the area grows: a country outline carries far more points than the map can show, a subdivision barely more.</para>
        /// </summary>
        public const double PolygonReductionFactor = 0.01;

        /// <summary>
        /// The reduction factor applied to a country outline. See <see cref="PolygonReductionFactor"/>.
        /// </summary>
        public const double PolygonReductionFactor_Country = 0.00001;

        /// <summary>
        /// The reduction factor applied to a county outline. See <see cref="PolygonReductionFactor"/>.
        /// </summary>
        public const double PolygonReductionFactor_County = 0.001;

        /// <summary>
        /// The reduction factor applied to a voivodeship outline. See <see cref="PolygonReductionFactor"/>.
        /// </summary>
        public const double PolygonReductionFactor_Voivodeship = 0.001;

        /// <summary>
        /// Default storey height in meters used to extrude 2D building footprints.
        /// </summary>
        public const double StoreyHeight = 3.0;

        /// <summary>
        /// Whether the ground surface is added to the scenes that display <see cref="DiGi.Analytical.Building.Classes.BuildingModel"/> geometry (the 3D viewer and the communication view).
        /// <para>The standalone terrain feature (the Terrain controller and its own pages) shows the surface on its own, where the elevation is correct as stored.</para>
        /// <para>Note that an area smaller than the sampling lattice legitimately holds no points: the counties are sampled at 10 m to 100 m, so a request with a radius below the lattice step answers 404 without meaning that nothing was ever stored there.</para>
        /// </summary>
        public const bool TerrainEnabled = true;

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
        public const string TerrainUri = GISWebAPIUri + "/gis/terrain";
    }
}
