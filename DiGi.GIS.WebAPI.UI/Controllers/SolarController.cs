using DiGi.Analytical.Building.Classes;
using DiGi.Analytical.Building.Solar;
using DiGi.Core.Classes;
using DiGi.Core.Enums;
using DiGi.Core.Interfaces;
using DiGi.EPW.Classes;
using DiGi.Geometry.Planar.Classes;
using DiGi.Geometry.Spatial.Classes;
using DiGi.GIS.WebAPI.UI.Classes;
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
    /// <para>The radiation routes answer the same refusals: 400 for a neighbour radius outside (0, <see cref="Constants.Default.SolarSurroundingRadiusMax"/>], 204 when the building or its weather file is not found, 422 when the building cannot be located or has no closed external envelope, 413 above <see cref="Constants.Default.SolarReceiverCountMax"/> receiving surfaces or <see cref="Constants.Default.SolarCasterTriangleCountMax"/> caster triangles, 502 when the neighbours cannot be read, and 499 / 504 / 500 for a client cancel, an upstream timeout and any other failure.</para>
    /// </summary>
    [Route("[controller]")]
    public class SolarController : Controller
    {
        private readonly IHttpClientFactory httpClientFactory;
        private readonly ILogger<SolarController> logger;
        private readonly SemaphoreSlim semaphoreSlim;

        /// <summary>
        /// Initializes a new instance of the <see cref="SolarController"/> class.
        /// </summary>
        /// <param name="httpClientFactory">The <see cref="IHttpClientFactory"/> used to create <see cref="HttpClient"/> instances.</param>
        /// <param name="semaphoreSlim">The gate shared by every solar radiation solve on this host, registered under <see cref="Constants.Default.SolarSolveGateKey"/>.</param>
        /// <param name="logger">The logger receiving one line per solar radiation request.</param>
        public SolarController(IHttpClientFactory httpClientFactory, [FromKeyedServices(Constants.Default.SolarSolveGateKey)] SemaphoreSlim semaphoreSlim, ILogger<SolarController> logger)
        {
            this.httpClientFactory = httpClientFactory;
            this.semaphoreSlim = semaphoreSlim;
            this.logger = logger;
        }

        /// <summary>
        /// Displays the solar radiation 3D viewer of a building: the page streams its scene from <see cref="GetGLBBuildingModelByIdAsync(long, int?, double?, CancellationToken)"/> and shows a legend with the colour ramp, the neighbour radius and the EPW station. The page itself carries no geometry and runs no solve.
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

            SolarSceneViewModel solarSceneViewModel = new($"Solar radiation {id}", $"~/solar/glb/buildingmodelbyid?{query}", radius_Value, StationName(ePWFile), stationUrl);

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
            return await SolveAsync(id, countyId, radius, (buildingModel, buildingModels_Surrounding, surfaceSolarRadiationResults) =>
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
            }, cancellationToken);
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

        private IActionResult PayloadTooLarge(string message)
        {
            return StatusCode(StatusCodes.Status413PayloadTooLarge, new List<string>() { message });
        }

        private IActionResult RadiusBadRequest()
        {
            return BadRequest(new List<string>() { string.Format(CultureInfo.InvariantCulture, "The neighbour radius must be a number above 0 and at most {0} metres.", Constants.Default.SolarSurroundingRadiusMax) });
        }

        // The pipeline shared by the JSON and glb routes: fetch and stamp the building, refuse what
        // the synchronous limits do not cover before any expensive step, fetch the neighbours and the
        // weather, then solve behind the host-wide gate and hand the results to the route's response.
        private async Task<IActionResult> SolveAsync(long id, int? countyId, double? radius, Func<BuildingModel, List<BuildingModel>, List<SurfaceSolarRadiationResult>, IActionResult> respond, CancellationToken cancellationToken)
        {
            double radius_Value = radius ?? Constants.Default.SolarSurroundingRadius;
            if (!IsValidRadius(radius_Value))
            {
                return RadiusBadRequest();
            }

            Stopwatch stopwatch = Stopwatch.StartNew();

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

                if (normals.Count > Constants.Default.SolarReceiverCountMax)
                {
                    return PayloadTooLarge($"Building {id} has {normals.Count} external walls and roofs; a calculation covers at most {Constants.Default.SolarReceiverCountMax}. Buildings this large need a background calculation, which is not available yet.");
                }

                // The neighbour circle is centred on the footprint and reaches the radius beyond its farthest corner.
                Circle2D? circle2D = buildingModel.TerrainCircle(radius_Value, 0);
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
                if (casterTriangleCount > Constants.Default.SolarCasterTriangleCountMax)
                {
                    return PayloadTooLarge($"The surroundings of building {id} within {radius_Value.ToString(CultureInfo.InvariantCulture)} m hold {casterTriangleCount} shading triangles; a calculation covers at most {Constants.Default.SolarCasterTriangleCountMax}. Choose a smaller radius.");
                }

                ShadingSolverOptions shadingSolverOptions = new()
                {
                    AngleTolerance = Constants.Default.SolarAngleTolerance,
                };

                List<SurfaceSolarRadiationResult>? surfaceSolarRadiationResults;

                await semaphoreSlim.WaitAsync(cancellationToken);
                try
                {
                    surfaceSolarRadiationResults = shadingModel.SurfaceSolarRadiationResults(normals, ePWFile, shadingSolverOptions, message => logger.LogInformation("{Message}", message));
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
    }
}
