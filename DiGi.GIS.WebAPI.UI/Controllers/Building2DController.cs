using DiGi.Analytical.Building.Classes;
using DiGi.Geometry.Planar.Classes;
using DiGi.Geometry.Spatial.Classes;
using DiGi.GIS.PostgreSQL.Classes;
using DiGi.GIS.WebAPI.UI.ViewModels;
using DiGi.GLTF;
using DiGi.GLTF.Analytical;
using DiGi.GLTF.Analytical.Enums;
using DiGi.GLTF.Classes;
using DiGi.WebAPI.Classes;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;

namespace DiGi.GIS.WebAPI.UI.Controllers
{
    /// <summary>
    /// Provides API endpoints for managing and retrieving building 2D information.
    /// </summary>
    [Route("[controller]")]
    public class Building2DController : Controller
    {
        private readonly IHttpClientFactory httpClientFactory;

        // Constructor injection for the PostgreSQL data source
        /// <summary>
        /// Initializes a new instance of the <see cref="Building2DController"/> class.
        /// </summary>
        /// <param name="httpClientFactory">The <see cref="IHttpClientFactory"/> used to create <see cref="HttpClient"/> instances.</param>
        public Building2DController(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory;
        }

        /// <summary>
        /// Asynchronously retrieves building 2D references associated with the specified administrative areal 2D identifier.
        /// </summary>
        /// <param name="administrativeAreal2DId">The administrative areal 2D identifier to filter by.</param>
        /// <returns>A <see cref="Task{IActionResult}"/> representing the asynchronous operation.</returns>
        [HttpGet("building2Dreferencesbyadministrativeareal2Did")]
        public async Task<IActionResult> GetBuilding2DReferencesByAdministrativeAreal2DIdAsync([FromQuery(Name = "administrativeareal2Did")] int administrativeAreal2DId)
        {
            HttpClient httpClient = httpClientFactory.CreateClient();

            UrlBuilder urlBuilder = new("https://api.digiproject.uk/gis/building2D/building2Dreferencesbyadministrativeareal2Did");
            urlBuilder = urlBuilder.AddParameter("administrativeareal2Did", administrativeAreal2DId);

            HttpResponseMessage httpResponseMessage = await httpClient.GetAsync(urlBuilder.ToString());
            if (!httpResponseMessage.IsSuccessStatusCode)
            {
                return BadRequest();
            }

            string json = await httpResponseMessage.Content.ReadAsStringAsync();
            if (string.IsNullOrWhiteSpace(json))
            {
                return NoContent();
            }

            // Here we use your DLL to turn JSON back into real C# objects.
            // Note: Since AdministrativeAreal2D is abstract,
            // you might need a specific converter or a concrete type.
            List<Building2DReference>? building2DReferences = Core.Convert.ToDiGi<Building2DReference>(json);

            // We pass the objects to a View
            return View("Building2DReferencesView", new Building2DReferencesViewModel(building2DReferences));
        }

