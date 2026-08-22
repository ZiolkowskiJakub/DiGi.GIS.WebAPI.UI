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
        /// </summary>
        /// <param name="reference">The reference of the building model.</param>
        /// <param name="x">The X coordinate of the building centroid.</param>
        /// <param name="y">The Y coordinate of the building centroid.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by the caller to cancel the asynchronous operation.</param>
        /// <returns>A <see cref="Task{IActionResult}"/> rendering the 3D glTF scene view or a not found response.</returns>
        [HttpGet("itembyreference")]
        public async Task<IActionResult> GetItemByReferenceAsync([FromQuery(Name = "reference")] string reference, [FromQuery(Name = "x")] double x, [FromQuery(Name = "y")] double y, CancellationToken cancellationToken = default)
        {
            if (!double.IsFinite(x) || !double.IsFinite(y))
            {
                return BadRequest();
            }

            HttpClient httpClient = httpClientFactory.CreateClient();

            #region Building2DReference

            // The footprint standing at the same point carries the cadastral reference the details panel
            // needs, which the reference of the model is not. The reference given by the caller is kept as
            // the fallback, so a building with no footprint stored still names something.
            UrlBuilder urlBuilder = new($"{Constants.Default.GISWebAPIUri}/gis/building2D/itemsbycircle");
            urlBuilder = urlBuilder.AddParameter("x", x);
            urlBuilder = urlBuilder.AddParameter("y", y);
            urlBuilder = urlBuilder.AddParameter("radius", Constants.Default.BuildingSearchRadius);

            GIS.Classes.Building2D? building2D = await httpClient.ItemAsync<GIS.Classes.Building2D>(urlBuilder.ToString(), cancellationToken);

            ViewData["Building2DReference"] = building2D?.Reference ?? reference;

            #endregion Building2DReference

            #region BuildingModel

            urlBuilder = new($"{Constants.Default.GISWebAPIUri}/gis/buildingmodel/itemsbycircle");
            urlBuilder = urlBuilder.AddParameter("x", x);
            urlBuilder = urlBuilder.AddParameter("y", y);
            urlBuilder = urlBuilder.AddParameter("radius", Constants.Default.BuildingSearchRadius);
            urlBuilder = urlBuilder.AddParameter("tolerance", Constants.Default.BuildingSearchTolerance);

            BuildingModel? buildingModel = await httpClient.ItemAsync<BuildingModel>(urlBuilder.ToString(), cancellationToken);
            if (buildingModel is null)
            {
                return NotFound();
            }

            #endregion BuildingModel

            // Reuse the building's own stored reference so the rebuilt component nodes carry a fully-qualified
            // reference (building + county + component guid) rather than a bare component identifier.
            IReference? reference_BuildingModel = null;
            if (buildingModel.TryGetValue<string>(Analytical.Enums.BuildingModelParameter.Reference, out string? referenceText) && Core.Query.TryParse(referenceText, out IReference? reference_Temp))
            {
                reference_BuildingModel = reference_Temp;
            }

            List<GLTFNode>? gLTFNodes = buildingModel.ToGLTF_GLTFNodes(reference_BuildingModel);
            if (gLTFNodes is null || gLTFNodes.Count == 0)
            {
                return NotFound();
            }

            Circle2D? circle2D_Terrain = buildingModel.TerrainCircle();

            await AddTerrainAsync(gLTFNodes, httpClient, circle2D_Terrain, [buildingModel], cancellationToken);

            string name = $"BuildingModel {buildingModel.UniqueId}";

            GLTFScene? gLTFScene = GLTF.Create.GLTFScene(gLTFNodes, name);
            if (gLTFScene is null)
            {
                return NotFound();
            }

            ViewModels.GLTFSceneViewModel? gLTFSceneViewModel = gLTFScene.GLTFSceneViewModel(name);
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
            if (double.IsNaN(centerX) || double.IsNaN(centerY) || double.IsNaN(radius) || radius <= 0)
            {
                return BadRequest();
            }

            string gLBUrl = $"~/buildingmodel/glb/buildingsbyradius?centerX={centerX.ToString(CultureInfo.InvariantCulture)}&centerY={centerY.ToString(CultureInfo.InvariantCulture)}&radius={radius.ToString(CultureInfo.InvariantCulture)}";

            string title = $"Buildings ({centerX}, {centerY}) r = {radius} m";

            // Multi-building default scope box: +-50 m in X/Y around the scene center, Z from -1 to 49.
            GLTFSceneViewModel gLTFSceneViewModel = new(title, gLBUrl, "50;50;-1;49");

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
            if (double.IsNaN(centerX) || double.IsNaN(centerY) || double.IsNaN(radius) || radius <= 0)
            {
                return BadRequest();
            }

            HttpClient httpClient = httpClientFactory.CreateClient();

            UrlBuilder urlBuilder = new($"{Constants.Default.GISWebAPIUri}/gis/buildingmodel/itemsbycircle");
            urlBuilder = urlBuilder.AddParameter("x", centerX);
            urlBuilder = urlBuilder.AddParameter("y", centerY);
            urlBuilder = urlBuilder.AddParameter("radius", radius);

            List<BuildingModel>? buildingModels = await httpClient.ItemsAsync<BuildingModel>(urlBuilder.ToString(), cancellationToken);
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

            List<GLTFNode> gLTFNodes = [];
            foreach (BuildingModel buildingModel in buildingModels)
            {
                IReference? reference = null;
                if(buildingModel.TryGetValue<string>(Analytical.Enums.BuildingModelParameter.Reference, out string? referenceText) && Core.Query.TryParse(referenceText, out IReference? reference_Temp))
                {
                    reference = reference_Temp;
                }

                List<GLTFNode>? gLTFNodes_Temp = buildingModel.ToGLTF_GLTFNodes(reference, Core.Constants.Tolerance.Distance, BuildingModelDetailLevel.Envelope);
                if (gLTFNodes_Temp is not null)
                {
                    gLTFNodes.AddRange(gLTFNodes_Temp);
                }
            }

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
        /// </summary>
        /// <param name="id">The unique identifier of the building.</param>
        /// <param name="countyId">The optional unique identifier of the county associated with the building.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by the caller to cancel the asynchronous operation.</param>
        /// <returns>An <see cref="IActionResult"/> rendering the glTF scene view.</returns>
        [HttpGet("buildingmodelbyid")]
        public async Task<IActionResult> GetBuildingModelByIdAsync([FromQuery(Name = "id")] long id, [FromQuery(Name = "countyid")] int? countyId, CancellationToken cancellationToken = default)
        {
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

            GLTFSceneViewModel gLTFSceneViewModel = new($"BuildingModel {id}", gLBUrl);

            return View("~/Views/GLTF/GLTFSceneView.cshtml", gLTFSceneViewModel);
        }

        /// <summary>
        /// Asynchronously retrieves the 3D <see cref="BuildingModel"/> for the building with the specified unique identifier from the database (see <see cref="Query.BuildingModelAsync(HttpClient, long, int?, CancellationToken)"/>), converts each of its components (walls, floors and roofs) into a separate node of a batched <see cref="GLTFScene"/> (translated to a local origin) and streams it as a binary glTF (.glb) payload.
        /// <para>Each component carries its own identity in the scene object map, so the 3D viewer can hit-test and select individual components instead of the building as a whole.</para>
        /// </summary>
        /// <param name="id">The unique identifier of the building.</param>
        /// <param name="countyId">The optional unique identifier of the county associated with the building.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by the caller to cancel the asynchronous operation.</param>
        /// <returns>A <see cref="Task{IActionResult}"/> holding the .glb file.</returns>
        [HttpGet("glb/buildingmodelbyid")]
        public async Task<IActionResult> GetGLBBuildingModelByIdAsync([FromQuery(Name = "id")] long id, [FromQuery(Name = "countyid")] int? countyId, CancellationToken cancellationToken = default)
        {
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

            Circle2D? circle2D_Terrain = buildingModel.TerrainCircle();

            await AddTerrainAsync(gLTFNodes, httpClient, circle2D_Terrain, [buildingModel], cancellationToken);

            string name = $"BuildingModel {id.ToString(CultureInfo.InvariantCulture)}";

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