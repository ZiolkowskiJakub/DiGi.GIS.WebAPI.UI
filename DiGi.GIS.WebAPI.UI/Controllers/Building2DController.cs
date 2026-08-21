using DiGi.Geometry.Planar.Classes;
using DiGi.GIS.PostgreSQL.Classes;
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
    /// Provides the pages and partial views of the building feature.
    /// <para>The data itself is owned by the GIS Web API (gis/building2D); this controller only reads it and renders it.</para>
    /// </summary>
    [Route("[controller]")]
    public class Building2DController : Controller
    {
        private readonly IHttpClientFactory httpClientFactory;

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
        /// <param name="cancellationToken">A cancellation token that can be used by the caller to cancel the asynchronous operation.</param>
        /// <returns>A <see cref="Task{IActionResult}"/> representing the asynchronous operation.</returns>
        [HttpGet("building2Dreferencesbyadministrativeareal2Did")]
        public async Task<IActionResult> GetBuilding2DReferencesByAdministrativeAreal2DIdAsync([FromQuery(Name = "administrativeareal2Did")] int administrativeAreal2DId, CancellationToken cancellationToken = default)
        {
            if (administrativeAreal2DId <= 0)
            {
                return BadRequest();
            }

            HttpClient httpClient = httpClientFactory.CreateClient();

            UrlBuilder urlBuilder = new($"{Constants.Default.GISWebAPIUri}/gis/building2D/building2Dreferencesbyadministrativeareal2Did");
            urlBuilder = urlBuilder.AddParameter("administrativeareal2Did", administrativeAreal2DId);

            List<Building2DReference>? building2DReferences = await httpClient.ItemsAsync<Building2DReference>(urlBuilder.ToString(), cancellationToken);

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
        /// <param name="cancellationToken">A cancellation token that can be used by the caller to cancel the asynchronous operation.</param>
        /// <returns>A <see cref="Task{IActionResult}"/> rendering the building details page.</returns>
        [HttpGet("detailsbyreference")]
        public async Task<IActionResult> GetDetailsByReferenceAsync([FromQuery(Name = "reference")] string? reference, [FromQuery(Name = "countyid")] int? countyId = null, [FromQuery(Name = "x")] double? x = null, [FromQuery(Name = "y")] double? y = null, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(reference))
            {
                return BadRequest();
            }

            // A viewer node reference may be a ComplexReference that carries the building reference together with
            // its county (see PostgreSQL.Create.Reference). Unwrap it so the by-reference lookup and county
            // partition use the plain building reference; a plain reference is returned unchanged.
            if (PostgreSQL.Query.TryParse(reference, out string buildingModelReference, out int? countyId_Reference, out _))
            {
                if (!string.IsNullOrWhiteSpace(buildingModelReference))
                {
                    reference = buildingModelReference;
                }

                countyId ??= countyId_Reference;
            }

            HttpClient httpClient = httpClientFactory.CreateClient();

            countyId ??= await CountyIdAsync(httpClient, x, y, cancellationToken);

            Building2DReference? building2DReference = await httpClient.Building2DReferenceAsync(reference, countyId, cancellationToken);
            if (building2DReference is null)
            {
                return NotFound();
            }

            return View("Building2DDetailsView", new Building2DReferencesViewModel([building2DReference]));
        }

        /// <summary>
        /// Asynchronously retrieves a building 2D item by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the item to retrieve.</param>
        /// <param name="countyId">The optional unique identifier of the county associated with the item.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by the caller to cancel the asynchronous operation.</param>
        /// <returns>A task that represents the asynchronous operation, containing the <see cref="Microsoft.AspNetCore.Mvc.IActionResult"/> result.</returns>
        [HttpGet("itembyid")]
        public async Task<IActionResult> GetItemByIdAsync([FromQuery(Name = "id")] long id, [FromQuery(Name = "countyid")] int? countyId, CancellationToken cancellationToken = default)
        {
            HttpClient httpClient = httpClientFactory.CreateClient();

            UrlBuilder urlBuilder;

            #region Building2DReference

            urlBuilder = new($"{Constants.Default.GISWebAPIUri}/gis/building2D/building2Dreferencebyid");
            urlBuilder = urlBuilder.AddParameter("id", id);
            if (countyId.HasValue)
            {
                urlBuilder = urlBuilder.AddParameter("countyid", countyId.Value);
            }

            Building2DReference? building2DReference = await httpClient.ItemAsync<Building2DReference>(urlBuilder.ToString(), cancellationToken);
            if (building2DReference is null)
            {
                return NoContent();
            }

            #endregion Building2DReference

            #region Building2D

            urlBuilder = new($"{Constants.Default.GISWebAPIUri}/gis/building2D/itembyid");
            urlBuilder = urlBuilder.AddParameter("id", id);
            if (countyId.HasValue)
            {
                urlBuilder = urlBuilder.AddParameter("countyid", countyId.Value);
            }

            GIS.Classes.Building2D? building2D = await httpClient.ItemAsync<GIS.Classes.Building2D>(urlBuilder.ToString(), cancellationToken);
            if (building2D is null)
            {
                return NoContent();
            }

            #endregion Building2D

            #region AdministrativeAreal2DReferencePath

            // A building sits in a subdivision where the import resolved one, and in a county otherwise.
            int? administrativeAreal2DId = building2DReference.SubdivisionId ?? building2DReference.CountyId;

            AdministrativeAreal2DReferencePath? administrativeAreal2DReferencePath = null;

            if (administrativeAreal2DId.HasValue)
            {
                urlBuilder = new($"{Constants.Default.GISWebAPIUri}/gis/administrativeareal2D/administrativeareal2Dreferencepathbyid");
                urlBuilder = urlBuilder.AddParameter("id", administrativeAreal2DId.Value);

                administrativeAreal2DReferencePath = await httpClient.ItemAsync<AdministrativeAreal2DReferencePath>(urlBuilder.ToString(), cancellationToken);
            }

            #endregion AdministrativeAreal2DReferencePath

            Building2DViewModel building2DViewModel = new(building2DReference, building2D, administrativeAreal2DReferencePath);

            return PartialView("_Building2DView", building2DViewModel);
        }

        /// <summary>
        /// Retrieves the outline of a building footprint, reduced for drawing.
        /// </summary>
        /// <param name="id">The unique identifier of the building.</param>
        /// <param name="countyId">The optional county identifier used to filter the request.</param>
        /// <param name="reductionFactor">The optional reduction factor for simplifying the polygon geometry.</param>
        /// <param name="minCount">The optional fewest points the reduced outline may keep.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by the caller to cancel the asynchronous operation.</param>
        /// <returns>A <see cref="Task{IActionResult}"/> carrying the outline as a space separated coordinate list.</returns>
        [HttpGet("svg/polygonbyid")]
        public async Task<IActionResult> GetPolygonByIdAsync([FromQuery(Name = "id")] long id, [FromQuery(Name = "countyid")] int? countyId, [FromQuery(Name = "reductionfactor")] double? reductionFactor = null, [FromQuery(Name = "mincount")] int? minCount = null, CancellationToken cancellationToken = default)
        {
            HttpClient httpClient = httpClientFactory.CreateClient();

            UrlBuilder urlBuilder = new($"{Constants.Default.GISWebAPIUri}/gis/building2D/itembyid");
            urlBuilder = urlBuilder.AddParameter("id", id);
            if (countyId.HasValue)
            {
                urlBuilder = urlBuilder.AddParameter("countyid", countyId.Value);
            }

            GIS.Classes.Building2D? building2D = await httpClient.ItemAsync<GIS.Classes.Building2D>(urlBuilder.ToString(), cancellationToken);
            if (building2D is null)
            {
                return NoContent();
            }

            List<Point2D>? point2Ds = building2D.PolygonalFace2D?.ExternalEdge?.GetPoints();
            Modify.Reduce(point2Ds, reductionFactor, minCount ?? Constants.Default.PolygonMinimumPointCount);

            string result = point2Ds is null ? string.Empty : string.Join(" ", point2Ds.ConvertAll(point2D => $"{point2D.X} {point2D.Y}"));

            return Content(result, "text/plain");
        }

        // This action will trigger for: gis.digiproject.uk/building2D
        /// <summary>
        /// Initializes and returns the start view for the 2D building interface.
        /// </summary>
        /// <returns>An <see cref="Microsoft.AspNetCore.Mvc.IActionResult"/> result that renders the starting view.</returns>
        [HttpGet("")]
        public IActionResult Start()
        {
            return View();
        }

        /// <summary>
        /// Asynchronously resolves which county a plan position falls in.
        /// <para>Used only as a fallback: the 3D viewer knows a building by its reference and its centroid, and the reference alone does not say which county partition holds it.</para>
        /// </summary>
        /// <param name="httpClient">The HTTP client used for the requests.</param>
        /// <param name="x">The X coordinate, in PL-1992 (EPSG:2180) metres. This value can be null.</param>
        /// <param name="y">The Y coordinate, in PL-1992 (EPSG:2180) metres. This value can be null.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by the caller to cancel the asynchronous operation.</param>
        /// <returns>The identifier of the county, or <see langword="null"/> when it cannot be resolved.</returns>
        private static async Task<int?> CountyIdAsync(HttpClient httpClient, double? x, double? y, CancellationToken cancellationToken)
        {
            if (!x.HasValue || !y.HasValue || !double.IsFinite(x.Value) || !double.IsFinite(y.Value))
            {
                return null;
            }

            // The type filter is what makes this a county lookup rather than a request for every administrative
            // area covering the point - the country, the voivodeship and the municipality cover it too, and the
            // first of those would otherwise be read as the answer. The integer token is used rather than the
            // member name because it binds against every deployed build of the GIS Web API regardless of enum
            // renames.
            UrlBuilder urlBuilder = new($"{Constants.Default.GISWebAPIUri}/gis/administrativeareal2D/itemsbypoint");
            urlBuilder = urlBuilder.AddParameter("x", x.Value);
            urlBuilder = urlBuilder.AddParameter("y", y.Value);
            urlBuilder = urlBuilder.AddParameter("administrativearealtype", (int)AdministrativeArealType.County);

            GIS.Classes.AdministrativeAreal2D? administrativeAreal2D = await httpClient.ItemAsync<GIS.Classes.AdministrativeAreal2D>(urlBuilder.ToString(), cancellationToken);

            string? code = administrativeAreal2D?.Code;
            if (string.IsNullOrWhiteSpace(code))
            {
                return null;
            }

            // TODO [GISWebAPIRedeploy] Swap idbycode for idsbycode and try each part.
            // A county whose territory is disconnected is stored as one row per polygon part, and idbycode
            // collapses the code to the lowest of them - so for the 18 multi-part codes this can name a part the
            // building is not filed under. idsbycode returns every part and is the correct source, but it answers
            // 404 on the deployed GIS Web API (verified 2026-08-21). Remove this marker once
            // GET /information/controllers reports a DiGi.GIS.WebAPI build carrying idsbycode.
            urlBuilder = new($"{Constants.Default.GISWebAPIUri}/gis/administrativeareal2D/idbycode");
            urlBuilder = urlBuilder.AddParameter("code", code);
            urlBuilder = urlBuilder.AddParameter("administrativearealtype", (int)AdministrativeArealType.County);

            string? json = await httpClient.JsonAsync(urlBuilder.ToString(), cancellationToken);

            return int.TryParse(json, out int countyId) ? countyId : null;
        }
    }
}
