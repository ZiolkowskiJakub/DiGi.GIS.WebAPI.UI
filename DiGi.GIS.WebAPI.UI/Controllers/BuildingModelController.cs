using DiGi.Analytical.Building.Classes;
using DiGi.Analytical.Building.Enums;
using DiGi.Core.Interfaces;
using DiGi.Geometry.Planar;
using DiGi.Geometry.Planar.Classes;
using DiGi.Geometry.Spatial.Classes;
using DiGi.GIS.WebAPI.UI.ViewModels;
using DiGi.GLTF;
using DiGi.GLTF.Analytical;
using DiGi.GLTF.Classes;
using DiGi.WebAPI.Classes;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace DiGi.GIS.WebAPI.UI.Controllers
{
    /// <summary>
    /// Provides controller endpoints for accessing analytical <see cref="BuildingModel"/> data, acting as an interface between the client and the underlying GIS building data services.
    /// </summary>
    [Route("[controller]")]
    public class BuildingModelController : Controller
    {
        private readonly IHttpClientFactory httpClientFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="BuildingModelController"/> class.
        /// </summary>
        /// <param name="httpClientFactory">The <see cref="IHttpClientFactory"/> used to create and manage <see cref="HttpClient"/> instances for making API requests.</param>
        public BuildingModelController(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory;
        }

        /// <summary>
        /// Asynchronously loads a <see cref="BuildingModel"/> from the GIS Web API by searching for the building at the specified coordinates, converts its components into separate selectable <see cref="GLTFNode"/> instances and renders the 3D viewer page.
        /// <para>Of the models found at the point, the one whose stored reference is <paramref name="reference"/> is shown, so a neighbour within <see cref="Constants.Default.BuildingSearchRadius"/> is never shown in its place; without a match the first one found is, as before.</para>
        /// <para>The shown building is also resolved to its identifier and county (<see cref="Query.Building2DReferenceByPointAsync(HttpClient?, string?, int?, double?, double?, CancellationToken)"/>), concurrently with the model read. When it resolves, the page offers the "Solar radiation" panel and its component nodes take the root reference of <see cref="GetGLBBuildingModelByIdAsync(long, int?, double?, CancellationToken)"/>, so the solar results name them; otherwise the panel is left out and the model's stored reference is the root, as before.</para>
        /// </summary>
        /// <param name="reference">The reference of the building model.</param>
        /// <param name="x">The X coordinate of the building centroid.</param>
        /// <param name="y">The Y coordinate of the building centroid.</param>
        /// <param name="radius">The optional minimum view range in metres (the "View range" slider then spans it to <see cref="Constants.Default.BuildingViewRangeFactor"/> times it and the ground is loaded out to that maximum); when null, <see cref="Constants.Default.TerrainRadius"/> is used.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by the caller to cancel the asynchronous operation.</param>
        /// <returns>A <see cref="Task{IActionResult}"/> rendering the 3D glTF scene view or a not found response.</returns>
        [HttpGet("itembyreference")]
        public async Task<IActionResult> GetItemByReferenceAsync([FromQuery(Name = "reference")] string reference, [FromQuery(Name = "x")] double x, [FromQuery(Name = "y")] double y, [FromQuery(Name = "radius")] double? radius, CancellationToken cancellationToken = default)
        {
            if (!double.IsFinite(x) || !double.IsFinite(y))
            {
                return BadRequest();
            }

            Classes.ViewRange? viewRange = Create.BuildingViewRange(radius);
            if (viewRange is null || ModelState.GetValidationState("radius") == Microsoft.AspNetCore.Mvc.ModelBinding.ModelValidationState.Invalid)
            {
                return BadRequest($"The radius must be a positive number of meters not greater than {System.Math.Floor(Constants.Default.TerrainRadiusMax / Constants.Default.BuildingViewRangeFactor)} m.");
            }

            HttpClient httpClient = httpClientFactory.CreateClient();

            // The models at the point, the footprint at the point and the identifier of the requested building are read
            // concurrently.
            UrlBuilder urlBuilder = new($"{Constants.Default.GISWebAPIUri}/gis/buildingmodel/itemsbycircle");
            urlBuilder = urlBuilder.AddParameter("x", x);
            urlBuilder = urlBuilder.AddParameter("y", y);
            urlBuilder = urlBuilder.AddParameter("radius", Constants.Default.BuildingSearchRadius);
            urlBuilder = urlBuilder.AddParameter("tolerance", Constants.Default.BuildingSearchTolerance);

            Task<List<BuildingModel>?> task_BuildingModels = httpClient.ItemsAsync<BuildingModel>(urlBuilder.ToString(), cancellationToken);

            // The footprint standing at the same point carries the cadastral reference the details panel needs when the
            // building cannot be resolved below. The reference given by the caller is the last fallback, so a building
            // with no footprint stored still names something.
            urlBuilder = new($"{Constants.Default.GISWebAPIUri}/gis/building2D/itemsbycircle");
            urlBuilder = urlBuilder.AddParameter("x", x);
            urlBuilder = urlBuilder.AddParameter("y", y);
            urlBuilder = urlBuilder.AddParameter("radius", Constants.Default.BuildingSearchRadius);

            Task<GIS.Classes.Building2D?> task_Building2D = httpClient.ItemAsync<GIS.Classes.Building2D>(urlBuilder.ToString(), cancellationToken);

            // The identifier and county the solar routes need.
            Task<PostgreSQL.Classes.Building2DReference?> task_Building2DReference = httpClient.Building2DReferenceByPointAsync(reference, null, x, y, cancellationToken);

            List<BuildingModel>? buildingModels = await task_BuildingModels;
            BuildingModel? buildingModel = buildingModels?.Find(buildingModel_Temp => BuildingModelReference(TryGetReferenceText(buildingModel_Temp)) == BuildingModelReference(reference)) ?? buildingModels?.FirstOrDefault();
            if (buildingModel is null)
            {
                return NotFound();
            }

            GIS.Classes.Building2D? building2D = await task_Building2D;
            PostgreSQL.Classes.Building2DReference? building2DReference = await task_Building2DReference;

            // The caller named another building than the one shown (no model matched it): resolve the one shown.
            string? referenceText_BuildingModel = TryGetReferenceText(buildingModel);
            if (referenceText_BuildingModel is not null && BuildingModelReference(referenceText_BuildingModel) != BuildingModelReference(reference))
            {
                building2DReference = await httpClient.Building2DReferenceByPointAsync(referenceText_BuildingModel, null, x, y, cancellationToken);
            }

            ViewData["Building2DReference"] = building2DReference?.Reference ?? building2D?.Reference ?? reference;

            IReference? reference_BuildingModel = null;
            ViewModels.SolarSettingsViewModel? solarSettingsViewModel = null;
            if (building2DReference is not null && building2DReference.Id > 0)
            {
                // The root reference of buildingmodel/buildingmodelbyid, which the solar view references its surfaces by.
                // gis/buildingmodel/itemsbycircle stores the county-qualified (complex) reference on the model, while the
                // by-identifier read behind that page and the solar routes stores the plain one; the plain one is set on
                // this copy so the two roots are the same text.
                string? reference_Plain = BuildingModelReference(referenceText_BuildingModel);
                if (!string.IsNullOrWhiteSpace(reference_Plain) && reference_Plain != referenceText_BuildingModel)
                {
                    buildingModel.SetValue(Analytical.Enums.BuildingModelParameter.Reference, reference_Plain, new Core.Parameter.Classes.SetValueSettings(true, false));
                }

                reference_BuildingModel = PostgreSQL.Create.Reference(buildingModel, null, building2DReference.CountyId);
                solarSettingsViewModel = Create.SolarSettingsViewModel(building2DReference.Id, building2DReference.CountyId, radius);
            }
            else if (Core.Query.TryParse(referenceText_BuildingModel, out IReference? reference_Temp))
            {
                // Reuse the building's own stored reference so the rebuilt component nodes carry a fully-qualified
                // reference (building + county + component guid) rather than a bare component identifier.
                reference_BuildingModel = reference_Temp;
            }

            List<GLTFNode>? gLTFNodes = buildingModel.ToGLTF_GLTFNodes(reference_BuildingModel);
            if (gLTFNodes is null || gLTFNodes.Count == 0)
            {
                return NotFound();
            }

            Circle2D? circle2D_Terrain = buildingModel.TerrainCircle(minimumRadius: viewRange.Maximum);

            await AddTerrainAsync(gLTFNodes, httpClient, circle2D_Terrain, [buildingModel], cancellationToken);

            string name = $"BuildingModel {buildingModel.UniqueId}";

            GLTFScene? gLTFScene = GLTF.Create.GLTFScene(gLTFNodes, name);
            if (gLTFScene is null)
            {
                return NotFound();
            }

            ViewModels.GLTFSceneViewModel? gLTFSceneViewModel = gLTFScene.GLTFSceneViewModel(name, viewRange, solarSettingsViewModel);
            if (gLTFSceneViewModel is null)
            {
                return NotFound();
            }

            return View("~/Views/GLTF/GLTFSceneView.cshtml", gLTFSceneViewModel);
        }

        /// <summary>
        /// Renders the 3D viewer page for all buildings within the specified circular area. The page itself carries no geometry; the viewer streams the binary glTF payload from the glb endpoint.
        /// <para>The search is purely spatial: the area may span multiple counties, so no county identifier is required.</para>
        /// </summary>
        /// <param name="centerX">The X coordinate of the center of the search circle.</param>
        /// <param name="centerY">The Y coordinate of the center of the search circle.</param>
        /// <param name="radius">The radius of the search circle in meters.</param>
        /// <returns>An <see cref="IActionResult"/> rendering the glTF scene view.</returns>
        [HttpGet("itemsbyradius")]
        public IActionResult GetItemsByRadius([FromQuery(Name = "centerX")] double centerX, [FromQuery(Name = "centerY")] double centerY, [FromQuery(Name = "radius")] double radius)
        {
            if (double.IsNaN(centerX) || double.IsNaN(centerY) || double.IsNaN(radius) || radius <= 0 || radius > Constants.Default.DisplayRadiusMax)
            {
                return BadRequest();
            }

            string gLBUrl = $"~/buildingmodel/glb/buildingsbyradius?centerX={centerX.ToString(CultureInfo.InvariantCulture)}&centerY={centerY.ToString(CultureInfo.InvariantCulture)}&radius={radius.ToString(CultureInfo.InvariantCulture)}";

            string title = $"Buildings ({centerX}, {centerY}) r = {radius} m";

            // Multi-building default scope box: +-50 m in X/Y around the scene center; the viewer fits Z to the buildings' elevation.
            GLTFSceneViewModel gLTFSceneViewModel = new(title, gLBUrl, "50;50", Create.ViewRange(radius));

            return View("~/Views/GLTF/GLTFSceneView.cshtml", gLTFSceneViewModel);
        }

        /// <summary>
        /// Asynchronously retrieves all building models within the specified circular area from the PostgreSQL database via the GIS Web API, converts each <see cref="BuildingModel"/> into a batched <see cref="GLTFScene"/> with buildings selectable as whole envelopes and streams it as a binary glTF (.glb) payload.
        /// <para>The search is purely spatial: the area may span multiple counties, so no county identifier is required.</para>
        /// </summary>
        /// <param name="centerX">The X coordinate of the center of the search circle.</param>
        /// <param name="centerY">The Y coordinate of the center of the search circle.</param>
        /// <param name="radius">The radius of the search circle in meters.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by the caller to cancel the asynchronous operation.</param>
        /// <returns>A <see cref="Task{IActionResult}"/> holding the .glb file.</returns>
        [HttpGet("glb/buildingsbyradius")]
        public async Task<IActionResult> GetBuildingsGLBByRadiusAsync([FromQuery(Name = "centerX")] double centerX, [FromQuery(Name = "centerY")] double centerY, [FromQuery(Name = "radius")] double radius, CancellationToken cancellationToken = default)
        {
            if (double.IsNaN(centerX) || double.IsNaN(centerY) || double.IsNaN(radius) || radius <= 0 || radius > Constants.Default.DisplayRadiusMax)
            {
                return BadRequest();
            }

            HttpClient httpClient = httpClientFactory.CreateClient();

            List<BuildingModel>? buildingModels = await BuildingModelsByCircleAsync(httpClient, centerX, centerY, radius, cancellationToken);
            if (buildingModels is null || buildingModels.Count == 0)
            {
                return NoContent();
            }

            Circle2D circle2D_Search = new(new Point2D(centerX, centerY), radius);
            Circle2D? circle2D_Terrain = buildingModels.TerrainCircle(circle2D_Search);

            Task<GLTFNode?>? task_Terrain = Constants.Default.TerrainEnabled && circle2D_Terrain is not null
                ? httpClient.TerrainGLTFNodeAsync(circle2D_Terrain, cancellationToken: cancellationToken)
                : null;

            string name = $"Buildings ({centerX}, {centerY}) r = {radius} m";

            List<GLTFNode> gLTFNodes = EnvelopeGLTFNodes(buildingModels);

            if (gLTFNodes is null || gLTFNodes.Count == 0)
            {
                return NoContent();
            }

            // The ground is cut to the buildings that were actually fetched, so a building whose centre falls
            // outside the requested circle keeps the ground beneath it - it is not in the scene to reveal it.
            GLTFNode? gLTFNode_Terrain = task_Terrain is null ? null : await task_Terrain;
            gLTFNode_Terrain = gLTFNode_Terrain.TerrainGLTFNode(buildingModels, circle2D_Terrain);
            if (gLTFNode_Terrain is not null)
            {
                gLTFNodes.Add(gLTFNode_Terrain);
            }

            GLTFScene? gLTFScene = GLTF.Create.GLTFScene(gLTFNodes, name, referencePointOverride: new Point3D(centerX, centerY, 0));
            if (gLTFScene is null)
            {
                return NoContent();
            }

            byte[]? bytes = GLTF.Convert.ToSystem_Bytes(gLTFScene, true);
            if (bytes is null || bytes.Length == 0)
            {
                return NoContent();
            }

            return File(bytes, "model/gltf-binary", "buildings.glb");
        }

        /// <summary>
        /// Renders the 3D viewer page for a single building. The page itself carries no geometry; the viewer streams the binary glTF payload from the glb endpoint.
        /// <para>The page offers the "Solar radiation" panel (<see cref="SolarController.GetViewByBuildingModelIdAsync(long, int?, double?, CancellationToken)"/>), with <paramref name="radius"/> as the neighbour radius limited by <see cref="Query.SolarRadius(double?)"/>.</para>
        /// </summary>
        /// <param name="id">The unique identifier of the building.</param>
        /// <param name="countyId">The optional unique identifier of the county associated with the building.</param>
        /// <param name="radius">The optional minimum view range in metres (the "View range" slider then spans it to <see cref="Constants.Default.BuildingViewRangeFactor"/> times it and the ground is loaded out to that maximum); when null, <see cref="Constants.Default.TerrainRadius"/> is used.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by the caller to cancel the asynchronous operation.</param>
        /// <returns>An <see cref="IActionResult"/> rendering the glTF scene view.</returns>
        [HttpGet("buildingmodelbyid")]
        public async Task<IActionResult> GetBuildingModelByIdAsync([FromQuery(Name = "id")] long id, [FromQuery(Name = "countyid")] int? countyId, [FromQuery(Name = "radius")] double? radius, CancellationToken cancellationToken = default)
        {
            Classes.ViewRange? viewRange = Create.BuildingViewRange(radius);
            if (viewRange is null || ModelState.GetValidationState("radius") == Microsoft.AspNetCore.Mvc.ModelBinding.ModelValidationState.Invalid)
            {
                return BadRequest($"The radius must be a positive number of meters not greater than {System.Math.Floor(Constants.Default.TerrainRadiusMax / Constants.Default.BuildingViewRangeFactor)} m.");
            }

            HttpClient httpClient = httpClientFactory.CreateClient();

            UrlBuilder urlBuilder = new($"{Constants.Default.GISWebAPIUri}/gis/building2D/building2Dreferencebyid");
            urlBuilder = urlBuilder.AddParameter("id", id);
            if (countyId.HasValue)
            {
                urlBuilder = urlBuilder.AddParameter("countyid", countyId.Value);
            }

            // The cadastral reference is what the details panel of the viewer looks the building up by, so
            // the page carries it when it is known and simply omits the panel when it is not.
            PostgreSQL.Classes.Building2DReference? building2DReference = await httpClient.ItemAsync<PostgreSQL.Classes.Building2DReference>(urlBuilder.ToString(), cancellationToken);

            if (!string.IsNullOrEmpty(building2DReference?.Reference))
            {
                ViewData["Building2DReference"] = building2DReference.Reference;
            }

            string gLBUrl = $"~/buildingmodel/glb/buildingmodelbyid?id={id.ToString(CultureInfo.InvariantCulture)}";
            if (countyId.HasValue)
            {
                gLBUrl += $"&countyid={countyId.Value.ToString(CultureInfo.InvariantCulture)}";
            }

            if (radius.HasValue)
            {
                gLBUrl += $"&radius={radius.Value.ToString(CultureInfo.InvariantCulture)}";
            }

            // Surrounding elements are offered by the page but only fetched when the user first asks for them.
            string surroundingsGLBUrl = $"~/buildingmodel/glb/surroundingsbybuildingid?id={id.ToString(CultureInfo.InvariantCulture)}";
            if (countyId.HasValue)
            {
                surroundingsGLBUrl += $"&countyid={countyId.Value.ToString(CultureInfo.InvariantCulture)}";
            }

            GLTFSceneViewModel gLTFSceneViewModel = new($"BuildingModel {id}", gLBUrl, viewRange: viewRange, surroundingsGLBUrl: surroundingsGLBUrl, solarSettings: Create.SolarSettingsViewModel(id, countyId, radius));

            return View("~/Views/GLTF/GLTFSceneView.cshtml", gLTFSceneViewModel);
        }

        /// <summary>
        /// Asynchronously retrieves the 3D <see cref="BuildingModel"/> for the building with the specified unique identifier from the database (see <see cref="Query.BuildingModelAsync(HttpClient, long, int?, CancellationToken)"/>), converts each of its components (walls, floors and roofs) into a separate node of a batched <see cref="GLTFScene"/> (translated to a local origin) and streams it as a binary glTF (.glb) payload.
        /// <para>Each component carries its own identity in the scene object map, so the 3D viewer can hit-test and select individual components instead of the building as a whole.</para>
        /// </summary>
        /// <param name="id">The unique identifier of the building.</param>
        /// <param name="countyId">The optional unique identifier of the county associated with the building.</param>
        /// <param name="radius">The optional minimum view range in metres (the "View range" slider then spans it to <see cref="Constants.Default.BuildingViewRangeFactor"/> times it and the ground is loaded out to that maximum); when null, <see cref="Constants.Default.TerrainRadius"/> is used.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by the caller to cancel the asynchronous operation.</param>
        /// <returns>A <see cref="Task{IActionResult}"/> holding the .glb file.</returns>
        [HttpGet("glb/buildingmodelbyid")]
        public async Task<IActionResult> GetGLBBuildingModelByIdAsync([FromQuery(Name = "id")] long id, [FromQuery(Name = "countyid")] int? countyId, [FromQuery(Name = "radius")] double? radius, CancellationToken cancellationToken = default)
        {
            Classes.ViewRange? viewRange = Create.BuildingViewRange(radius);
            if (viewRange is null || ModelState.GetValidationState("radius") == Microsoft.AspNetCore.Mvc.ModelBinding.ModelValidationState.Invalid)
            {
                return BadRequest($"The radius must be a positive number of meters not greater than {System.Math.Floor(Constants.Default.TerrainRadiusMax / Constants.Default.BuildingViewRangeFactor)} m.");
            }

            HttpClient httpClient = httpClientFactory.CreateClient();

            BuildingModel? buildingModel = await httpClient.BuildingModelAsync(id, countyId, cancellationToken);
            if (buildingModel is null)
            {
                return NoContent();
            }

            // Give every component node a fully-qualified reference (building + county + component guid) so a
            // selected element can be traced back to its building; ToGLTF_GLTFNodes flattens the component step in.
            IReference? reference = PostgreSQL.Create.Reference(buildingModel, null, countyId);

            List<GLTFNode>? gLTFNodes = buildingModel.ToGLTF_GLTFNodes(reference);
            if (gLTFNodes is null || gLTFNodes.Count == 0)
            {
                return NoContent();
            }

            Circle2D? circle2D_Terrain = buildingModel.TerrainCircle(minimumRadius: viewRange.Maximum);

            await AddTerrainAsync(gLTFNodes, httpClient, circle2D_Terrain, [buildingModel], cancellationToken);

            string name = $"BuildingModel {id.ToString(CultureInfo.InvariantCulture)}";

            // An explicit local origin (the building centre) keeps this payload lined up with the surrounding elements, which are streamed separately.
            Point2D? center = buildingModel.TerrainCircle(0, 0)?.Center;

            GLTFScene? gLTFScene = GLTF.Create.GLTFScene(gLTFNodes, name, referencePointOverride: center is null ? null : new Point3D(center.X, center.Y, 0));
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

        /// <summary>
        /// Asynchronously retrieves the buildings surrounding the building with the specified unique identifier and streams them as a binary glTF (.glb) payload of non-selectable "surroundings" nodes (see <see cref="Constants.Default.SurroundingName"/>), which the Building Viewer loads lazily when the user asks to show the surrounding elements.
        /// <para>The buildings are converted at <see cref="BuildingModelDetailLevel.Envelope"/> detail, the target building itself and the terrain are left out, and the scene is translated to the same local origin as the one of <see cref="GetGLBBuildingModelByIdAsync(long, int?, double?, CancellationToken)"/>, so the two payloads line up.</para>
        /// </summary>
        /// <param name="id">The unique identifier of the target building.</param>
        /// <param name="countyId">The optional unique identifier of the county associated with the building.</param>
        /// <param name="radius">The optional search radius in metres around the building; when null, <see cref="Constants.Default.SurroundingRadius"/> is used.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by the caller to cancel the asynchronous operation.</param>
        /// <returns>A <see cref="Task{IActionResult}"/> holding the .glb file, or no content when the building has no neighbours.</returns>
        [HttpGet("glb/surroundingsbybuildingid")]
        public async Task<IActionResult> GetGLBSurroundingsByBuildingIdAsync([FromQuery(Name = "id")] long id, [FromQuery(Name = "countyid")] int? countyId, [FromQuery(Name = "radius")] double? radius, CancellationToken cancellationToken = default)
        {
            double radius_Search = radius ?? Constants.Default.SurroundingRadius;
            if (double.IsNaN(radius_Search) || radius_Search <= 0 || radius_Search > Constants.Default.DisplayRadiusMax || ModelState.GetValidationState("radius") == Microsoft.AspNetCore.Mvc.ModelBinding.ModelValidationState.Invalid)
            {
                return BadRequest($"The radius must be a positive number of meters not greater than {Constants.Default.DisplayRadiusMax} m.");
            }

            HttpClient httpClient = httpClientFactory.CreateClient();

            BuildingModel? buildingModel = await httpClient.BuildingModelAsync(id, countyId, cancellationToken);
            if (buildingModel is null)
            {
                return NoContent();
            }

            Point2D? center = buildingModel.TerrainCircle(0, 0)?.Center;
            if (center is null)
            {
                return NoContent();
            }

            List<BuildingModel>? buildingModels = await BuildingModelsByCircleAsync(httpClient, center.X, center.Y, radius_Search, cancellationToken);
            if (buildingModels is null || buildingModels.Count == 0)
            {
                return NoContent();
            }

            // The target building is part of the answer, and is matched by reference (falling back to its centre)
            // so that it is never drawn twice - once opaque and selectable, once as a surrounding element.
            string? referenceText_Target = TryGetReferenceText(buildingModel);
            List<BuildingModel> buildingModels_Surrounding = [];
            foreach (BuildingModel buildingModel_Temp in buildingModels)
            {
                string? referenceText = TryGetReferenceText(buildingModel_Temp);
                Point2D? center_Temp = buildingModel_Temp.TerrainCircle(0, 0)?.Center;
                bool isTarget = referenceText_Target is not null && referenceText is not null
                    ? referenceText == referenceText_Target
                    : center_Temp is not null && center_Temp.Distance(center) < Constants.Default.BuildingSearchTolerance;

                if (!isTarget)
                {
                    buildingModels_Surrounding.Add(buildingModel_Temp);
                }
            }

            List<GLTFNode> gLTFNodes = [];
            foreach (GLTFNode gLTFNode in EnvelopeGLTFNodes(buildingModels_Surrounding))
            {
                gLTFNodes.Add(new GLTFNode(Constants.Default.SurroundingName, gLTFNode.Reference, gLTFNode.Mesh3D, gLTFNode.Color, gLTFNode.Opacity, gLTFNode.Properties));
            }

            if (gLTFNodes.Count == 0)
            {
                return NoContent();
            }

            string name = $"Surroundings {id.ToString(CultureInfo.InvariantCulture)}";

            GLTFScene? gLTFScene = GLTF.Create.GLTFScene(gLTFNodes, name, referencePointOverride: new Point3D(center.X, center.Y, 0));
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

        /// <summary>
        /// Handles the HTTP GET request to the root endpoint and returns the 3D viewer landing page.
        /// </summary>
        /// <returns>An <see cref="IActionResult"/> representing the start view.</returns>
        [HttpGet("")]
        public IActionResult Start()
        {
            return View("~/Views/GLTF/Start.cshtml");
        }

        /// <summary>
        /// Asynchronously retrieves the <see cref="BuildingModel"/> items whose position lies within the given circle from the GIS Web API.
        /// </summary>
        /// <param name="httpClient">The HTTP client used for the request.</param>
        /// <param name="centerX">The X coordinate of the center of the search circle.</param>
        /// <param name="centerY">The Y coordinate of the center of the search circle.</param>
        /// <param name="radius">The radius of the search circle in meters.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by the caller to cancel the asynchronous operation.</param>
        /// <returns>The buildings found, or <see langword="null"/> when the request fails.</returns>
        private static async Task<List<BuildingModel>?> BuildingModelsByCircleAsync(HttpClient httpClient, double centerX, double centerY, double radius, CancellationToken cancellationToken)
        {
            UrlBuilder urlBuilder = new($"{Constants.Default.GISWebAPIUri}/gis/buildingmodel/itemsbycircle");
            urlBuilder = urlBuilder.AddParameter("x", centerX);
            urlBuilder = urlBuilder.AddParameter("y", centerY);
            urlBuilder = urlBuilder.AddParameter("radius", radius);

            return await httpClient.ItemsAsync<BuildingModel>(urlBuilder.ToString(), cancellationToken);
        }

        /// <summary>
        /// Converts each of the given buildings into <see cref="GLTFNode"/> instances at <see cref="BuildingModelDetailLevel.Envelope"/> detail, referenced by the reference stored on the building.
        /// </summary>
        /// <param name="buildingModels">The buildings to convert.</param>
        /// <returns>The nodes of all the buildings, in world coordinates. The list is empty when nothing could be converted.</returns>
        private static List<GLTFNode> EnvelopeGLTFNodes(IEnumerable<BuildingModel> buildingModels)
        {
            List<GLTFNode> result = [];
            foreach (BuildingModel buildingModel in buildingModels)
            {
                IReference? reference = null;
                string? referenceText = TryGetReferenceText(buildingModel);
                if (referenceText is not null && Core.Query.TryParse(referenceText, out IReference? reference_Temp))
                {
                    reference = reference_Temp;
                }

                List<GLTFNode>? gLTFNodes = buildingModel.ToGLTF_GLTFNodes(reference, Core.Constants.Tolerance.Distance, BuildingModelDetailLevel.Envelope);
                if (gLTFNodes is not null)
                {
                    result.AddRange(gLTFNodes);
                }
            }

            return result;
        }

        /// <summary>
        /// Gets the plain building reference of a reference text, unwrapping a ComplexReference that carries it together with its county (see <c>PostgreSQL.Create.Reference</c>), so two forms of the same building compare equal.
        /// </summary>
        /// <param name="referenceText">The reference text, plain or complex. This value can be null.</param>
        /// <returns>The plain building reference, or the text itself when it holds none.</returns>
        private static string? BuildingModelReference(string? referenceText)
        {
            if (PostgreSQL.Query.TryParse(referenceText, out string buildingModelReference, out _, out _) && !string.IsNullOrWhiteSpace(buildingModelReference))
            {
                return buildingModelReference;
            }

            return referenceText;
        }

        /// <summary>
        /// Reads the reference text stored on a building.
        /// </summary>
        /// <param name="buildingModel">The building.</param>
        /// <returns>The reference text, or <see langword="null"/> when the building carries none.</returns>
        private static string? TryGetReferenceText(BuildingModel buildingModel)
        {
            return buildingModel.TryGetValue<string>(Analytical.Enums.BuildingModelParameter.Reference, out string? referenceText) ? referenceText : null;
        }

        /// <summary>
        /// Adds the ground surface around the given circular area to the nodes of a scene, with the outlines of the buildings of the scene cut out of it.
        /// <para>The surface is optional: no stored elevation points, an undeployed or unreachable terrain service and a timeout all leave the scene exactly as it was, so a building scene never depends on terrain being there.</para>
        /// </summary>
        /// <param name="gLTFNodes">The nodes of the scene being built.</param>
        /// <param name="httpClient">The HTTP client used for the request.</param>
        /// <param name="circle2D">The circular area of ground to show, in PL-1992 (EPSG:2180) metres. This value can be null.</param>
        /// <param name="buildingModels">The buildings of the scene, whose outlines are cut out of the ground so it does not run through their interiors (see <see cref="Create.TerrainGLTFNode(GLTFNode?, IEnumerable{BuildingModel}?, Circle2D?, double, double)"/>). This value can be null.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by the caller to cancel the asynchronous operation.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        private static async Task AddTerrainAsync(List<GLTFNode> gLTFNodes, HttpClient httpClient, Circle2D? circle2D, IEnumerable<BuildingModel>? buildingModels, CancellationToken cancellationToken)
        {
            if (!Constants.Default.TerrainEnabled || circle2D is null)
            {
                return;
            }

            GLTFNode? gLTFNode = await httpClient.TerrainGLTFNodeAsync(circle2D, cancellationToken: cancellationToken);

            gLTFNode = gLTFNode.TerrainGLTFNode(buildingModels, circle2D);
            if (gLTFNode is not null)
            {
                gLTFNodes.Add(gLTFNode);
            }
        }
    }
}