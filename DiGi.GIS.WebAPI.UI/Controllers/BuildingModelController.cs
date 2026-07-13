using DiGi.Analytical.Building.Classes;
using DiGi.GIS.WebAPI.UI.ViewModels;
using DiGi.GLTF;
using DiGi.GLTF.Analytical;
using DiGi.GLTF.Analytical.Enums;
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
        /// Asynchronously creates a <see cref="BuildingModel"/> for the building with the specified unique identifier (see <see cref="Create.BuildingModelAsync(HttpClient?, long, int?, double, double)"/>) and returns it as JSON.
        /// </summary>
        /// <param name="id">The unique identifier of the building.</param>
        /// <param name="countyId">The optional unique identifier of the county associated with the building.</param>
        /// <returns>A <see cref="Task{IActionResult}"/> holding the serialized <see cref="BuildingModel"/>.</returns>
        [HttpGet("itembyid")]
        public async Task<IActionResult> GetItemByIdAsync([FromQuery(Name = "id")] long id, [FromQuery(Name = "countyid")] int? countyId)
        {
            HttpClient httpClient = httpClientFactory.CreateClient();

            BuildingModel? buildingModel = await httpClient.BuildingModelAsync(id, countyId);
            if (buildingModel is null)
            {
                return NoContent();
            }

            string json = Core.Convert.ToSystem_String(buildingModel) ?? string.Empty;
            if (string.IsNullOrWhiteSpace(json))
            {
                return NotFound();
            }

            return Content(json, "application/json");
        }

        /// <summary>
        /// Asynchronously loads a <see cref="BuildingModel"/> from the GIS Web API by searching for the building at the specified coordinates, converts its components into separate selectable <see cref="GLTFNode"/> instances and renders the 3D viewer page.
        /// </summary>
        /// <param name="id">The unique identifier of the building model (used as the page title).</param>
        /// <param name="x">The X coordinate of the building centroid.</param>
        /// <param name="y">The Y coordinate of the building centroid.</param>
        /// <returns>A <see cref="Task{IActionResult}"/> rendering the 3D glTF scene view or a not found response.</returns>
        [HttpGet("detailsbyid")]
        public async Task<IActionResult> GetDetailsByIdAsync([FromQuery(Name = "id")] string id, [FromQuery(Name = "x")] double x, [FromQuery(Name = "y")] double y)
        {
            if (double.IsNaN(x) || double.IsNaN(y))
            {
                return BadRequest();
            }

            HttpClient httpClient = httpClientFactory.CreateClient();

            UrlBuilder urlBuilder = new("https://api.digiproject.uk/gis/buildingmodel/itemsbycircle");
            urlBuilder = urlBuilder.AddParameter("x", x);
            urlBuilder = urlBuilder.AddParameter("y", y);
            urlBuilder = urlBuilder.AddParameter("radius", 5);
            urlBuilder = urlBuilder.AddParameter("tolerance", 5);

            HttpResponseMessage httpResponseMessage = await httpClient.GetAsync(urlBuilder.ToString());
            if (!httpResponseMessage.IsSuccessStatusCode)
            {
                return NotFound();
            }

            string json = await httpResponseMessage.Content.ReadAsStringAsync();
            if (string.IsNullOrWhiteSpace(json))
            {
                return NotFound();
            }

            List<BuildingModel>? buildingModels = Core.Convert.ToDiGi<BuildingModel>(json);
            BuildingModel? buildingModel = buildingModels?.FirstOrDefault();
            if (buildingModel is null)
            {
                return NotFound();
            }

            List<GLTFNode>? gLTFNodes = buildingModel.ToGLTF_GLTFNodes();
            if (gLTFNodes is null || gLTFNodes.Count == 0)
            {
                return NotFound();
            }

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
        /// Renders the 3D viewer page for all building models within the specified circular area. The page itself carries no geometry; the viewer streams the binary glTF payload from the glb endpoint.
        /// <para>The search is purely spatial: the area may span multiple counties, so no county identifier is required.</para>
        /// </summary>
        /// <param name="centerX">The X coordinate of the center of the search circle.</param>
        /// <param name="centerY">The Y coordinate of the center of the search circle.</param>
        /// <param name="radius">The radius of the search circle in meters.</param>
        /// <param name="storeyHeight">The optional storey height in meters used for the extrusions.</param>
        /// <returns>A <see cref="IActionResult"/> rendering the glTF scene view.</returns>
        [HttpGet("/buildingmodel/buildingsbyradius")]
        public IActionResult GetBuildingsByRadius([FromQuery(Name = "centerX")] double centerX, [FromQuery(Name = "centerY")] double centerY, [FromQuery(Name = "radius")] double radius, [FromQuery(Name = "storeyheight")] double? storeyHeight = null)
        {
            if (double.IsNaN(centerX) || double.IsNaN(centerY) || double.IsNaN(radius) || radius <= 0)
            {
                return BadRequest();
            }

            string gLBUrl = $"~/buildingmodel/glb/buildingsbyradius?centerX={centerX.ToString(CultureInfo.InvariantCulture)}&centerY={centerY.ToString(CultureInfo.InvariantCulture)}&radius={radius.ToString(CultureInfo.InvariantCulture)}";
            if (storeyHeight is not null && storeyHeight.HasValue)
            {
                gLBUrl += $"&storeyheight={storeyHeight.Value.ToString(CultureInfo.InvariantCulture)}";
            }

            string title = $"Building Models ({centerX}, {centerY}) r = {radius} m";

            GLTFSceneViewModel gLTFSceneViewModel = new(title, gLBUrl);

            return View("~/Views/GLTF/GLTFSceneView.cshtml", gLTFSceneViewModel);
        }

        /// <summary>
        /// Asynchronously retrieves all building models within the specified circular area from the PostgreSQL database via the GIS Web API, converts each <see cref="BuildingModel"/> into a batched <see cref="GLTFScene"/> with buildings selectable as whole envelopes and streams it as a binary glTF (.glb) payload.
        /// <para>The search is purely spatial: the area may span multiple counties, so no county identifier is required.</para>
        /// </summary>
        /// <param name="centerX">The X coordinate of the center of the search circle.</param>
        /// <param name="centerY">The Y coordinate of the center of the search circle.</param>
        /// <param name="radius">The radius of the search circle in meters.</param>
        /// <param name="storeyHeight">The optional storey height in meters used for the extrusions.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by the caller to cancel the asynchronous operation.</param>
        /// <returns>A <see cref="Task{IActionResult}"/> holding the .glb file.</returns>
        [HttpGet("/buildingmodel/glb/buildingsbyradius")]
        public async Task<IActionResult> GetBuildingsGLBByRadiusAsync([FromQuery(Name = "centerX")] double centerX, [FromQuery(Name = "centerY")] double centerY, [FromQuery(Name = "radius")] double radius, [FromQuery(Name = "storeyheight")] double? storeyHeight = null, CancellationToken cancellationToken = default)
        {
            if (double.IsNaN(centerX) || double.IsNaN(centerY) || double.IsNaN(radius) || radius <= 0)
            {
                return BadRequest();
            }

            HttpClient httpClient = httpClientFactory.CreateClient();

            UrlBuilder urlBuilder = new("https://api.digiproject.uk/gis/buildingmodel/itemsbycircle");
            urlBuilder = urlBuilder.AddParameter("x", centerX);
            urlBuilder = urlBuilder.AddParameter("y", centerY);
            urlBuilder = urlBuilder.AddParameter("radius", radius);
            if (storeyHeight is not null && storeyHeight.HasValue)
            {
                urlBuilder = urlBuilder.AddParameter("storeyheight", storeyHeight.Value);
            }

            HttpResponseMessage httpResponseMessage = await httpClient.GetAsync(urlBuilder.ToString(), cancellationToken);
            if (!httpResponseMessage.IsSuccessStatusCode)
            {
                return BadRequest();
            }

            string json = await httpResponseMessage.Content.ReadAsStringAsync(cancellationToken);
            if (string.IsNullOrWhiteSpace(json))
            {
                return NoContent();
            }

            List<BuildingModel>? buildingModels = Core.Convert.ToDiGi<BuildingModel>(json);
            if (buildingModels is null || buildingModels.Count == 0)
            {
                return NoContent();
            }

            List<GLTFNode> gLTFNodes = [];
            foreach (BuildingModel buildingModel in buildingModels)
            {
                List<GLTFNode>? gLTFNodes_Temp = buildingModel.ToGLTF_GLTFNodes(Core.Constants.Tolerance.Distance, BuildingDisplayMode.Envelope);
                if (gLTFNodes_Temp is not null)
                {
                    gLTFNodes.AddRange(gLTFNodes_Temp);
                }
            }

            if (gLTFNodes is null || gLTFNodes.Count == 0)
            {
                return NoContent();
            }

            string name = $"Building Models ({centerX}, {centerY}) r = {radius} m";

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

            return File(bytes, "model/gltf-binary", "buildings.glb");
        }

        /// <summary>
        /// Handles the HTTP GET request to the root endpoint and returns the starting view for building data operations.
        /// </summary>
        /// <returns>An <see cref="IActionResult"/> representing the start view.</returns>
        [HttpGet("")]
        public IActionResult Start()
        {
            return View();
        }
    }
}
