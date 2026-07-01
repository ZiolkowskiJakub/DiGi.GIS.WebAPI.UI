using DiGi.Geometry.Planar.Classes;
using DiGi.GIS.Classes;
using DiGi.GIS.PostgreSQL;
using DiGi.GIS.PostgreSQL.Enums;
using DiGi.WebAPI.Classes;
using Microsoft.AspNetCore.Mvc;
using DiGi.GIS.WebAPI.UI.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace DiGi.GIS.WebAPI.UI.Controllers
{
    /// <summary>
    /// Provides API endpoints for managing and retrieving 2D administrative areal data.
    /// </summary>
    [Route("[controller]")]
    public class AdministrativeAreal2DController : Controller
    {
        private readonly IHttpClientFactory httpClientFactory;

        // Constructor injection for the PostgreSQL data source
        /// <summary>
        /// Initializes a new instance of the <see cref="AdministrativeAreal2DController"/> class.
        /// </summary>
        /// <param name="httpClientFactory">The <see cref="IHttpClientFactory"/> used to create HTTP clients.</param>
        public AdministrativeAreal2DController(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory;
        }

        /// <summary>
        /// Searches for administrative area reference paths by name.
        /// </summary>
        /// <param name="text">The text to search for within the name column.</param>
        /// <returns>A <see cref="Task{IActionResult}"/> representing the asynchronous operation, containing the result of the search.</returns>
        [HttpPost("administrativeareal2Dreferencepathsbyname")]
        public async Task<IActionResult> GetAdministrativeAreal2DReferencePathsByNameAsync([FromBody] string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return Ok();
            }

            HttpClient httpClient = httpClientFactory.CreateClient();

            try
            {
                string url = "https://api.digiproject.uk/gis/administrativeareal2d/administrativeareal2Dreferencepathsbyname";

                HttpResponseMessage httpResponseMessage = await httpClient.PostAsJsonAsync(url, text);

                if (httpResponseMessage.IsSuccessStatusCode)
                {
                    string json = await httpResponseMessage.Content.ReadAsStringAsync();

                    if (string.IsNullOrEmpty(json))
                    {
                        return NoContent();
                    }

                    return Content(json, "application/json");
                }

                return StatusCode((int)httpResponseMessage.StatusCode, "External API returned an error.");
            }
            catch (Exception exception)
            {
                // Log details using your logging framework
                return StatusCode(500, $"Internal server error: {exception.Message}");
            }
        }

        /// <summary>
        /// Retrieves administrative 2D area references based on the specified administrative areal type, parent identifier, and unique code filter.
        /// </summary>
        /// <param name="administrativeArealType">The type of the administrative area.</param>
        /// <param name="parentId">The optional identifier of the parent administrative area.</param>
        /// <param name="uniqueCode">An optional flag indicating whether to filter by unique code.</param>
        /// <returns>A <see cref="Task{IActionResult}"/> representing the asynchronous operation result.</returns>
        [HttpGet("administrativeareal2Dreferencesbyadministrativearealtype")]
        public async Task<IActionResult> GetAdministrativeAreal2DReferencesByAdministrativeArealTypeAsync([FromQuery(Name = "administrativearealtype")] string administrativeArealType, [FromQuery(Name = "parentid")] int? parentId, [FromQuery(Name = "uniquecode")] bool? uniqueCode)
        {
            HttpClient httpClient = httpClientFactory.CreateClient();

            UrlBuilder urlBuilder = new("https://api.digiproject.uk/gis/administrativeareal2D/administrativeareal2Dreferencesbyadministrativearealtype");
            if (!string.IsNullOrWhiteSpace(administrativeArealType))
            {
                urlBuilder = urlBuilder.AddParameter("administrativearealtype", administrativeArealType);
            }

            if (parentId is not null && parentId.HasValue)
            {
                urlBuilder = urlBuilder.AddParameter("parentId", parentId.Value);
            }

            if (uniqueCode is not null && uniqueCode.HasValue)
            {
                urlBuilder = urlBuilder.AddParameter("uniquecode", uniqueCode.Value);
            }

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
            List<PostgreSQL.Classes.AdministrativeAreal2DReference>? administrativeAreal2DReferences = Core.Convert.ToDiGi<PostgreSQL.Classes.AdministrativeAreal2DReference>(json);

            // We pass the objects to a Partial View
            return PartialView("_AdministrativeAreal2DReferences", administrativeAreal2DReferences ?? []);
        }

        /// <summary>
        /// Retrieves administrative areal 2D references by the specified code asynchronously.
        /// </summary>
        /// <param name="code">The code used to retrieve the administrative areal 2D references.</param>
        /// <returns>A <see cref="Task{IActionResult}"/> representing the asynchronous operation result.</returns>
        [HttpGet("administrativeareal2Dreferencesbycode")]
        public async Task<IActionResult> GetAdministrativeAreal2DReferencesByCodeAsync([FromQuery(Name = "code")] string code)
        {
            HttpClient httpClient = httpClientFactory.CreateClient();

            UrlBuilder urlBuilder;
            HttpResponseMessage httpResponseMessage;
            string json;

            #region AdministrativeAreal2DReference

            urlBuilder = new("https://api.digiproject.uk/gis/administrativeareal2D/administrativeareal2Dreferencebycode");
            if (code is not null)
            {
                urlBuilder = urlBuilder.AddParameter("code", code);
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

            PostgreSQL.Classes.AdministrativeAreal2DReference? administrativeAreal2DReference = Core.Convert.ToDiGi<PostgreSQL.Classes.AdministrativeAreal2DReference>(json)?.FirstOrDefault();
            if (administrativeAreal2DReference is null)
            {
                return NotFound();
            }

            if (administrativeAreal2DReference.AdministrativeArealType == PostgreSQL.Enums.AdministrativeArealType.Subdivison && administrativeAreal2DReference.AdministrativeArealType.ParentAdministrativeArealType() is PostgreSQL.Enums.AdministrativeArealType administrativeArealType_Parent)
            {
                urlBuilder = urlBuilder.AddParameter("administrativearealtype", administrativeArealType_Parent.ToString());

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

                administrativeAreal2DReference = Core.Convert.ToDiGi<PostgreSQL.Classes.AdministrativeAreal2DReference>(json)?.FirstOrDefault();
                if (administrativeAreal2DReference is null)
                {
                    return NotFound();
                }
            }

            #endregion AdministrativeAreal2DReference

            #region AdministrativeAreal2D

            urlBuilder = new("https://api.digiproject.uk/gis/administrativeareal2D/itembyid");
            if (code is not null)
            {
                urlBuilder = urlBuilder.AddParameter("id", administrativeAreal2DReference.Id);
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

            AdministrativeAreal2D? administrativeAreal2D = Core.Convert.ToDiGi<AdministrativeAreal2D>(json)?.FirstOrDefault();
            if (administrativeAreal2D is null)
            {
                return NotFound();
            }

            #endregion AdministrativeAreal2D

            #region AdministrativeAreal2DReferencePath

            urlBuilder = new("https://api.digiproject.uk/gis/administrativeareal2D/administrativeareal2Dreferencepathbyid");
            if (code is not null)
            {
                urlBuilder = urlBuilder.AddParameter("id", administrativeAreal2DReference.Id);
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

            PostgreSQL.Classes.AdministrativeAreal2DReferencePath? administrativeAreal2DReferencePath = Core.Convert.ToDiGi<PostgreSQL.Classes.AdministrativeAreal2DReferencePath>(json)?.FirstOrDefault();
            if (administrativeAreal2DReferencePath is null)
            {
                return NotFound();
            }

            administrativeAreal2DReferencePath.Remove(PostgreSQL.Enums.AdministrativeArealType.Subdivison);

            #endregion AdministrativeAreal2DReferencePath

            #region AdministrativeAreal2DReferences

            urlBuilder = new("https://api.digiproject.uk/gis/administrativeareal2D/administrativeareal2Dreferencesbycode");
            if (code is not null)
            {
                urlBuilder = urlBuilder.AddParameter("code", code);
            }

            PostgreSQL.Enums.AdministrativeArealType? administrativeArealType_Child = administrativeAreal2DReference.AdministrativeArealType.ChildAdministrativeArealType();
            if (administrativeArealType_Child is not null && administrativeArealType_Child.HasValue)
            {
                urlBuilder = urlBuilder.AddParameter("administrativearealtype", administrativeArealType_Child.Value.ToString());
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

            List<PostgreSQL.Classes.AdministrativeAreal2DReference>? administrativeAreal2DReferences = Core.Convert.ToDiGi<PostgreSQL.Classes.AdministrativeAreal2DReference>(json);
            if (administrativeAreal2DReferences is null)
            {
                return NotFound();
            }

            #endregion AdministrativeAreal2DReferences

            AdministrativeAreal2DView administrativeAreal2DView = new(administrativeAreal2DReference, administrativeAreal2D, administrativeAreal2DReferencePath, administrativeAreal2DReferences);

            return View("AdministrativeAreal2DView", administrativeAreal2DView);
        }

        /// <summary>
        /// Retrieves the administrative areal 2D references by their identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the administrative areal 2D reference.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains an <see cref="Microsoft.AspNetCore.Mvc.IActionResult"/>.</returns>
        [HttpGet("administrativeareal2Dreferencesbyid")]
        public async Task<IActionResult> GetAdministrativeAreal2DReferencesByIdAsync([FromQuery(Name = "id")] int id)
        {
            HttpClient httpClient = httpClientFactory.CreateClient();

            UrlBuilder urlBuilder;
            HttpResponseMessage httpResponseMessage;
            string json;

            #region AdministrativeAreal2DReferencePath

            urlBuilder = new("https://api.digiproject.uk/gis/administrativeareal2D/administrativeareal2Dreferencepathbyid");
            urlBuilder = urlBuilder.AddParameter("id", id);

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

            PostgreSQL.Classes.AdministrativeAreal2DReferencePath? administrativeAreal2DReferencePath = Core.Convert.ToDiGi<PostgreSQL.Classes.AdministrativeAreal2DReferencePath>(json)?.FirstOrDefault();
            if (administrativeAreal2DReferencePath is null)
            {
                return NotFound();
            }

            #endregion AdministrativeAreal2DReferencePath

            #region AdministrativeAreal2DReference

            PostgreSQL.Classes.AdministrativeAreal2DReference? administrativeAreal2DReference = administrativeAreal2DReferencePath.AdministrativeAreal2DReferences?.Last();
            if (administrativeAreal2DReference is null)
            {
                return NotFound();
            }

            #endregion AdministrativeAreal2DReference

            #region AdministrativeAreal2D

            urlBuilder = new("https://api.digiproject.uk/gis/administrativeareal2D/itembyid");
            urlBuilder = urlBuilder.AddParameter("id", id);

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

            AdministrativeAreal2D? administrativeAreal2D = Core.Convert.ToDiGi<AdministrativeAreal2D>(json)?.FirstOrDefault();
            if (administrativeAreal2D is null)
            {
                return NotFound();
            }

            #endregion AdministrativeAreal2D

            #region AdministrativeAreal2DReferences

            List<PostgreSQL.Classes.AdministrativeAreal2DReference>? administrativeAreal2DReferences = null;

            if (administrativeAreal2DReference.Code is string code && !string.IsNullOrWhiteSpace(code) && administrativeAreal2DReference?.AdministrativeArealType is PostgreSQL.Enums.AdministrativeArealType administrativeArealType && administrativeArealType.ChildAdministrativeArealType() is PostgreSQL.Enums.AdministrativeArealType administrativeArealType_Child)
            {
                urlBuilder = new("https://api.digiproject.uk/gis/administrativeareal2D/administrativeareal2Dreferencesbycode");
                urlBuilder = urlBuilder.AddParameter("code", code);
                urlBuilder = urlBuilder.AddParameter("administrativearealtype", administrativeArealType_Child.ToString());

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

                administrativeAreal2DReferences = Core.Convert.ToDiGi<PostgreSQL.Classes.AdministrativeAreal2DReference>(json);
            }

            #endregion AdministrativeAreal2DReferences

            AdministrativeAreal2DView administrativeAreal2DView = new(administrativeAreal2DReference, administrativeAreal2D, administrativeAreal2DReferencePath, administrativeAreal2DReferences);

            return View("AdministrativeAreal2DView", administrativeAreal2DView);
        }

        /// <summary>
        /// Retrieves an administrative areal 2D item by its specified code.
        /// </summary>
        /// <param name="code">The code of the item to retrieve.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains an <see cref="Microsoft.AspNetCore.Mvc.IActionResult"/>.</returns>
        [HttpGet("itembycode")]
        public async Task<IActionResult> GetItemByCodeAsync([FromQuery(Name = "code")] string code)
        {
            HttpClient httpClient = httpClientFactory.CreateClient();
            string url = $"https://api.digiproject.uk/gis/administrativeareal2D/itembycode?code={code}";

            HttpResponseMessage httpResponseMessage = await httpClient.GetAsync(url);
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
            AdministrativeAreal2D? administrativeAreal2D = Core.Convert.ToDiGi<AdministrativeAreal2D>(json)?.FirstOrDefault();
            // We pass the object to a View
            return View("AdministrativeAreal2DView", administrativeAreal2D);
        }

        /// <summary>
        /// Retrieves items filtered by the specified administrative area type.
        /// </summary>
        /// <param name="administrativeArealType">The administrative area type (e.g., country, voivodeship, county, municipality) to filter by.</param>
        /// <returns>A <see cref="Task{IActionResult}"/> representing the asynchronous operation result containing the items or an error response.</returns>
        [HttpGet("itemsbyadministrativearealtype")]
        public async Task<IActionResult> GetItemsByAdministrativeArealTypeAsync([FromQuery(Name = "administrativearealtype")] string administrativeArealType)
        {
            HttpClient httpClient = httpClientFactory.CreateClient();
            string url = $"https://api.digiproject.uk/gis/administrativeareal2D/itemsbyadministrativearealtype?administrativearealtype={administrativeArealType}";

            HttpResponseMessage httpResponseMessage = await httpClient.GetAsync(url);
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
            List<AdministrativeAreal2D>? administrativeAreal2Ds = Core.Convert.ToDiGi<AdministrativeAreal2D>(json);

            // We pass the objects to a Partial View
            return PartialView("_AdministrativeAreal2Ds", administrativeAreal2Ds ?? []);
        }

        /// <summary>
        /// Retrieves polygons associated with a specific administrative areal identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the administrative areal.</param>
        /// <param name="reductionFactor">The optional reduction factor used to simplify the geometry of the retrieved polygons.</param>
        /// <param name="minCount">The optional minimum count for filtering the results.</param>
        /// <returns>A <see cref="Microsoft.AspNetCore.Mvc.IActionResult"/> representing the result of the request.</returns>
        [HttpGet("svg/polygonsbyid")]
        public async Task<IActionResult> GetPolygonsByIdAsync([FromQuery(Name = "id")] int id, [FromQuery(Name = "reductionfactor")] double? reductionFactor = null, [FromQuery(Name = "mincount")] int? minCount = null)
        {
            HttpClient httpClient = httpClientFactory.CreateClient();

            UrlBuilder urlBuilder;
            HttpResponseMessage httpResponseMessage;
            string json;

            #region AdministrativeAreal2DReference

            urlBuilder = new("https://api.digiproject.uk/gis/administrativeareal2D/administrativeareal2Dreferencebyid");
            urlBuilder = urlBuilder.AddParameter("id", id);

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

            PostgreSQL.Classes.AdministrativeAreal2DReference? administrativeAreal2DReference = Core.Convert.ToDiGi<PostgreSQL.Classes.AdministrativeAreal2DReference>(json)?.FirstOrDefault();
            if (administrativeAreal2DReference is null)
            {
                return NotFound();
            }

            #endregion AdministrativeAreal2DReference

            AdministrativeArealType administrativeArealType = administrativeAreal2DReference.AdministrativeArealType;

            List<AdministrativeAreal2D> administrativeAreal2Ds = [];

            if (administrativeArealType == AdministrativeArealType.Subdivison || administrativeArealType == AdministrativeArealType.Municipality)
            {
                #region AdministrativeAreal2D

                urlBuilder = new("https://api.digiproject.uk/gis/administrativeareal2D/itembyid");
                urlBuilder = urlBuilder.AddParameter("id", id);

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

                AdministrativeAreal2D? administrativeAreal2D = Core.Convert.ToDiGi<AdministrativeAreal2D>(json)?.FirstOrDefault();
                if (administrativeAreal2D is null)
                {
                    return NotFound();
                }

                administrativeAreal2Ds.Add(administrativeAreal2D);

                #endregion AdministrativeAreal2D
            }
            else
            {
                #region AdministrativeAreal2Ds

                urlBuilder = new("https://api.digiproject.uk/gis/administrativeareal2D/itemsbycode");
                urlBuilder = urlBuilder.AddParameter("code", administrativeAreal2DReference.Code);
                urlBuilder = urlBuilder.AddParameter("administrativearealtype", administrativeArealType.ToString());

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

                if (Core.Convert.ToDiGi<AdministrativeAreal2D>(json) is not List<AdministrativeAreal2D> administrativeAreal2Ds_Temp || administrativeAreal2Ds_Temp.Count == 0)
                {
                    return NotFound();
                }

                administrativeAreal2Ds = administrativeAreal2Ds_Temp;

                #endregion AdministrativeAreal2Ds
            }

            #region Point2Ds

            // Prepare a list of polygons. Each polygon is a list of coordinates [x, y, x, y...]
            List<List<double>> result = [];

            if (reductionFactor is null)
            {
                switch (administrativeArealType)
                {
                    case AdministrativeArealType.Country:
                        reductionFactor = 0.00001;
                        break;

                    case AdministrativeArealType.Voivodeship:
                        reductionFactor = 0.001;
                        break;

                    case AdministrativeArealType.County:
                        reductionFactor = 0.001;
                        break;

                    default:
                        reductionFactor = 0.01;
                        break;
                }
            }

            if (minCount is null)
            {
                switch (administrativeArealType)
                {
                    case AdministrativeArealType.Country:
                        minCount = 30;
                        break;

                    case AdministrativeArealType.Voivodeship:
                        minCount = 50;
                        break;

                    default:
                        minCount = 100;
                        break;
                }
            }

            foreach (AdministrativeAreal2D? area in administrativeAreal2Ds)
            {
                List<Point2D>? point2Ds = area.PolygonalFace2D?.ExternalEdge?.GetPoints();
                if (point2Ds != null)
                {
                    Modify.Reduce(point2Ds, reductionFactor, minCount ?? 100);

                    List<double> coordinates = [];
                    foreach (Point2D? point2D in point2Ds)
                    {
                        coordinates.Add(point2D.X);
                        coordinates.Add(point2D.Y);
                    }
                    result.Add(coordinates);
                }
            }

            #endregion Point2Ds

            // Return as JSON: [[x,y,x,y], [x,y,x,y]]
            return Ok(result);
        }

        // This action will trigger for: gis.digiproject.uk/administrativeareal2D
        /// <summary>
        /// Starts the Administrative Areal 2D view.
        /// </summary>
        /// <returns>An <see cref="Microsoft.AspNetCore.Mvc.IActionResult"/> result that renders the start view.</returns>
        [HttpGet("")]
        public IActionResult Start()
        {
            return View();
        }
    }
}