        /// <summary>
        /// Renders the standalone building details page for the specified building reference (used e.g. by the "Show" button of the 3D viewer "Details" panel).
        /// <para>The partial view _Building2DView cannot be rendered on its own: it depends on the scripts, styles and AJAX loading logic of the references master-detail layout. The building context is therefore injected into <c>Building2DDetailsView</c>, which renders only the details side and loads the partial through the same AJAX pipeline.</para>
        /// <para>The building data is partitioned per county, so the by-reference lookup requires a county identifier. When it is not provided, it is resolved from the optional <paramref name="x"/>/<paramref name="y"/> point (e.g. the building centroid known to the 3D viewer).</para>
        /// </summary>
        /// <param name="reference">The unique reference string of the building.</param>
        /// <param name="countyId">The optional unique identifier of the county associated with the building.</param>
        /// <param name="x">The optional X coordinate of a point inside the building used to resolve the county when <paramref name="countyId"/> is not provided.</param>
        /// <param name="y">The optional Y coordinate of a point inside the building used to resolve the county when <paramref name="countyId"/> is not provided.</param>
        /// <returns>A <see cref="Task{IActionResult}"/> rendering the building details page.</returns>
        [HttpGet("detailsbyreference")]
        public async Task<IActionResult> GetDetailsByReferenceAsync([FromQuery(Name = "reference")] string? reference, [FromQuery(Name = "countyid")] int? countyId = null, [FromQuery(Name = "x")] double? x = null, [FromQuery(Name = "y")] double? y = null)
        {
            if (string.IsNullOrWhiteSpace(reference))
            {
                return BadRequest();
            }

            HttpClient httpClient = httpClientFactory.CreateClient();

            UrlBuilder urlBuilder;
            HttpResponseMessage httpResponseMessage;
            string json;

            #region CountyId by point

            if (countyId is null && x is not null && y is not null && !double.IsNaN(x.Value) && !double.IsNaN(y.Value))
            {
                urlBuilder = new("https://api.digiproject.uk/gis/administrativeareal2D/itemsbypoint");
                urlBuilder = urlBuilder.AddParameter("x", x.Value);
                urlBuilder = urlBuilder.AddParameter("y", y.Value);
                urlBuilder = urlBuilder.AddParameter("type", "County");

                httpResponseMessage = await httpClient.GetAsync(urlBuilder.ToString());
                if (httpResponseMessage.IsSuccessStatusCode)
                {
                    json = await httpResponseMessage.Content.ReadAsStringAsync();

                    string? code = string.IsNullOrWhiteSpace(json) ? null : Core.Convert.ToDiGi<GIS.Classes.AdministrativeAreal2D>(json)?.FirstOrDefault()?.Code;
                    if (!string.IsNullOrWhiteSpace(code))
                    {
                        urlBuilder = new("https://api.digiproject.uk/gis/administrativeareal2D/idbycode");
                        urlBuilder = urlBuilder.AddParameter("code", code);
                        urlBuilder = urlBuilder.AddParameter("administrativearealtype", "County");

                        httpResponseMessage = await httpClient.GetAsync(urlBuilder.ToString());
                        if (httpResponseMessage.IsSuccessStatusCode)
                        {
                            json = await httpResponseMessage.Content.ReadAsStringAsync();
                            if (int.TryParse(json, out int countyId_Temp))
                            {
                                countyId = countyId_Temp;
                            }
                        }
                    }
                }
            }

            #endregion CountyId by point

            #region Building2DReference

            urlBuilder = new("https://api.digiproject.uk/gis/building2D/building2Dreferencebyreference");
            urlBuilder = urlBuilder.AddParameter("reference", reference);
            if (countyId is not null && countyId.HasValue)
            {
                urlBuilder = urlBuilder.AddParameter("countyId", countyId.Value);
            }

            httpResponseMessage = await httpClient.GetAsync(urlBuilder.ToString());
            if (!httpResponseMessage.IsSuccessStatusCode)
            {
                return NotFound();
            }

            json = await httpResponseMessage.Content.ReadAsStringAsync();
            if (string.IsNullOrWhiteSpace(json))
            {
                return NotFound();
            }

            Building2DReference? building2DReference = Core.Convert.ToDiGi<Building2DReference>(json)?.FirstOrDefault();
            if (building2DReference is null)
            {
                return NotFound();
            }

            #endregion Building2DReference

            return View("Building2DDetailsView", new Building2DReferencesViewModel([building2DReference]));
        }

