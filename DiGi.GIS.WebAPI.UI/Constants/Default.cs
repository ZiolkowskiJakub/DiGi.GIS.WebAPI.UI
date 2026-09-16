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
        /// URI of the GIS Web API endpoint listing the columns of the building data table, which the Typology definition page offers for grouping and the definition import resolves its columns against.
        /// </summary>
        public const string BuildingDataColumnsUri = GISWebAPIUri + "/gis/BuildingData/columns";

        /// <summary>
        /// URI of the GIS Web API endpoint that pages building data rows by county part, used by the Typology solve to fetch the table the solver classifies.
        /// </summary>
        public const string BuildingDataTableUri = GISWebAPIUri + "/gis/BuildingData/tablebybuildingdatabypagingparameter";

        /// <summary>
        /// URI of the GIS Web API endpoint answering the value distribution histogram (bucket, actual bucket min/max, building count) of one building data column inside a county part, which the Typology definition Load reads to split an area's buildings into equal-count ranges (issue #30).
        /// </summary>
        public const string BuildingDataHistogramUri = GISWebAPIUri + "/gis/BuildingData/histogramsummary";

        /// <summary>
        /// The number of buckets the histogram relay asks for; 1000 is the upstream cap and, with the equal-count bucketing the relay asks for (issue #37), gives the Load's quantile boundaries a resolution of one part in a thousand of every county part's buildings whatever the value distribution.
        /// <para>The cost is one sort of the part's values and at most 1000 rows back — the same order as the 0.38 s partition scan measured for the equal-width request (issue #30).</para>
        /// </summary>
        public const int HistogramBucketCount = 1000;

        /// <summary>
        /// The ceiling on the total number of buildings one Typology solve classifies; an area above it is refused with a 413 and an actionable message instead of timing out the fetch and the solve.
        /// <para>Chosen above the largest county verified live (code 1465 with 154 529 buildings) and below any voivodeship, so a county still solves while a voivodeship or country scope degrades gracefully (issue #22 guardrail).</para>
        /// </summary>
        public const int BuildingSolveCeiling = 200_000;

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
        /// The maximum radius in meters for fast loading 3D display areas without user warnings.
        /// </summary>
        public const double DisplayRadiusFastMax = 1000.0;

        /// <summary>
        /// The maximum allowable radius in meters for 3D display areas.
        /// </summary>
        public const double DisplayRadiusMax = 1500.0;

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
        /// The clip margin in metres added to a terrain query beyond the boundary the surface is clipped to, so that the clipping never runs along the very edge of the surface.
        /// <para>This covers the clip only. The distance the surface can stop short of the query radius on a coarse lattice is a separate term - see <see cref="TerrainLatticeStepMax"/> - and <see cref="Query.TerrainQueryCircle(Geometry.Planar.Classes.Circle2D?, double, double, double)"/> adds both.</para>
        /// </summary>
        public const double TerrainBuffer = 15.0;

        /// <summary>
        /// The coarsest lattice, in metres, the counties' elevation points are sampled on (10 m to 100 m - see <see cref="TerrainEnabled"/>).
        /// <para>The terrain service triangulates only the stored points inside the query circle, so the surface it answers stops short of the query radius by up to one lattice diagonal: a point lies inside the returned surface once all four corners of its lattice cell are inside the query, and the farthest corner is <c>step * sqrt(2)</c> away. Growing a query by <c>TerrainLatticeStepMax * sqrt(2)</c> beyond the display boundary therefore guarantees the boundary is covered on any lattice up to this step, and it is deliberately the worst case rather than the county's own step so that no scene depends on knowing which county it is in. Measured on the deployed 100 m lattice the shortfall reaches 101 m; the bound is 141.4 m.</para>
        /// </summary>
        public const double TerrainLatticeStepMax = 100.0;

        /// <summary>
        /// The number of segments used to discretize a circular boundary into a regular 2D polygon during terrain clipping.
        /// </summary>
        public const int TerrainCircleSegmentCount = 64;

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
        /// The margin in meters extending beyond building bounding envelopes when calculating dynamic terrain coverage.
        /// </summary>
        public const double TerrainPadding = 50.0;

        /// <summary>
        /// The default minimum radius of the ground surface, in metres, shown around a scene that holds a building model.
        /// <para>A building scene ensures at least this radius of ground is displayed for context even when building footprints are small.</para>
        /// </summary>
        public const double TerrainRadius = 100.0;

        /// <summary>
        /// The maximum search radius in meters supported by the GIS Web API terrain service.
        /// </summary>
        public const double TerrainRadiusMax = 2000.0;

        /// <summary>
        /// The longest a terrain request to the GIS Web API may take, in seconds, before it is abandoned.
        /// <para>Terrain is an optional overlay, so a stalled terrain query must not hold a page request open for the 100 second <see cref="System.Net.Http.HttpClient"/> default. Abandoning it is answered exactly like an area with no stored elevation points.</para>
        /// </summary>
        public const double TerrainRequestTimeout = 30.0;

        /// <summary>
        /// Base URI of the GIS Web API terrain endpoints.
        /// </summary>
        public const string TerrainUri = GISWebAPIUri + "/gis/terrain";

        /// <summary>
        /// The curve and edge thickness written into every appearance of a Typology definition document.
        /// <para>The definition page edits a single color per bucket; the document carries a full <see cref="DiGi.Typology.Visual.Classes.TypologyAppearance"/> per bucket, so the thickness the page does not edit is fixed here rather than invented per export.</para>
        /// </summary>
        public const double TypologyAppearanceThickness = 1.0;

        /// <summary>
        /// URI of the endpoint that exchanges a set of credentials for a session token.
        /// </summary>
        public const string UserLoginUri = UserWebAPIUri + "/user/login";

        /// <summary>
        /// URI of the endpoint that terminates the presented session, revoking its token until the token's natural expiration.
        /// </summary>
        public const string UserLogoutUri = UserWebAPIUri + "/user/logout";

        /// <summary>
        /// URI of the endpoint that issues a new token for the identity carried by the presented one.
        /// </summary>
        public const string UserRefreshUri = UserWebAPIUri + "/user/session/refresh";

        /// <summary>
        /// URI of the endpoint that reads the stored record of the authenticated user.
        /// </summary>
        public const string UserSecureDataUri = UserWebAPIUri + "/user/secure-data";

        /// <summary>
        /// URI of the endpoint that introspects the presented session.
        /// </summary>
        public const string UserSessionUri = UserWebAPIUri + "/user/session";

        /// <summary>
        /// The name of the cookie this application keeps a visitor's session token in.
        /// <para>The token never reaches the browser as a value: the cookie is written HttpOnly by the server and read back by it on every relayed request, so a script on the page cannot read, copy or leak it. See <see cref="Create.UserTokenCookieOptions"/>.</para>
        /// </summary>
        public const string UserTokenCookieName = "digi_user_token";

        /// <summary>
        /// Base URI of the user authentication service (DiGi.User.WebAPI, hosted by the generic DiGi.WebAPI.WindowsService) this application signs its visitors in against.
        /// <para>Kept apart from <see cref="GISWebAPIUri"/> even though both address the same host today: the authentication service is versioned and deployed independently, so pointing sign-in at another host must not move every GIS read with it.</para>
        /// </summary>
        public const string UserWebAPIUri = "https://api.digiproject.uk";

        /// <summary>
        /// Base URI of the user authentication service used during local development.
        /// <para>Points at the production service for the same reason <see cref="GISWebAPIUri_Development"/> does: no host runs locally by default, and a dead localhost URI would turn every sign-in attempt into a failure indistinguishable from a wrong password. Restore a localhost URI (matching the local host port) only when debugging DiGi.User.WebAPI locally.</para>
        /// </summary>
        public const string UserWebAPIUri_Development = "https://api.digiproject.uk";
    }
}
