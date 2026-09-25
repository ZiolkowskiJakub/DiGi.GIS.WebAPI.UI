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
        /// The number of rows the Typology solve asks <see cref="BuildingDataTableUri"/> for per page - the upstream cap.
        /// <para>Page size does not decide the cost of the read: a warm 10 000-row page answers in about 0.1 s and a cold one is bound by the database reading its heap (DiGi.GIS.WebAPI.UI#29). The cap only keeps the number of round trips down.</para>
        /// </summary>
        public const int BuildingDataPageSize = 10000;

        /// <summary>
        /// The response header in which <see cref="BuildingDataTableUri"/> returns the cursor of the next page of a physical-order read (DiGi.GIS.WebAPI#40). Absent when the county part is exhausted, and absent when the endpoint answered in reference order instead.
        /// <para>Must equal the GIS Web API's <c>Constants.Header.NextCursor</c>: the client has no compile-time link to the API (Coding - WebAPI Contracts, section 1).</para>
        /// </summary>
        public const string NextCursorHeaderName = "DiGi-Next-Cursor";

        /// <summary>
        /// URI of the GIS Web API endpoint answering the building centroids of an administrative area as one compact, columnar document - <c>{"References":[…],"CountyIds":[…],"X":[…],"Y":[…]}</c> - the 2D dot layer of the Typology area view is drawn from (DiGi.GIS.WebAPI#40).
        /// </summary>
        public const string Building2DCentroidsUri = GISWebAPIUri + "/gis/building2D/centroidsbyadministrativeareal2Did";

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
        /// URI of the GIS Web API endpoint drawing the next building to verify: one with orthophoto coverage and no user-provided year built yet.
        /// <para>Requires a signed-in session.</para>
        /// </summary>
        public const string OrtoDataRandomBuilding2DReferenceUri = GISWebAPIUri + "/gis/ortodatas/randombuilding2dreference";

        /// <summary>
        /// URI of the GIS Web API endpoint listing the photo years held for a building - the only years the Orto Data page renders a card for.
        /// <para>Requires a signed-in session.</para>
        /// </summary>
        public const string OrtoDataYearsByReferenceUri = GISWebAPIUri + "/gis/ortodatas/yearsbyreference";

        /// <summary>
        /// URI of the GIS Web API endpoint serving the orthophoto image of a building for one year, as JPEG bytes.
        /// <para>The <c>fallbackbyreference</c> parameter the relay sends is ignored by builds that predate it.</para>
        /// </summary>
        public const string OrtoDataImageByReferenceUri = GISWebAPIUri + "/gis/ortodatas/imagebyreference";

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
        /// URI of the GIS Web API endpoint answering every stored building model whose footprint lies within a circle (<c>x</c>, <c>y</c>, <c>radius</c>), from which the solar radiation calculation takes the neighbours casting shade on the analysed building.
        /// </summary>
        public const string BuildingModelItemsByCircleUri = GISWebAPIUri + "/gis/buildingmodel/itemsbycircle";

        /// <summary>
        /// URI of the GIS Web API endpoint answering the EPW weather file of the station serving a location (<c>x</c>, <c>y</c>), the weather the solar radiation calculation integrates over one year.
        /// <para>The station differs by location, and so does the file's quality: for Warsaw Ursynów it serves IWEC <c>WARSAW</c>, whose snow depth is a filler value, and elsewhere IMGW <c>Warszawa Okecie</c>, whose albedo is all missing (ZiolkowskiJakub/DiGi.Solar#7, comment 5831808537).</para>
        /// </summary>
        public const string EPWFileItemUri = GISWebAPIUri + "/gis/epwfile/item";

        /// <summary>
        /// The angular tolerance, in radians, below which the solar radiation calculation groups sun directions into one shading solve (<c>ShadingSolverOptions.AngleTolerance</c>); 2°, twice the solver default.
        /// <para>Measured over a full EPW year (ZiolkowskiJakub/DiGi.Solar#7, comment 5831808537): it halves the direction groups (615 → 295) and the solve time, and changes the annual per-surface irradiation by at most 2.06 %. #7 measured on a 16-thread machine, not on this host; the relative saving holds on any machine.</para>
        /// </summary>
        public const double SolarAngleTolerance = Core.Constants.Tolerance.Angle;

        /// <summary>
        /// The upper bound, in metres, of the neighbour radius a solar radiation request may ask for; a larger one is refused with a 400.
        /// <para>Headroom above <see cref="SolarSurroundingRadius"/> for tall distant casters in low winter sun, which the measured buildings do not cover; 200 m cost up to twice the solve time for no measured gain (ZiolkowskiJakub/DiGi.Solar#7, comment 5831808537).</para>
        /// </summary>
        public const double SolarSurroundingRadiusMax = 100.0;

        /// <summary>
        /// The default neighbour radius, in metres, of a solar radiation request, measured from the edge of the analysed building's footprint: every building within it casts shade.
        /// <para>Going from 50 to 200 m changed the shading loss of the three measured buildings by at most 0.1 point while the solve time rose by 10–125 % (ZiolkowskiJakub/DiGi.Solar#7, comment 5831808537).</para>
        /// </summary>
        public const double SolarSurroundingRadius = 50.0;

        /// <summary>
        /// The ceiling on the number of shading-only triangles (neighbours and the analysed building's own non-receiving components) one synchronous solar radiation request solves against; above it the request is refused with a 413.
        /// <para>A 50 m radius in central Warsaw gave 8 600–10 700 caster triangles (ZiolkowskiJakub/DiGi.Solar#7, comment 5831808537), which the ceiling still admits at the default radius. On this host, a 4-core Intel N150 (DiGi.GIS.WebAPI.UI#59), casters cost more than on the 16-thread machine #7 measured: 25 receivers among 8 600 caster triangles took 42 s against 23 s for 28 receivers among a few. Larger requests are the background jobs of DiGi.GIS.WebAPI.UI#60.</para>
        /// </summary>
        public const int SolarCasterTriangleCountMax = 12_000;

        /// <summary>
        /// The number of solar radiation solves that may run at the same time on this host; further requests wait for the gate registered under <see cref="SolarSolveGateKey"/>.
        /// <para>One solve already uses every core: two parallel solves took 38.9 s each against 23.0 s for one on the 16-thread machine #7 measured, so a second request waits less on average when queued (ZiolkowskiJakub/DiGi.Solar#7, comment 5831808537). This host has 4 cores, which makes the case for one slot stronger.</para>
        /// </summary>
        public const int SolarConcurrentSolveCount = 1;

        /// <summary>
        /// The irradiation, in kWh/m² per year, at the top of the colour ramp of the solar radiation viewer; higher values take the top colour.
        /// <para>Fixed rather than fitted to each building so that two buildings read the same colour for the same irradiation. The best roofs of the measured Warsaw buildings received 997–1 041 kWh/m² against an annual global horizontal irradiation of 978–999 kWh/m² (ZiolkowskiJakub/DiGi.Solar#7, comment 5831808537).</para>
        /// </summary>
        public const double SolarIrradiationScaleMax = 1200.0;

        /// <summary>
        /// The ceiling on the number of receiving surfaces (external walls and roofs) of the building one synchronous solar radiation request calculates; above it the request is refused with a 413.
        /// <para>Measured on this host, a 4-core Intel N150 with 16 GB, whole requests in central Warsaw at the default radius took about 1.7 s per receiver: 42 s at 25 receivers, 56 s at 34, 65 s at 37 and 51–159 s at 43–48, repeated runs of one building varying up to 2.5 times (DiGi.GIS.WebAPI.UI#59). At 30 receivers a typical request stays near one minute, well under the ~135 s at which the front end answered 503. ZiolkowskiJakub/DiGi.Solar#7 proposed 100, but measured a 16-thread machine, not this host. Refine it from the per-request log of <c>SolarController</c> (<c>logs\log-yyyyMMdd.txt</c>). Larger buildings are the background jobs of DiGi.GIS.WebAPI.UI#60.</para>
        /// </summary>
        public const int SolarReceiverCountMax = 30;

        /// <summary>
        /// The year every EPW record is mapped to before the sun position is calculated. Typical meteorological years mix calendar years month by month, and the last record of the year rolls into the next one, so one fixed year keeps the time series monotonic.
        /// <para>A non-leap year, so the 8 760 hours of an EPW year map one to one; a 29 February record is skipped.</para>
        /// </summary>
        public const int SolarReferenceYear = 2025;

        /// <summary>
        /// The dependency injection key of the <see cref="System.Threading.SemaphoreSlim"/> of <see cref="SolarConcurrentSolveCount"/> slots that gates every solar radiation solve on this host.
        /// <para>A keyed singleton rather than a static field so that the background jobs of DiGi.GIS.WebAPI.UI#60 share the same gate with the synchronous requests.</para>
        /// </summary>
        public const string SolarSolveGateKey = "SolarSolveGate";

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

        /// <summary>
        /// URI of the GIS Web API endpoint recording a reviewer's year built answer for a building.
        /// <para>Requires a signed-in session.</para>
        /// </summary>
        public const string YearBuiltDataSetUserYearBuiltUri = GISWebAPIUri + "/gis/yearbuiltdata/setuseryearbuilt";
    }
}
