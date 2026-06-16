using DiGi.Geometry.Planar.Classes;
using DiGi.GIS.Classes;
using DiGi.GIS.PostgreSQL.Classes;
using DiGi.WebAPI.Classes;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace DiGi.GIS.WebAPI.UI.Classes
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

            // We pass the objects to a Partial View
            return PartialView("_Building2DReferencesView", new Building2DReferencesView(building2DReferences));
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

            Building2DView building2DView = new(building2DReference, building2D, administrativeAreal2DReferencePath);

            // We pass the objects to a Partial View
            return PartialView("_Building2DView", building2DView);
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