        /// <summary>
        /// Asynchronously retrieves a building 2D item by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the item to retrieve.</param>
        /// <param name="countyId">The optional unique identifier of the county associated with the item.</param>
        /// <returns>A task that represents the asynchronous operation, containing the <see cref="Microsoft.AspNetCore.Mvc.IActionResult"/> result.</returns>
        [HttpGet("itembyid")]
        public async Task<IActionResult> GetItemByIdAsync([FromQuery(Name = "id")] long id, [FromQuery(Name = "countyid")] int? countyId)
        {
            HttpClient httpClient = httpClientFactory.CreateClient();

            UrlBuilder urlBuilder;
            HttpResponseMessage httpResponseMessage;
            string json;

            #region Building2DReference

            urlBuilder = new("https://api.digiproject.uk/gis/building2D/building2Dreferencebyid");
            urlBuilder = urlBuilder.AddParameter("id", id);
            if (countyId is not null && countyId.HasValue)
            {
                urlBuilder = urlBuilder.AddParameter("countyId", countyId.Value);
            }

            httpResponseMessage = await httpClient.GetAsync(urlBuilder.ToString());
            if (!httpResponseMessage.IsSuccessStatusCode)
            {
                return BadRequest();
            }

            json = await httpResponseMessage.Content.ReadAsStringAsync();
            if (string.IsNullOrWhiteSpace(json))
            {
                return NoContent();
            }

            Building2DReference? building2DReference = Core.Convert.ToDiGi<Building2DReference>(json)?.FirstOrDefault();
            if (building2DReference is null)
            {
                return NoContent();
            }

            #endregion Building2DReference

            #region Building2D

            urlBuilder = new("https://api.digiproject.uk/gis/building2D/itembyid");
            urlBuilder = urlBuilder.AddParameter("id", id);
            if (countyId is not null && countyId.HasValue)
            {
                urlBuilder = urlBuilder.AddParameter("countyId", countyId.Value);
            }

            httpResponseMessage = await httpClient.GetAsync(urlBuilder.ToString());
            if (!httpResponseMessage.IsSuccessStatusCode)
            {
                return BadRequest();
            }

            json = await httpResponseMessage.Content.ReadAsStringAsync();
            if (string.IsNullOrWhiteSpace(json))
            {
                return NoContent();
            }

            GIS.Classes.Building2D? building2D = Core.Convert.ToDiGi<GIS.Classes.Building2D>(json)?.FirstOrDefault();
            if (building2D is null)
            {
                return NoContent();
            }

            #endregion Building2D

            #region AdministrativeAreal2DReferencePath

            int? administrativeAreal2DId = building2DReference?.SubdivisionId;

            if (administrativeAreal2DId is null || !administrativeAreal2DId.HasValue)
            {
                administrativeAreal2DId = building2DReference?.CountyId;
            }

            AdministrativeAreal2DReferencePath? administrativeAreal2DReferencePath = null;

            if (administrativeAreal2DId is not null && administrativeAreal2DId.HasValue)
            {
                urlBuilder = new("https://api.digiproject.uk/gis/administrativeareal2D/administrativeareal2Dreferencepathbyid");
                urlBuilder = urlBuilder.AddParameter("id", administrativeAreal2DId.Value);

                httpResponseMessage = await httpClient.GetAsync(urlBuilder.ToString());
                if (httpResponseMessage.IsSuccessStatusCode)
                {
                    json = await httpResponseMessage.Content.ReadAsStringAsync();
                    if (!string.IsNullOrWhiteSpace(json))
                    {
                        administrativeAreal2DReferencePath = Core.Convert.ToDiGi<AdministrativeAreal2DReferencePath>(json)?.FirstOrDefault();
                    }
                }
            }

            #endregion AdministrativeAreal2DReferencePath

            Building2DViewModel building2DViewModel = new(building2DReference, building2D, administrativeAreal2DReferencePath);

            // We pass the objects to a Partial View
            return PartialView("_Building2DView", building2DViewModel);
        }

        /// <summary>
        /// Retrieves a polygon by its unique identifier asynchronously.
        /// </summary>
        /// <param name="id">The unique identifier of the polygon.</param>
        /// <param name="countyId">The optional county identifier used to filter the request.</param>
        /// <param name="reductionFactor">The optional reduction factor for simplifying the polygon geometry.</param>
        /// <param name="minCount">The optional minimum count threshold for the data retrieval.</param>
        /// <returns>A <see cref="Task{IActionResult}"/> representing the asynchronous operation result containing the requested polygon data.</returns>
        [HttpGet("svg/polygonbyid")]
        public async Task<IActionResult> GetPolygonByIdAsync([FromQuery(Name = "id")] long id, [FromQuery(Name = "countyid")] int? countyId, [FromQuery(Name = "reductionfactor")] double? reductionFactor = null, [FromQuery(Name = "mincount")] int? minCount = null)
        {
            HttpClient httpClient = httpClientFactory.CreateClient();

            UrlBuilder urlBuilder;
            HttpResponseMessage httpResponseMessage;
            string json;

            #region Building2D

            urlBuilder = new("https://api.digiproject.uk/gis/building2D/itembyid");
            urlBuilder = urlBuilder.AddParameter("id", id);
            if (countyId is not null && countyId.HasValue)
            {
                urlBuilder = urlBuilder.AddParameter("countyId", countyId.Value);
            }

            httpResponseMessage = await httpClient.GetAsync(urlBuilder.ToString());
            if (!httpResponseMessage.IsSuccessStatusCode)
            {
                return BadRequest();
            }

            json = await httpResponseMessage.Content.ReadAsStringAsync();
            if (string.IsNullOrWhiteSpace(json))
            {
                return NoContent();
            }

            GIS.Classes.Building2D? building2D = Core.Convert.ToDiGi<GIS.Classes.Building2D>(json)?.FirstOrDefault();
            if (building2D is null)
            {
                return NotFound();
            }

            #endregion Building2D

            #region Point2Ds

            List<Point2D>? point2Ds = building2D.PolygonalFace2D?.ExternalEdge?.GetPoints();
            Modify.Reduce(point2Ds, reductionFactor, minCount ?? 100);

            #endregion Point2Ds

            string result = point2Ds is null ? string.Empty : string.Join(" ", point2Ds.ConvertAll(p => $"{p.X} {p.Y}"));

            return Content(result, "text/plain");
        }

