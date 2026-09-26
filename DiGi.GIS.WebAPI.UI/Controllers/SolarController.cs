using DiGi.Analytical.Building.Classes;
using DiGi.Analytical.Building.Solar;
using DiGi.Core.Classes;
using DiGi.Core.Enums;
using DiGi.Core.Interfaces;
using DiGi.EPW.Classes;
using DiGi.Geometry.Planar.Classes;
using DiGi.Geometry.Spatial.Classes;
using DiGi.GIS.WebAPI.UI.Classes;
using DiGi.GIS.WebAPI.UI.Enums;
using DiGi.GIS.WebAPI.UI.ViewModels;
using DiGi.GLTF.Classes;
using DiGi.Solar.Classes;
using DiGi.WebAPI.Classes;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace DiGi.GIS.WebAPI.UI.Controllers
{
    /// <summary>
    /// Solar calculations for the 3D viewers: the sun position for the Lighting panel, and the annual solar radiation on the external walls and roofs of one building, calculated on this host's CPU with DiGi.Solar over one EPW year, with the building itself and its neighbours casting shade.
    /// <para>The radiation routes answer the same refusals: 400 for a neighbour radius outside (0, <see cref="Constants.Default.SolarSurroundingRadiusMax"/>], 204 when the building or its weather file is not found, 422 when the building cannot be located or has no closed external envelope, 413 above <see cref="Constants.Default.SolarReceiverCountMax"/> receiving surfaces or <see cref="Constants.Default.SolarCasterTriangleCountMax"/> caster triangles, 502 when the neighbours cannot be read, 503 with <c>Retry-After</c> when the solve gate stays busy for <see cref="Constants.Default.SolarSolveGateWaitSeconds"/> seconds, and 499 / 504 / 500 for a client cancel, an upstream timeout and any other failure.</para>
    /// <para>Buildings above those ceilings are calculated as background jobs (<c>solar/jobs</c>, DiGi.GIS.WebAPI.UI#60): the POST runs the same checks with the job ceilings (<see cref="Constants.Default.SolarJobReceiverCountMax"/>, <see cref="Constants.Default.SolarJobCasterTriangleCountMax"/>), queues the prepared calculation and answers 202, or 503 with <c>Retry-After</c> when <see cref="Constants.Default.SolarJobQueueLengthMax"/> jobs are already waiting. The job routes answer 404 for an unknown or expired job and 409 for results of a job that has not completed.</para>
    /// </summary>
    [Route("[controller]")]
    public class SolarController : Controller
    {
        private readonly IHttpClientFactory httpClientFactory;
        private readonly ILogger<SolarController> logger;
        private readonly SemaphoreSlim semaphoreSlim;
        private readonly SolarJobQueue solarJobQueue;

        /// <summary>
        /// Initializes a new instance of the <see cref="SolarController"/> class.
        /// </summary>
        /// <param name="httpClientFactory">The <see cref="IHttpClientFactory"/> used to create <see cref="HttpClient"/> instances.</param>
        /// <param name="semaphoreSlim">The gate shared by every solar radiation solve on this host, registered under <see cref="Constants.Default.SolarSolveGateKey"/>.</param>
        /// <param name="solarJobQueue">The background solar radiation jobs of this host.</param>
        /// <param name="logger">The logger receiving one line per solar radiation request.</param>
        public SolarController(IHttpClientFactory httpClientFactory, [FromKeyedServices(Constants.Default.SolarSolveGateKey)] SemaphoreSlim semaphoreSlim, SolarJobQueue solarJobQueue, ILogger<SolarController> logger)
        {
            this.httpClientFactory = httpClientFactory;
            this.semaphoreSlim = semaphoreSlim;
            this.solarJobQueue = solarJobQueue;
            this.logger = logger;
        }

        /// <summary>
        /// Gets or sets how long a synchronous request waits for the solve gate before it is refused with a 503; <see cref="Constants.Default.SolarSolveGateWaitSeconds"/> outside the tests.
        /// </summary>
        internal TimeSpan SolveGateWait { get; set; } = TimeSpan.FromSeconds(Constants.Default.SolarSolveGateWaitSeconds);

        /// <summary>
        /// Handles the HTTP GET request to the root endpoint and returns the solar radiation landing page, where a building is chosen by its identifier.
        /// </summary>
        /// <returns>An <see cref="IActionResult"/> representing the start view.</returns>
        [HttpGet("")]
        public IActionResult Start()
        {
            return View("~/Views/Solar/Start.cshtml");
        }

        /// <summary>
        /// Displays the solar radiation 3D viewer of a building: the page streams its scene from <see cref="GetGLBBuildingModelByIdAsync(long, int?, double?, CancellationToken)"/> and shows a legend with the colour ramp, the neighbour radius and the EPW station. The page itself carries no geometry and runs no solve.
        /// <para>When the scene request is refused with a 413, the page offers a background calculation (<see cref="PostJobAsync(long, int?, double?, CancellationToken)"/>), polls it and loads its scene from <see cref="GetJobGLB(Guid)"/>; the job identifier is kept in the page address (<c>job</c>), so a reload resumes the polling.</para>
        /// </summary>
        /// <param name="id">The unique identifier of the building.</param>
        /// <param name="countyId">The optional unique identifier of the county associated with the building.</param>
        /// <param name="radius">The neighbour radius in metres, measured from the edge of the footprint; omitted means <see cref="Constants.Default.SolarSurroundingRadius"/>.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by the caller to cancel the asynchronous operation.</param>
        /// <returns>The viewer page; 400 for an invalid radius, 204 when the building or its weather file is not found, 422 when the building cannot be located.</returns>
        [HttpGet("buildingmodelbyid")]
        public async Task<IActionResult> GetBuildingModelByIdAsync([FromQuery(Name = "id")] long id, [FromQuery(Name = "countyid")] int? countyId, [FromQuery(Name = "radius")] double? radius, CancellationToken cancellationToken = default)
        {
            double radius_Value = radius ?? Constants.Default.SolarSurroundingRadius;
            if (!IsValidRadius(radius_Value))
            {
                return RadiusBadRequest();
            }

            HttpClient httpClient = httpClientFactory.CreateClient();

            BuildingModel? buildingModel = await httpClient.BuildingModelAsync(id, countyId, cancellationToken);
            cancellationToken.ThrowIfCancellationRequested();
            if (buildingModel is null)
            {
                return NoContent();
            }

            // GIS qualifier: this project's own Modify class shadows DiGi.GIS.Analytical.Modify.
            if (!GIS.Analytical.Modify.UpdateBuildingInformation(buildingModel))
            {
                return UnprocessableEntity(new List<string>() { $"Building {id} could not be located, so the sun cannot be positioned for it." });
            }

            Point2D? center = buildingModel.TerrainCircle(0, 0)?.Center;
            if (center is null)
            {
                return UnprocessableEntity(new List<string>() { $"Building {id} has no geometry." });
            }

            EPWFile? ePWFile = await httpClient.ItemAsync<EPWFile>(EPWFileItemUri(center), cancellationToken);
            cancellationToken.ThrowIfCancellationRequested();
            if (ePWFile is null)
            {
                return NoContent();
            }

            string query = $"id={id.ToString(CultureInfo.InvariantCulture)}";
            if (countyId.HasValue)
            {
                query += $"&countyid={countyId.Value.ToString(CultureInfo.InvariantCulture)}";
            }

            query += $"&radius={radius_Value.ToString(CultureInfo.InvariantCulture)}";

            string stationUrl = $"~/epwfile/item?x={center.X.ToString(CultureInfo.InvariantCulture)}&y={center.Y.ToString(CultureInfo.InvariantCulture)}";

            SolarSceneViewModel solarSceneViewModel = new($"Solar radiation {id}", $"~/solar/glb/buildingmodelbyid?{query}", "~/solar/jobs", query, radius_Value, StationName(ePWFile), stationUrl);

            return View("~/Views/Solar/SolarSceneView.cshtml", solarSceneViewModel);
        }

        /// <summary>
        /// Calculates the annual solar radiation on the external walls and roofs of a building and streams it as a binary glTF (.glb) scene: each receiving surface coloured by its irradiation on the fixed ramp of <see cref="Query.SolarIrradiationColor(double)"/> and carrying its <see cref="SurfaceSolarRadiationResult"/> as node properties, the building's other components grey, and the neighbours as grey semi-transparent context.
        /// </summary>
        /// <param name="id">The unique identifier of the building.</param>
        /// <param name="countyId">The optional unique identifier of the county associated with the building.</param>
        /// <param name="radius">The neighbour radius in metres, measured from the edge of the footprint; omitted means <see cref="Constants.Default.SolarSurroundingRadius"/>.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by the caller to cancel the asynchronous operation.</param>
        /// <returns>The <c>model/gltf-binary</c> payload, or one of the refusals listed on <see cref="SolarController"/>.</returns>
        [HttpGet("glb/buildingmodelbyid")]
        public async Task<IActionResult> GetGLBBuildingModelByIdAsync([FromQuery(Name = "id")] long id, [FromQuery(Name = "countyid")] int? countyId, [FromQuery(Name = "radius")] double? radius, CancellationToken cancellationToken = default)
        {
            return await SolveAsync(id, countyId, radius, (buildingModel, buildingModels_Surrounding, surfaceSolarRadiationResults) => GLBResult(buildingModel, buildingModels_Surrounding, surfaceSolarRadiationResults, id, countyId), cancellationToken);
        }

        /// <summary>
        /// Calculates the annual solar radiation on the external walls and roofs of a building: one <see cref="SurfaceSolarRadiationResult"/> per receiving surface, over one EPW year, with the building itself and its neighbours within <paramref name="radius"/> casting shade.
        /// </summary>
        /// <param name="id">The unique identifier of the building.</param>
        /// <param name="countyId">The optional unique identifier of the county associated with the building.</param>
        /// <param name="radius">The neighbour radius in metres, measured from the edge of the footprint; omitted means <see cref="Constants.Default.SolarSurroundingRadius"/>.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by the caller to cancel the asynchronous operation.</param>
        /// <returns>The results as DiGi JSON (<c>Core.Convert.ToSystem_String</c>, readable by <c>Core.Convert.ToDiGi</c>), or one of the refusals listed on <see cref="SolarController"/>.</returns>
        [HttpGet("radiationbybuildingmodelid")]
        public async Task<IActionResult> GetRadiationByBuildingModelIdAsync([FromQuery(Name = "id")] long id, [FromQuery(Name = "countyid")] int? countyId, [FromQuery(Name = "radius")] double? radius, CancellationToken cancellationToken = default)
        {
            return await SolveAsync(id, countyId, radius, (buildingModel, buildingModels_Surrounding, surfaceSolarRadiationResults) =>
            {
                return Content(Core.Convert.ToSystem_String(surfaceSolarRadiationResults) ?? "[]", "application/json");
            }, cancellationToken);
        }

        /// <summary>
        /// Queues the annual solar radiation of a building as a background job, for buildings above the synchronous ceilings. The building, its neighbours and its weather are fetched and checked in this request, with the job ceilings (<see cref="Constants.Default.SolarJobReceiverCountMax"/>, <see cref="Constants.Default.SolarJobCasterTriangleCountMax"/>), so every refusal is immediate; the solve runs later, one job at a time, behind the gate shared with the synchronous routes.
        /// </summary>
        /// <param name="id">The unique identifier of the building.</param>
        /// <param name="countyId">The optional unique identifier of the county associated with the building.</param>
        /// <param name="radius">The neighbour radius in metres, measured from the edge of the footprint; omitted means <see cref="Constants.Default.SolarSurroundingRadius"/>.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by the caller to cancel the asynchronous operation; it does not reach the queued job.</param>
        /// <returns>202 with the <see cref="SolarJobViewModel"/> of the job and its <c>Location</c> (<c>solar/jobs/{jobId}</c>); 503 with <c>Retry-After</c> when the queue is full; otherwise one of the refusals listed on <see cref="SolarController"/>.</returns>
        [HttpPost("jobs")]
        public async Task<IActionResult> PostJobAsync([FromQuery(Name = "id")] long id, [FromQuery(Name = "countyid")] int? countyId, [FromQuery(Name = "radius")] double? radius, CancellationToken cancellationToken = default)
        {
            double radius_Value = radius ?? Constants.Default.SolarSurroundingRadius;
            if (!IsValidRadius(radius_Value))
            {
                return RadiusBadRequest();
            }

            // A full queue is refused before the upstream requests and the shading model, which would
            // compete with the running job for nothing; TryEnqueue checks again for a race.
            if (solarJobQueue.QueuedCount() >= Constants.Default.SolarJobQueueLengthMax)
            {
                return QueueFull();
            }

            return await PrepareAsync(id, countyId, radius_Value, true, (buildingModel, buildingModels_Surrounding, normals, ePWFile, shadingModel, casterTriangleCount) =>
            {
                // The calculation holds its inputs until it has run; nothing is shared with another job.
                SolarJob solarJob = new(Guid.NewGuid(), id, countyId, radius_Value, normals.Count, buildingModel, buildingModels_Surrounding, () => shadingModel.SurfaceSolarRadiationResults(normals, ePWFile, SolarShadingSolverOptions(), message => logger.LogInformation("{Message}", message)));
                if (!solarJobQueue.TryEnqueue(solarJob))
                {
                    return Task.FromResult(QueueFull());
                }

                logger.LogInformation("Solar radiation job {JobId} queued for building {Id} (county {CountyId}): radius {Radius} m, {ReceiverCount} receivers, {CasterTriangleCount} caster triangles.", solarJob.Id, id, countyId, radius_Value, normals.Count, casterTriangleCount);

                return Task.FromResult<IActionResult>(Accepted($"{Request.PathBase}/solar/jobs/{solarJob.Id.ToString("D", CultureInfo.InvariantCulture)}", solarJobQueue.SolarJobViewModel(solarJob.Id)));
            }, cancellationToken);
        }

        /// <summary>
        /// Gets the state of a background solar radiation job: its status, its place in the queue, the seconds spent in its current state and, for a failed job, the error text.
        /// </summary>
        /// <param name="jobId">The unique identifier of the job.</param>
        /// <returns>200 with the <see cref="SolarJobViewModel"/> of the job; 404 when it is unknown or has expired.</returns>
        [HttpGet("jobs/{jobId:guid}")]
        public IActionResult GetJob([FromRoute(Name = "jobId")] Guid jobId)
        {
            SolarJobViewModel? solarJobViewModel = solarJobQueue.SolarJobViewModel(jobId);
            if (solarJobViewModel is null)
            {
                return JobNotFound(jobId);
            }

            return Ok(solarJobViewModel);
        }

        /// <summary>
        /// Gets the results of a completed background solar radiation job, in the shape of <see cref="GetRadiationByBuildingModelIdAsync(long, int?, double?, CancellationToken)"/>.
        /// </summary>
        /// <param name="jobId">The unique identifier of the job.</param>
        /// <returns>The results as DiGi JSON; 409 when the job has not completed; 404 when it is unknown or has expired.</returns>
        [HttpGet("jobs/{jobId:guid}/result")]
        public IActionResult GetJobResult([FromRoute(Name = "jobId")] Guid jobId)
        {
            SolarJob? solarJob = CompletedJob(jobId, out IActionResult? actionResult);
            if (solarJob?.SurfaceSolarRadiationResults is not List<SurfaceSolarRadiationResult> surfaceSolarRadiationResults)
            {
                return actionResult ?? JobNotFound(jobId);
            }

            return Content(Core.Convert.ToSystem_String(surfaceSolarRadiationResults) ?? "[]", "application/json");
        }

        /// <summary>
        /// Streams the coloured binary glTF (.glb) scene of a completed background solar radiation job, in the shape of <see cref="GetGLBBuildingModelByIdAsync(long, int?, double?, CancellationToken)"/>, built from the stored results without solving again.
        /// </summary>
        /// <param name="jobId">The unique identifier of the job.</param>
        /// <returns>The <c>model/gltf-binary</c> payload; 409 when the job has not completed; 404 when it is unknown or has expired.</returns>
        [HttpGet("jobs/{jobId:guid}/glb")]
        public IActionResult GetJobGLB([FromRoute(Name = "jobId")] Guid jobId)
        {
            SolarJob? solarJob = CompletedJob(jobId, out IActionResult? actionResult);
            if (solarJob?.SurfaceSolarRadiationResults is not List<SurfaceSolarRadiationResult> surfaceSolarRadiationResults || solarJob.BuildingModel is not BuildingModel buildingModel || solarJob.BuildingModels_Surrounding is not List<BuildingModel> buildingModels_Surrounding)
            {
                return actionResult ?? JobNotFound(jobId);
            }

            return GLBResult(buildingModel, buildingModels_Surrounding, surfaceSolarRadiationResults, solarJob.BuildingModelId, solarJob.CountyId);
        }

        /// <summary>
        /// Cancels a background solar radiation job: a queued job is never calculated; a running calculation cannot be interrupted, so it finishes and its results are discarded.
        /// </summary>
        /// <param name="jobId">The unique identifier of the job.</param>
        /// <returns>204; 404 when the job is unknown or has expired.</returns>
        [HttpDelete("jobs/{jobId:guid}")]
        public IActionResult DeleteJob([FromRoute(Name = "jobId")] Guid jobId)
        {
            if (!solarJobQueue.Cancel(jobId))
            {
                return JobNotFound(jobId);
            }

            logger.LogInformation("Solar radiation job {JobId} cancelled.", jobId);

            return NoContent();
        }

        /// <summary>
        /// [TEMPORARY] Calculates the sun position for a world location and a local date and time, for the 3D viewer Lighting panel.
        /// <para>Hosted locally until a DiGi.Solar backed endpoint is available on the central GIS Web API. The route contract (solar/sundirection) is final - when the central endpoint exists this action becomes a proxy like the other actions in this project, and the consuming frontend (gltf-viewer.js) stays unchanged.</para>
        /// </summary>
        /// <param name="x">The X coordinate in the EPSG:2180 coordinate system [m].</param>
        /// <param name="y">The Y coordinate in the EPSG:2180 coordinate system [m].</param>
        /// <param name="date">The local calendar date in the yyyy-MM-dd format.</param>
        /// <param name="hour">The local time of day as a decimal hour in the 0-24 range.</param>
        /// <returns>JSON with the true solar angles: azimuth [deg] (0 = north, clockwise) and altitude [deg] above the horizon (negative at night).</returns>
        [HttpGet("sundirection")]
        public IActionResult GetSunDirection([FromQuery(Name = "x")] double x, [FromQuery(Name = "y")] double y, [FromQuery(Name = "date")] string? date, [FromQuery(Name = "hour")] double hour)
        {
            if (!double.IsFinite(x) || !double.IsFinite(y) || !double.IsFinite(hour) || hour < 0 || hour > 24)
            {
                return BadRequest();
            }

            if (!DateTime.TryParseExact(date, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dateTime))
            {
                return BadRequest();
            }

            dateTime = dateTime.AddHours(hour);

            // GIS qualifier: this project's own Query class (DiGi.GIS.WebAPI.UI.Query) shadows
            // DiGi.GIS.Query in the enclosing-namespace lookup.
            Coordinates? coordinates = GIS.Query.Coordinates(new Point2D(x, y));
            if (coordinates is null)
            {
                return NoContent();
            }

            // EPSG:2180 scenes are Polish, so the local time zone is CET/CEST. The offset follows
            // the daylight saving state of the requested date, because this endpoint reports
            // wall-clock sun angles for a single date. A shading model, in contrast, holds one
            // fixed offset (UTC.Plus0100, no DST) for its whole solve - see UpdateBuildingInformation.
            // The identifier is the Windows form; .NET resolves it on any platform through the built-in IANA mapping.
            TimeZoneInfo timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time");
            UTC uTC = timeZoneInfo.GetUtcOffset(dateTime).TotalHours == 2 ? UTC.Plus0200 : UTC.Plus0100;

            Vector3D? vector3D = Solar.Query.SunDirection(coordinates, uTC, dateTime, true);
            if (vector3D is null)
            {
                return NoContent();
            }

            // SunDirection returns the direction sunlight travels, built from the solar angles as
            // x = cos(el) * cos(az + 90deg), y = -cos(el) * sin(az + 90deg), z = -sin(el);
            // inverted here back to the true solar angles the frontend contract expects.
            double altitude = Math.Asin(Math.Clamp(-vector3D.Z, -1, 1)) * (180.0 / Math.PI);
            double azimuth = (Math.Atan2(-vector3D.Y, vector3D.X) * (180.0 / Math.PI)) - 90.0;
            azimuth = ((azimuth % 360.0) + 360.0) % 360.0;

            return Ok(new { azimuth, altitude });
        }

        private static string EPWFileItemUri(Point2D center)
        {
            UrlBuilder urlBuilder = new(Constants.Default.EPWFileItemUri);
            urlBuilder = urlBuilder.AddParameter("x", center.X);
            urlBuilder = urlBuilder.AddParameter("y", center.Y);
            return urlBuilder.ToString();
        }

        private static bool IsValidRadius(double radius)
        {
            return double.IsFinite(radius) && radius > 0 && radius <= Constants.Default.SolarSurroundingRadiusMax;
        }

        private static string? StationName(EPWFile ePWFile)
        {
            Location? location = ePWFile.Location;
            if (location is null)
            {
                return null;
            }

            List<string> values = [];
            foreach (string? value in new string?[] { location.City, location.Source, string.IsNullOrWhiteSpace(location.WHO) ? null : $"WMO {location.WHO}" })
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    values.Add(value.Trim());
                }
            }

            return values.Count == 0 ? null : string.Join(", ", values);
        }

        // The glb of a solved building, shared by the synchronous route and the job route: each receiving
        // surface coloured by its irradiation, the building's other components grey, the neighbours as context.
        private IActionResult GLBResult(BuildingModel buildingModel, List<BuildingModel> buildingModels_Surrounding, List<SurfaceSolarRadiationResult> surfaceSolarRadiationResults, long id, int? countyId)
        {
            // The same root reference as the 3D building viewer, so a selected surface traces back to its building.
            IReference? reference = PostgreSQL.Create.Reference(buildingModel, null, countyId);

            List<GLTFNode>? gLTFNodes = buildingModel.SolarGLTFNodes(buildingModels_Surrounding, surfaceSolarRadiationResults, reference);
            if (gLTFNodes is null || gLTFNodes.Count == 0)
            {
                return NoContent();
            }

            string name = $"SolarRadiation {id}";

            GLTFScene? gLTFScene = GLTF.Create.GLTFScene(gLTFNodes, name);
            if (gLTFScene is null)
            {
                return NoContent();
            }

            byte[]? bytes = GLTF.Convert.ToSystem_Bytes(gLTFScene, true);
            if (bytes is null || bytes.Length == 0)
            {
                return NoContent();
            }

            return File(bytes, "model/gltf-binary", $"{name}.glb");
        }

        // A job whose results can be read: the job, or null with the refusal - 404 for an unknown or expired
        // job, 409 for one that has not completed. The state is read under the queue's lock, and a completed
        // job's results never change afterwards.
        private SolarJob? CompletedJob(Guid jobId, out IActionResult? actionResult)
        {
            SolarJobViewModel? solarJobViewModel = solarJobQueue.SolarJobViewModel(jobId);
            if (solarJobViewModel is null)
            {
                actionResult = JobNotFound(jobId);
                return null;
            }

            if (solarJobViewModel.Status != nameof(SolarJobStatus.Completed))
            {
                string message = $"Job {jobId:D} is {solarJobViewModel.Status.ToLowerInvariant()}; its results are available once it has completed.";
                if (!string.IsNullOrWhiteSpace(solarJobViewModel.Error))
                {
                    message += $" {solarJobViewModel.Error}";
                }

                actionResult = Conflict(new List<string>() { message });
                return null;
            }

            SolarJob? solarJob = solarJobQueue.SolarJob(jobId);
            actionResult = solarJob is null ? JobNotFound(jobId) : null;
            return solarJob;
        }

        private IActionResult JobNotFound(Guid jobId)
        {
            return NotFound(new List<string>() { string.Format(CultureInfo.InvariantCulture, "Job {0:D} is unknown or has expired: results are kept for {1} minutes after a job finishes, and jobs are lost when the server restarts.", jobId, Constants.Default.SolarJobResultRetentionMinutes) });
        }

        private IActionResult PayloadTooLarge(string message)
        {
            return StatusCode(StatusCodes.Status413PayloadTooLarge, new List<string>() { message });
        }

        private IActionResult QueueFull()
        {
            return ServiceUnavailable(Constants.Default.SolarJobRetryAfterSeconds, string.Format(CultureInfo.InvariantCulture, "{0} background calculations are already waiting, the most the server queues. Try again in a few minutes.", Constants.Default.SolarJobQueueLengthMax));
        }

        private IActionResult RadiusBadRequest()
        {
            return BadRequest(new List<string>() { string.Format(CultureInfo.InvariantCulture, "The neighbour radius must be a number above 0 and at most {0} metres.", Constants.Default.SolarSurroundingRadiusMax) });
        }

        private IActionResult ServiceUnavailable(int retryAfterSeconds, string message)
        {
            Response.Headers.RetryAfter = retryAfterSeconds.ToString(CultureInfo.InvariantCulture);
            return StatusCode(StatusCodes.Status503ServiceUnavailable, new List<string>() { message });
        }

        private static ShadingSolverOptions SolarShadingSolverOptions()
        {
            return new ShadingSolverOptions()
            {
                AngleTolerance = Constants.Default.SolarAngleTolerance,
            };
        }

        // The pipeline shared by the synchronous routes and the job route, up to the solve: fetch and stamp
        // the building, refuse what the ceilings do not cover before any expensive step (the job ceilings when
        // background is true), fetch the neighbours and the weather, build the shading model, then hand
        // everything to next - a synchronous solve, or a queued job.
        private async Task<IActionResult> PrepareAsync(long id, int? countyId, double radius, bool background, Func<BuildingModel, List<BuildingModel>, Dictionary<string, Vector3D>, EPWFile, ShadingModel, int, Task<IActionResult>> next, CancellationToken cancellationToken)
        {
            try
            {
                HttpClient httpClient = httpClientFactory.CreateClient();

                // Every fetch below answers null for a cancelled request too, so the token is checked
                // after each one: a client that went away is a 499, not a missing building.
                BuildingModel? buildingModel = await httpClient.BuildingModelAsync(id, countyId, cancellationToken);
                cancellationToken.ThrowIfCancellationRequested();
                if (buildingModel is null)
                {
                    return NoContent();
                }

                // GIS qualifier: this project's own Modify class shadows DiGi.GIS.Analytical.Modify.
                if (!GIS.Analytical.Modify.UpdateBuildingInformation(buildingModel))
                {
                    return UnprocessableEntity(new List<string>() { $"Building {id} could not be located, so the sun cannot be positioned for it." });
                }

                Dictionary<string, Vector3D>? normals = buildingModel.SolarReceiverNormals();
                if (normals is null || normals.Count == 0)
                {
                    return UnprocessableEntity(new List<string>() { $"Building {id} has no closed external envelope (a degenerate model), so its walls and roofs have no outward side to receive the sun." });
                }

                if (normals.Count > (background ? Constants.Default.SolarJobReceiverCountMax : Constants.Default.SolarReceiverCountMax))
                {
                    return PayloadTooLarge(background || normals.Count > Constants.Default.SolarJobReceiverCountMax
                        ? $"Building {id} has {normals.Count} external walls and roofs; a background calculation covers at most {Constants.Default.SolarJobReceiverCountMax}."
                        : $"Building {id} has {normals.Count} external walls and roofs; a calculation on request covers at most {Constants.Default.SolarReceiverCountMax}. Calculate it in the background instead, which covers up to {Constants.Default.SolarJobReceiverCountMax}.");
                }

                // The neighbour circle is centred on the footprint and reaches the radius beyond its farthest corner.
                Circle2D? circle2D = buildingModel.TerrainCircle(radius, 0);
                if (circle2D?.Center is not Point2D center)
                {
                    return UnprocessableEntity(new List<string>() { $"Building {id} has no geometry." });
                }

                UrlBuilder urlBuilder = new(Constants.Default.BuildingModelItemsByCircleUri);
                urlBuilder = urlBuilder.AddParameter("x", center.X);
                urlBuilder = urlBuilder.AddParameter("y", center.Y);
                urlBuilder = urlBuilder.AddParameter("radius", circle2D.Radius);

                // The circle contains the building itself, so a successful answer is never empty: an
                // absent or empty list is a failed read, and solving without neighbours would report
                // an unshaded result as if it were the answer.
                List<BuildingModel>? buildingModels_Surrounding = await httpClient.ItemsAsync<BuildingModel>(urlBuilder.ToString(), cancellationToken);
                cancellationToken.ThrowIfCancellationRequested();
                if (buildingModels_Surrounding is null || buildingModels_Surrounding.Count == 0)
                {
                    return StatusCode(StatusCodes.Status502BadGateway, new List<string>() { $"The neighbours of building {id} could not be read, so its shading cannot be calculated." });
                }

                EPWFile? ePWFile = await httpClient.ItemAsync<EPWFile>(EPWFileItemUri(center), cancellationToken);
                cancellationToken.ThrowIfCancellationRequested();
                if (ePWFile is null)
                {
                    return NoContent();
                }

                ShadingModel? shadingModel = buildingModel.ToSolar(buildingModels_Surrounding, x => new GuidReference(x).ToString() is string reference && normals.ContainsKey(reference));
                if (shadingModel is null)
                {
                    return UnprocessableEntity(new List<string>() { $"Building {id} could not be converted into a shading model." });
                }

                int casterTriangleCount = shadingModel.CasterTriangleCount();
                if (casterTriangleCount > (background ? Constants.Default.SolarJobCasterTriangleCountMax : Constants.Default.SolarCasterTriangleCountMax))
                {
                    string radiusText = radius.ToString(CultureInfo.InvariantCulture);
                    return PayloadTooLarge(background || casterTriangleCount > Constants.Default.SolarJobCasterTriangleCountMax
                        ? $"The surroundings of building {id} within {radiusText} m hold {casterTriangleCount} shading triangles; a background calculation covers at most {Constants.Default.SolarJobCasterTriangleCountMax}. Choose a smaller radius."
                        : $"The surroundings of building {id} within {radiusText} m hold {casterTriangleCount} shading triangles; a calculation on request covers at most {Constants.Default.SolarCasterTriangleCountMax}. Choose a smaller radius, or calculate it in the background, which covers up to {Constants.Default.SolarJobCasterTriangleCountMax}.");
                }

                return await next(buildingModel, buildingModels_Surrounding, normals, ePWFile, shadingModel, casterTriangleCount);
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                return StatusCode(499, "Calculation cancelled by the client.");
            }
            catch (TaskCanceledException exception)
            {
                return StatusCode(504, $"Building data request timed out: {exception.Message}");
            }
            catch (Exception exception)
            {
                logger.LogError(exception, "Solar radiation for building {Id} (county {CountyId}) failed.", id, countyId);
                return StatusCode(500, $"Internal server error: {exception.GetType().Name}: {exception.Message}");
            }
        }

        // The synchronous routes: prepare with the synchronous ceilings, then solve in this request behind the
        // host-wide gate and hand the results to the route's response. The gate is also held by background
        // jobs for many minutes, so the wait is bounded (SolarSolveGateWaitSeconds) and answers 503 rather
        // than holding the request until the front end gives up.
        private async Task<IActionResult> SolveAsync(long id, int? countyId, double? radius, Func<BuildingModel, List<BuildingModel>, List<SurfaceSolarRadiationResult>, IActionResult> respond, CancellationToken cancellationToken)
        {
            double radius_Value = radius ?? Constants.Default.SolarSurroundingRadius;
            if (!IsValidRadius(radius_Value))
            {
                return RadiusBadRequest();
            }

            Stopwatch stopwatch = Stopwatch.StartNew();

            return await PrepareAsync(id, countyId, radius_Value, false, async (buildingModel, buildingModels_Surrounding, normals, ePWFile, shadingModel, casterTriangleCount) =>
            {
                if (!await semaphoreSlim.WaitAsync(SolveGateWait, cancellationToken))
                {
                    logger.LogInformation("Solar radiation for building {Id} (county {CountyId}) refused: the solve gate stayed busy for {Wait} s.", id, countyId, SolveGateWait.TotalSeconds);
                    return ServiceUnavailable(Constants.Default.SolarSolveGateWaitSeconds, $"Another solar radiation calculation is running on the server. Try again in a minute, or calculate building {id} in the background.");
                }

                List<SurfaceSolarRadiationResult>? surfaceSolarRadiationResults;
                try
                {
                    surfaceSolarRadiationResults = shadingModel.SurfaceSolarRadiationResults(normals, ePWFile, SolarShadingSolverOptions(), message => logger.LogInformation("{Message}", message));
                }
                finally
                {
                    semaphoreSlim.Release();
                }

                logger.LogInformation("Solar radiation for building {Id} (county {CountyId}): radius {Radius} m, {ReceiverCount} receivers, {CasterTriangleCount} caster triangles, {ResultCount} results, total {Elapsed} ms.", id, countyId, radius_Value, normals.Count, casterTriangleCount, surfaceSolarRadiationResults?.Count, stopwatch.ElapsedMilliseconds);

                if (surfaceSolarRadiationResults is null)
                {
                    return UnprocessableEntity(new List<string>() { $"The solar radiation of building {id} could not be calculated from its model and weather file." });
                }

                return respond(buildingModel, buildingModels_Surrounding, surfaceSolarRadiationResults);
            }, cancellationToken);
        }
    }
}
