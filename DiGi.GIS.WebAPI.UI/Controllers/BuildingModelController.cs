using DiGi.Analytical.Building.Classes;
using DiGi.Analytical.Building.Enums;
using DiGi.Core.Interfaces;
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
        /// <returns>A <see cref="Task{IActionResult}"/> rendering the 3D glTF scene view or a not found response.</returns>
        [HttpGet("itembyreference")]
        public async Task<IActionResult> GetItemByReferenceAsync([FromQuery(Name = "reference")] string reference, [FromQuery(Name = "x")] double x, [FromQuery(Name = "y")] double y)
        {
            if (double.IsNaN(x) || double.IsNaN(y))
            {
                return BadRequest();
            }

            HttpClient httpClient = httpClientFactory.CreateClient();

            string? building2DReference = null;
            try
            {
                UrlBuilder building2DUrlBuilder = new("https://api.digiproject.uk/gis/building2D/itemsbycircle");
                building2DUrlBuilder = building2DUrlBuilder.AddParameter("x", x);
                building2DUrlBuilder = building2DUrlBuilder.AddParameter("y", y);
                building2DUrlBuilder = building2DUrlBuilder.AddParameter("radius", 5);

                HttpResponseMessage building2DResponse = await httpClient.GetAsync(building2DUrlBuilder.ToString());
                if (building2DResponse.IsSuccessStatusCode)
                {
                    string building2DJson = await building2DResponse.Content.ReadAsStringAsync();
                    if (!string.IsNullOrWhiteSpace(building2DJson))
                    {
                        List<GIS.Classes.Building2D>? building2Ds = Core.Convert.ToDiGi<GIS.Classes.Building2D>(building2DJson);
                        if (building2Ds is not null && building2Ds.Count > 0)
                        {
                            building2DReference = building2Ds[0].Reference;
                        }
                    }
                }
            }
            catch
            {
                // Fallback to reference parameter
            }

            ViewData["Building2DReference"] = building2DReference ?? reference;

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

            UrlBuilder urlBuilder = new("https://api.digiproject.uk/gis/buildingmodel/itemsbycircle");
            urlBuilder = urlBuilder.AddParameter("x", centerX);
            urlBuilder = urlBuilder.AddParameter("y", centerY);
            urlBuilder = urlBuilder.AddParameter("radius", radius);

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
        /// <returns>An <see cref="IActionResult"/> rendering the glTF scene view.</returns>
        [HttpGet("buildingmodelbyid")]
        public async Task<IActionResult> GetBuildingModelByIdAsync([FromQuery(Name = "id")] long id, [FromQuery(Name = "countyid")] int? countyId)
        {
            HttpClient httpClient = httpClientFactory.CreateClient();

            DiGi.WebAPI.Classes.UrlBuilder urlBuilder = new("https://api.digiproject.uk/gis/building2D/building2Dreferencebyid");
            urlBuilder = urlBuilder.AddParameter("id", id);
            if (countyId is not null && countyId.HasValue)
            {
                urlBuilder = urlBuilder.AddParameter("countyId", countyId.Value);
            }

            string? referenceString = null;
            HttpResponseMessage httpResponseMessage = await httpClient.GetAsync(urlBuilder.ToString());
            if (httpResponseMessage.IsSuccessStatusCode)
            {
                string json = await httpResponseMessage.Content.ReadAsStringAsync();
                if (!string.IsNullOrWhiteSpace(json))
                {
                    List<DiGi.GIS.PostgreSQL.Classes.Building2DReference>? building2DReferences = Core.Convert.ToDiGi<DiGi.GIS.PostgreSQL.Classes.Building2DReference>(json);
                    if (building2DReferences is not null && building2DReferences.Count > 0)
                    {
                        referenceString = building2DReferences[0].Reference;
                    }
                }
            }

            if (!string.IsNullOrEmpty(referenceString))
            {
                ViewData["Building2DReference"] = referenceString;
            }

            string gLBUrl = $"~/buildingmodel/glb/buildingmodelbyid?id={id.ToString(CultureInfo.InvariantCulture)}";
            if (countyId is not null && countyId.HasValue)
            {
                gLBUrl += $"&countyid={countyId.Value.ToString(CultureInfo.InvariantCulture)}";
            }

            GLTFSceneViewModel gLTFSceneViewModel = new($"BuildingModel {id}", gLBUrl);

            return View("~/Views/GLTF/GLTFSceneView.cshtml", gLTFSceneViewModel);
        }

        /// <summary>
        /// Asynchronously creates a <see cref="BuildingModel"/> for the building with the specified unique identifier (see <see cref="Create.BuildingModelAsync(HttpClient?, long, int?, double, double)"/>), converts each of its components (walls, floors and roofs) into a separate node of a batched <see cref="GLTFScene"/> (translated to a local origin) and streams it as a binary glTF (.glb) payload.
        /// <para>Each component carries its own identity in the scene object map, so the 3D viewer can hit-test and select individual components instead of the building as a whole.</para>
        /// </summary>
        /// <param name="id">The unique identifier of the building.</param>
        /// <param name="countyId">The optional unique identifier of the county associated with the building.</param>
        /// <returns>A <see cref="Task{IActionResult}"/> holding the .glb file.</returns>
        [HttpGet("glb/buildingmodelbyid")]
        public async Task<IActionResult> GetGLBBuildingModelByIdAsync([FromQuery(Name = "id")] long id, [FromQuery(Name = "countyid")] int? countyId)
        {
            HttpClient httpClient = httpClientFactory.CreateClient();

            // The storey height is a server side concern: it is always the application default, never a client input.
            BuildingModel? buildingModel = await httpClient.BuildingModelAsync(id, countyId, Constants.Default.StoreyHeight);
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
    }
}