        /// <summary>
        /// Asynchronously retrieves 2D points based on a collection of references and an optional county identifier.
        /// </summary>
        /// <param name="references">The collection of string references used to identify the building points.</param>
        /// <param name="countyId">The optional integer identifier of the county associated with the buildings.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains an <see cref="IActionResult"/>.</returns>
        [HttpGet("svg/pointsbyreferences")]
        public async Task<IActionResult> GetPointsByReferencesAsync([FromBody] IEnumerable<string> references, [FromQuery(Name = "countyid")] int? countyId)
        {
            HttpClient httpClient = httpClientFactory.CreateClient();

            UrlBuilder urlBuilder;
            HttpResponseMessage httpResponseMessage;
            string json;

            #region Point2Ds

            urlBuilder = new("https://api.digiproject.uk/gis/building2D/point2dsbyreferences");
            if (countyId is not null && countyId.HasValue)
            {
                urlBuilder = urlBuilder.AddParameter("countyId", countyId.Value);
            }

            HttpRequestMessage httpRequestMessage = new(HttpMethod.Get, urlBuilder.ToString())
            {
                Content = JsonContent.Create(references)
            };

            httpResponseMessage = await httpClient.SendAsync(httpRequestMessage);
            if (!httpResponseMessage.IsSuccessStatusCode)
            {
                return BadRequest();
            }

            json = await httpResponseMessage.Content.ReadAsStringAsync();
            if (string.IsNullOrWhiteSpace(json))
            {
                return NoContent();
            }

            List<Point2D>? point2Ds = Core.Convert.ToDiGi<Point2D>(json);
            if (point2Ds is null)
            {
                return NotFound();
            }

            #endregion Point2Ds

            string result = point2Ds is null ? string.Empty : string.Join(" ", point2Ds.ConvertAll(p => $"{p.X} {p.Y}"));

            return Content(result, "text/plain");
        }

        /// <summary>
        /// Renders the 3D viewer page for all buildings within the specified circular area. The page itself carries no geometry; the viewer streams the binary glTF payload from the glb endpoint.
        /// <para>The search is purely spatial: the area may span multiple counties, so no county identifier is required.</para>
        /// </summary>
        /// <param name="centerX">The X coordinate of the center of the search circle.</param>
        /// <param name="centerY">The Y coordinate of the center of the search circle.</param>
        /// <param name="radius">The radius of the search circle in meters.</param>
        /// <param name="storeyHeight">The optional storey height in meters used for the extrusions.</param>
        /// <returns>An <see cref="IActionResult"/> rendering the glTF scene view.</returns>
        [HttpGet("/gltf/buildingsbyradius")]
        public IActionResult GetBuildingsByRadius([FromQuery(Name = "centerX")] double centerX, [FromQuery(Name = "centerY")] double centerY, [FromQuery(Name = "radius")] double radius, [FromQuery(Name = "storeyheight")] double? storeyHeight = null)
        {
            if (double.IsNaN(centerX) || double.IsNaN(centerY) || double.IsNaN(radius) || radius <= 0)
            {
                return BadRequest();
            }

            string gLBUrl = $"~/gltf/glb/buildingsbyradius?centerX={centerX.ToString(System.Globalization.CultureInfo.InvariantCulture)}&centerY={centerY.ToString(System.Globalization.CultureInfo.InvariantCulture)}&radius={radius.ToString(System.Globalization.CultureInfo.InvariantCulture)}";
            if (storeyHeight is not null && storeyHeight.HasValue)
            {
                gLBUrl += $"&storeyheight={storeyHeight.Value.ToString(System.Globalization.CultureInfo.InvariantCulture)}";
            }

            string title = $"Buildings ({centerX}, {centerY}) r = {radius} m";

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
        [HttpGet("/gltf/glb/buildingsbyradius")]
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

        // This action will trigger for: gis.digiproject.uk/administrativeareal2D
        /// <summary>
        /// Initializes and returns the start view for the 2D building interface.
        /// </summary>
        /// <returns>An <see cref="Microsoft.AspNetCore.Mvc.IActionResult"/> result that renders the starting view.</returns>
        [HttpGet("")]
        public IActionResult Start()
        {
            return View();
        }
    }
}