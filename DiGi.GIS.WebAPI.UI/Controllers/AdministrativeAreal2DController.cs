using DiGi.Geometry.Planar.Classes;
using DiGi.GIS.Classes;
using DiGi.GIS.PostgreSQL;
using DiGi.GIS.PostgreSQL.Enums;
using DiGi.GIS.WebAPI.UI.ViewModels;
using DiGi.WebAPI.Classes;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace DiGi.GIS.WebAPI.UI.Controllers
{
    /// <summary>
    /// Provides the pages and partial views of the administrative area feature.
    /// <para>The data itself is owned by the GIS Web API (gis/administrativeareal2D); this controller only reads it and renders it, so the query rules stay owned by that service and cannot drift here.</para>
    /// </summary>
    [Route("[controller]")]
    public class AdministrativeAreal2DController : Controller
    {
        private readonly IHttpClientFactory httpClientFactory;

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
        /// <param name="cancellationToken">A cancellation token that can be used by the caller to cancel the asynchronous operation.</param>
        /// <returns>A <see cref="Task{IActionResult}"/> representing the asynchronous operation, containing the result of the search.</returns>
        [HttpPost("administrativeareal2Dreferencepathsbyname")]
        public async Task<IActionResult> GetAdministrativeAreal2DReferencePathsByNameAsync([FromBody] string text, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return Ok();
            }

            HttpClient httpClient = httpClientFactory.CreateClient();

            // Relayed verbatim rather than round tripped through the reference path objects: deserializing
            // and reserializing would produce the very same bytes.
            string? json = await httpClient.PostJsonAsync($"{Constants.Default.GISWebAPIUri}/gis/administrativeareal2D/administrativeareal2Dreferencepathsbyname", text, cancellationToken);
            if (string.IsNullOrWhiteSpace(json))
            {
                return NoContent();
            }

            return Content(json, "application/json");
        }

        /// <summary>
        /// Retrieves administrative 2D area references based on the specified administrative areal type, parent identifier, and unique code filter.
        /// </summary>
        /// <param name="administrativeArealType">The type of the administrative area.</param>
        /// <param name="parentId">The optional identifier of the parent administrative area.</param>
        /// <param name="uniqueCode">An optional flag indicating whether to filter by unique code.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by the caller to cancel the asynchronous operation.</param>
        /// <returns>A <see cref="Task{IActionResult}"/> representing the asynchronous operation result.</returns>
        [HttpGet("administrativeareal2Dreferencesbyadministrativearealtype")]
        public async Task<IActionResult> GetAdministrativeAreal2DReferencesByAdministrativeArealTypeAsync([FromQuery(Name = "administrativearealtype")] AdministrativeArealType? administrativeArealType, [FromQuery(Name = "parentid")] int? parentId, [FromQuery(Name = "uniquecode")] bool? uniqueCode, CancellationToken cancellationToken = default)
        {
            if (administrativeArealType is null || administrativeArealType.Value == AdministrativeArealType.Undefined)
            {
                return BadRequest();
            }

            HttpClient httpClient = httpClientFactory.CreateClient();

            UrlBuilder urlBuilder = new($"{Constants.Default.GISWebAPIUri}/gis/administrativeareal2D/administrativeareal2Dreferencesbyadministrativearealtype");
            urlBuilder = urlBuilder.AddParameter("administrativearealtype", (int)administrativeArealType.Value);
            if (parentId.HasValue)
            {
                urlBuilder = urlBuilder.AddParameter("parentId", parentId.Value);
            }

            if (uniqueCode.HasValue)
            {
                urlBuilder = urlBuilder.AddParameter("uniquecode", uniqueCode.Value);
            }

            List<PostgreSQL.Classes.AdministrativeAreal2DReference>? administrativeAreal2DReferences = await httpClient.ItemsAsync<PostgreSQL.Classes.AdministrativeAreal2DReference>(urlBuilder.ToString(), cancellationToken);

            return PartialView("_AdministrativeAreal2DReferences", administrativeAreal2DReferences ?? []);
        }

        /// <summary>
        /// Retrieves administrative areal 2D references by the specified code asynchronously.
        /// </summary>
        /// <param name="code">The code used to retrieve the administrative areal 2D references.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by the caller to cancel the asynchronous operation.</param>
        /// <returns>A <see cref="Task{IActionResult}"/> representing the asynchronous operation result.</returns>
        [HttpGet("administrativeareal2Dreferencesbycode")]
        public async Task<IActionResult> GetAdministrativeAreal2DReferencesByCodeAsync([FromQuery(Name = "code")] string code, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(code))
            {
                return BadRequest();
            }

            HttpClient httpClient = httpClientFactory.CreateClient();

            UrlBuilder urlBuilder;

            #region AdministrativeAreal2DReference

            urlBuilder = new($"{Constants.Default.GISWebAPIUri}/gis/administrativeareal2D/administrativeareal2Dreferencebycode");
            urlBuilder = urlBuilder.AddParameter("code", code);

            PostgreSQL.Classes.AdministrativeAreal2DReference? administrativeAreal2DReference = await httpClient.ItemAsync<PostgreSQL.Classes.AdministrativeAreal2DReference>(urlBuilder.ToString(), cancellationToken);
            if (administrativeAreal2DReference is null)
            {
                return NotFound();
            }

            // A subdivision is shown as its parent municipality: the code is shared, and a subdivision has
            // no page of its own.
            if (administrativeAreal2DReference.AdministrativeArealType == AdministrativeArealType.Subdivision && administrativeAreal2DReference.AdministrativeArealType.ParentAdministrativeArealType() is AdministrativeArealType administrativeArealType_Parent)
            {
                urlBuilder = urlBuilder.AddParameter("administrativearealtype", (int)administrativeArealType_Parent);

                administrativeAreal2DReference = await httpClient.ItemAsync<PostgreSQL.Classes.AdministrativeAreal2DReference>(urlBuilder.ToString(), cancellationToken);
                if (administrativeAreal2DReference is null)
                {
                    return NotFound();
                }
            }

            #endregion AdministrativeAreal2DReference

            #region AdministrativeAreal2D

            urlBuilder = new($"{Constants.Default.GISWebAPIUri}/gis/administrativeareal2D/itembyid");
            urlBuilder = urlBuilder.AddParameter("id", administrativeAreal2DReference.Id);

            AdministrativeAreal2D? administrativeAreal2D = await httpClient.ItemAsync<AdministrativeAreal2D>(urlBuilder.ToString(), cancellationToken);
            if (administrativeAreal2D is null)
            {
                return NotFound();
            }

            #endregion AdministrativeAreal2D

            #region AdministrativeAreal2DReferencePath

            urlBuilder = new($"{Constants.Default.GISWebAPIUri}/gis/administrativeareal2D/administrativeareal2Dreferencepathbyid");
            urlBuilder = urlBuilder.AddParameter("id", administrativeAreal2DReference.Id);

            PostgreSQL.Classes.AdministrativeAreal2DReferencePath? administrativeAreal2DReferencePath = await httpClient.ItemAsync<PostgreSQL.Classes.AdministrativeAreal2DReferencePath>(urlBuilder.ToString(), cancellationToken);
            if (administrativeAreal2DReferencePath is null)
            {
                return NotFound();
            }

            administrativeAreal2DReferencePath.Remove(AdministrativeArealType.Subdivision);

            #endregion AdministrativeAreal2DReferencePath

            #region AdministrativeAreal2DReferences

            urlBuilder = new($"{Constants.Default.GISWebAPIUri}/gis/administrativeareal2D/administrativeareal2Dreferencesbycode");
            urlBuilder = urlBuilder.AddParameter("code", code);

            if (administrativeAreal2DReference.AdministrativeArealType.ChildAdministrativeArealType() is AdministrativeArealType administrativeArealType_Child)
            {
                urlBuilder = urlBuilder.AddParameter("administrativearealtype", (int)administrativeArealType_Child);
            }

            List<PostgreSQL.Classes.AdministrativeAreal2DReference>? administrativeAreal2DReferences = await httpClient.ItemsAsync<PostgreSQL.Classes.AdministrativeAreal2DReference>(urlBuilder.ToString(), cancellationToken);
            if (administrativeAreal2DReferences is null)
            {
                return NotFound();
            }

            #endregion AdministrativeAreal2DReferences

            AdministrativeAreal2DViewModel administrativeAreal2DViewModel = new(administrativeAreal2DReference, administrativeAreal2D, administrativeAreal2DReferencePath, administrativeAreal2DReferences);

            return View("AdministrativeAreal2DView", administrativeAreal2DViewModel);
        }

        /// <summary>
        /// Retrieves the administrative areal 2D references by their identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the administrative areal 2D reference.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by the caller to cancel the asynchronous operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains an <see cref="Microsoft.AspNetCore.Mvc.IActionResult"/>.</returns>
        [HttpGet("administrativeareal2Dreferencesbyid")]
        public async Task<IActionResult> GetAdministrativeAreal2DReferencesByIdAsync([FromQuery(Name = "id")] int id, CancellationToken cancellationToken = default)
        {
            if (id <= 0)
            {
                return BadRequest();
            }

            HttpClient httpClient = httpClientFactory.CreateClient();

            UrlBuilder urlBuilder;

            #region AdministrativeAreal2DReferencePath

            urlBuilder = new($"{Constants.Default.GISWebAPIUri}/gis/administrativeareal2D/administrativeareal2Dreferencepathbyid");
            urlBuilder = urlBuilder.AddParameter("id", id);

            PostgreSQL.Classes.AdministrativeAreal2DReferencePath? administrativeAreal2DReferencePath = await httpClient.ItemAsync<PostgreSQL.Classes.AdministrativeAreal2DReferencePath>(urlBuilder.ToString(), cancellationToken);
            if (administrativeAreal2DReferencePath is null)
            {
                return NotFound();
            }

            #endregion AdministrativeAreal2DReferencePath

            #region AdministrativeAreal2DReference

            // The path runs from the country down to the requested area, so the last entry is the area itself.
            // The property rebuilds its list on every call, so it is read once.
            List<PostgreSQL.Classes.AdministrativeAreal2DReference> administrativeAreal2DReferences_Path = administrativeAreal2DReferencePath.AdministrativeAreal2DReferences;
            if (administrativeAreal2DReferences_Path.Count == 0)
            {
                return NotFound();
            }

            PostgreSQL.Classes.AdministrativeAreal2DReference administrativeAreal2DReference = administrativeAreal2DReferences_Path[^1];

            #endregion AdministrativeAreal2DReference

            #region AdministrativeAreal2D

            urlBuilder = new($"{Constants.Default.GISWebAPIUri}/gis/administrativeareal2D/itembyid");
            urlBuilder = urlBuilder.AddParameter("id", id);

            AdministrativeAreal2D? administrativeAreal2D = await httpClient.ItemAsync<AdministrativeAreal2D>(urlBuilder.ToString(), cancellationToken);
            if (administrativeAreal2D is null)
            {
                return NotFound();
            }

            #endregion AdministrativeAreal2D

            #region AdministrativeAreal2DReferences

            List<PostgreSQL.Classes.AdministrativeAreal2DReference>? administrativeAreal2DReferences = null;

            if (!string.IsNullOrWhiteSpace(administrativeAreal2DReference.Code) && administrativeAreal2DReference.AdministrativeArealType.ChildAdministrativeArealType() is AdministrativeArealType administrativeArealType_Child)
            {
                urlBuilder = new($"{Constants.Default.GISWebAPIUri}/gis/administrativeareal2D/administrativeareal2Dreferencesbycode");
                urlBuilder = urlBuilder.AddParameter("code", administrativeAreal2DReference.Code);
                urlBuilder = urlBuilder.AddParameter("administrativearealtype", (int)administrativeArealType_Child);

                administrativeAreal2DReferences = await httpClient.ItemsAsync<PostgreSQL.Classes.AdministrativeAreal2DReference>(urlBuilder.ToString(), cancellationToken);
            }

            #endregion AdministrativeAreal2DReferences

            AdministrativeAreal2DViewModel administrativeAreal2DViewModel = new(administrativeAreal2DReference, administrativeAreal2D, administrativeAreal2DReferencePath, administrativeAreal2DReferences);

            return View("AdministrativeAreal2DView", administrativeAreal2DViewModel);
        }

        /// <summary>
        /// Retrieves an administrative areal 2D item by its specified code.
        /// </summary>
        /// <param name="code">The code of the item to retrieve.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by the caller to cancel the asynchronous operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains an <see cref="Microsoft.AspNetCore.Mvc.IActionResult"/>.</returns>
        [HttpGet("itembycode")]
        public async Task<IActionResult> GetItemByCodeAsync([FromQuery(Name = "code")] string code, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(code))
            {
                return BadRequest();
            }

            HttpClient httpClient = httpClientFactory.CreateClient();

            UrlBuilder urlBuilder = new($"{Constants.Default.GISWebAPIUri}/gis/administrativeareal2D/itembycode");
            urlBuilder = urlBuilder.AddParameter("code", code);

            AdministrativeAreal2D? administrativeAreal2D = await httpClient.ItemAsync<AdministrativeAreal2D>(urlBuilder.ToString(), cancellationToken);
            if (administrativeAreal2D is null)
            {
                return NoContent();
            }

            return View("AdministrativeAreal2DView", administrativeAreal2D);
        }

        /// <summary>
        /// Retrieves items filtered by the specified administrative area type.
        /// </summary>
        /// <param name="administrativeArealType">The administrative area type (e.g., country, voivodeship, county, municipality) to filter by.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by the caller to cancel the asynchronous operation.</param>
        /// <returns>A <see cref="Task{IActionResult}"/> representing the asynchronous operation result containing the items or an error response.</returns>
        [HttpGet("itemsbyadministrativearealtype")]
        public async Task<IActionResult> GetItemsByAdministrativeArealTypeAsync([FromQuery(Name = "administrativearealtype")] AdministrativeArealType? administrativeArealType, CancellationToken cancellationToken = default)
        {
            if (administrativeArealType is null || administrativeArealType.Value == AdministrativeArealType.Undefined)
            {
                return BadRequest();
            }

            HttpClient httpClient = httpClientFactory.CreateClient();

            UrlBuilder urlBuilder = new($"{Constants.Default.GISWebAPIUri}/gis/administrativeareal2D/itemsbyadministrativearealtype");
            urlBuilder = urlBuilder.AddParameter("administrativearealtype", (int)administrativeArealType.Value);

            List<AdministrativeAreal2D>? administrativeAreal2Ds = await httpClient.ItemsAsync<AdministrativeAreal2D>(urlBuilder.ToString(), cancellationToken);

            return PartialView("_AdministrativeAreal2Ds", administrativeAreal2Ds ?? []);
        }

        /// <summary>
        /// Retrieves the outlines of an administrative area, reduced for drawing as an overview map.
        /// <para>An area whose territory is disconnected is stored as one row per polygon part, so an area identified by a code is drawn from every row sharing that code. An area addressed directly (a municipality or a subdivision) is drawn from its own row alone.</para>
        /// </summary>
        /// <param name="id">The unique identifier of the administrative area.</param>
        /// <param name="reductionFactor">The optional reduction factor used to simplify the geometry of the retrieved polygons. When omitted, a factor matching the size of the area is applied.</param>
        /// <param name="minCount">The optional fewest points a reduced outline may keep. When omitted, a count matching the size of the area is applied.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by the caller to cancel the asynchronous operation.</param>
        /// <returns>A <see cref="Microsoft.AspNetCore.Mvc.IActionResult"/> carrying one flat coordinate list per outline.</returns>
        [HttpGet("svg/polygonsbyid")]
        public async Task<IActionResult> GetPolygonsByIdAsync([FromQuery(Name = "id")] int id, [FromQuery(Name = "reductionfactor")] double? reductionFactor = null, [FromQuery(Name = "mincount")] int? minCount = null, CancellationToken cancellationToken = default)
        {
            if (id <= 0)
            {
                return BadRequest();
            }

            HttpClient httpClient = httpClientFactory.CreateClient();

            UrlBuilder urlBuilder;

            #region AdministrativeAreal2DReference

            urlBuilder = new($"{Constants.Default.GISWebAPIUri}/gis/administrativeareal2D/administrativeareal2Dreferencebyid");
            urlBuilder = urlBuilder.AddParameter("id", id);

            PostgreSQL.Classes.AdministrativeAreal2DReference? administrativeAreal2DReference = await httpClient.ItemAsync<PostgreSQL.Classes.AdministrativeAreal2DReference>(urlBuilder.ToString(), cancellationToken);
            if (administrativeAreal2DReference is null)
            {
                return NotFound();
            }

            #endregion AdministrativeAreal2DReference

            AdministrativeArealType administrativeArealType = administrativeAreal2DReference.AdministrativeArealType;

            #region AdministrativeAreal2Ds

            List<AdministrativeAreal2D> administrativeAreal2Ds = [];

            if (administrativeArealType == AdministrativeArealType.Subdivision || administrativeArealType == AdministrativeArealType.Municipality)
            {
                urlBuilder = new($"{Constants.Default.GISWebAPIUri}/gis/administrativeareal2D/itembyid");
                urlBuilder = urlBuilder.AddParameter("id", id);

                AdministrativeAreal2D? administrativeAreal2D = await httpClient.ItemAsync<AdministrativeAreal2D>(urlBuilder.ToString(), cancellationToken);
                if (administrativeAreal2D is null)
                {
                    return NotFound();
                }

                administrativeAreal2Ds.Add(administrativeAreal2D);
            }
            else
            {
                urlBuilder = new($"{Constants.Default.GISWebAPIUri}/gis/administrativeareal2D/itemsbycode");
                urlBuilder = urlBuilder.AddParameter("code", administrativeAreal2DReference.Code);
                urlBuilder = urlBuilder.AddParameter("administrativearealtype", (int)administrativeArealType);

                List<AdministrativeAreal2D>? administrativeAreal2Ds_Temp = await httpClient.ItemsAsync<AdministrativeAreal2D>(urlBuilder.ToString(), cancellationToken);
                if (administrativeAreal2Ds_Temp is null || administrativeAreal2Ds_Temp.Count == 0)
                {
                    return NotFound();
                }

                administrativeAreal2Ds = administrativeAreal2Ds_Temp;
            }

            #endregion AdministrativeAreal2Ds

            #region Point2Ds

            double reductionFactor_Temp = reductionFactor ?? ReductionFactor(administrativeArealType);
            int minCount_Temp = minCount ?? MinimumPointCount(administrativeArealType);

            // One flat [x, y, x, y, ...] list per outline.
            List<List<double>> result = [];

            foreach (AdministrativeAreal2D administrativeAreal2D in administrativeAreal2Ds)
            {
                List<Point2D>? point2Ds = administrativeAreal2D.PolygonalFace2D?.ExternalEdge?.GetPoints();
                if (point2Ds is null)
                {
                    continue;
                }

                Modify.Reduce(point2Ds, reductionFactor_Temp, minCount_Temp);

                List<double> coordinates = [];
                foreach (Point2D point2D in point2Ds)
                {
                    coordinates.Add(point2D.X);
                    coordinates.Add(point2D.Y);
                }

                result.Add(coordinates);
            }

            #endregion Point2Ds

            return Ok(result);
        }

        // This action will trigger for: gis.digiproject.uk/administrativeareal2D
        /// <summary>
        /// Starts the Administrative Areal 2D view.
        /// </summary>
        /// <returns>An <see cref="IActionResult"/> result that renders the start view.</returns>
        [HttpGet("")]
        public IActionResult Start()
        {
            return View();
        }

        /// <summary>
        /// Gives the reduction factor an outline of the given kind of area is simplified with.
        /// </summary>
        /// <param name="administrativeArealType">The kind of administrative area.</param>
        /// <returns>The reduction factor.</returns>
        private static double ReductionFactor(AdministrativeArealType administrativeArealType)
        {
            return administrativeArealType switch
            {
                AdministrativeArealType.Country => Constants.Default.PolygonReductionFactor_Country,
                AdministrativeArealType.Voivodeship => Constants.Default.PolygonReductionFactor_Voivodeship,
                AdministrativeArealType.County => Constants.Default.PolygonReductionFactor_County,
                _ => Constants.Default.PolygonReductionFactor,
            };
        }

        /// <summary>
        /// Gives the fewest points an outline of the given kind of area is allowed to keep.
        /// </summary>
        /// <param name="administrativeArealType">The kind of administrative area.</param>
        /// <returns>The fewest points to keep.</returns>
        private static int MinimumPointCount(AdministrativeArealType administrativeArealType)
        {
            return administrativeArealType switch
            {
                AdministrativeArealType.Country => Constants.Default.PolygonMinimumPointCount_Country,
                AdministrativeArealType.Voivodeship => Constants.Default.PolygonMinimumPointCount_Voivodeship,
                _ => Constants.Default.PolygonMinimumPointCount,
            };
        }
    }
}
