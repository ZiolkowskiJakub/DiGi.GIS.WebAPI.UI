using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using DiGi.GIS.PostgreSQL.Enums;
using DiGi.GIS.WebAPI.UI.ViewModels;
using DiGi.WebAPI.Classes;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Nodes;

namespace DiGi.GIS.WebAPI.UI.Controllers
{
    /// <summary>
    /// Provides the Typology definition feature: the page where a building typology is defined as a chain of building-data columns with rule types, ranges and colors.
    /// <para>The column data and the solving are owned by the GIS Web API (ZiolkowskiJakub/DiGi.Gis#5); this controller only reads and renders, so the query and rule semantics stay owned by that service and cannot drift here.</para>
    /// </summary>
    [Route("[controller]")]
    public class TypologyController : Controller
    {
        private readonly IHttpClientFactory httpClientFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="TypologyController"/> class.
        /// </summary>
        /// <param name="httpClientFactory">The <see cref="IHttpClientFactory"/> the feature's data actions use to create <see cref="HttpClient"/> instances.</param>
        public TypologyController(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory;
        }

        // This action will trigger for: gis.digiproject.uk/typology
        /// <summary>
        /// Starts the Typology definition page.
        /// </summary>
        /// <returns>An <see cref="IActionResult"/> result that renders the start view.</returns>
        [HttpGet("")]
        public IActionResult Start()
        {
            return View();
        }

        // Named AreaView rather than View: Controller.View() is already taken, and the action is the
        // Load modal's redirect target, not a generic view resolver.
        /// <summary>
        /// Renders the stub navigation target the Typology Load modal redirects to: the colour-coded building typology of one administrative area is out of scope (#18), so the page shows only the area context carried on the query.
        /// <para>The type is bound nullable and rejected when absent: a non-nullable <see cref="AdministrativeArealType"/> binding keeps <c>Country</c> for an omitted parameter, because the <c>Undefined</c> sentinel is -1 and not 0 - see Coding - WebAPI Contracts, section 2.</para>
        /// </summary>
        /// <param name="id">The unique identifier of the selected administrative area.</param>
        /// <param name="code">The optional code of the selected administrative area.</param>
        /// <param name="administrativeArealType">The type of the selected administrative area, as the integer the Load modal carried.</param>
        /// <returns>An <see cref="IActionResult"/> result that renders the stub view, or a 400 Bad Request response when the identifier or the area type is missing.</returns>
        [HttpGet("view")]
        public IActionResult AreaView([FromQuery(Name = "id")] int id, [FromQuery(Name = "code")] string? code = null, [FromQuery(Name = "administrativearealtype")] AdministrativeArealType? administrativeArealType = null)
        {
            if (id <= 0 || administrativeArealType is null || administrativeArealType.Value == AdministrativeArealType.Undefined)
            {
                return BadRequest();
            }

            return View("View", new TypologyViewViewModel(id, code, administrativeArealType.Value));
        }

        /// <summary>
        /// Retrieves the building-data columns available for typology grouping, sorted alphabetically by name.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token that can be used by the caller to cancel the asynchronous operation.</param>
        /// <returns>A <see cref="Task{IActionResult}"/> containing the sorted column list, or a 204 No Content response when the upstream service answers nothing.</returns>
        [HttpGet("columns")]
        public async Task<IActionResult> GetColumnsAsync(CancellationToken cancellationToken = default)
        {
            HttpClient httpClient = httpClientFactory.CreateClient();

            List<DiGi.PostgreSQL.Table.Classes.Column>? columns = await httpClient.BuildingDataColumnsAsync(cancellationToken);
            if (columns is null)
            {
                return NoContent();
            }

            List<TypologyColumnViewModel> viewModels = columns
                .OrderBy(column => column.Name ?? "", StringComparer.OrdinalIgnoreCase)
                .Select(column => new TypologyColumnViewModel(column))
                .ToList();

            return Ok(viewModels);
        }

        /// <summary>
        /// Resolves the administrative area selected in the Load Area modal into the county part identifiers that scope a "Load values" request of the Column Properties section.
        /// <para>The upstream <c>gis/BuildingData/uniquevalues</c> filters by a single county part id, so the page loads a county at a time and unions the answers. A county code maps to one id per polygon part (18 codes have several - see <c>Coding - GIS Administrative Data.md</c>), so a county resolves through <c>idsbycode</c> rather than the single id the modal row carries. A municipality or subdivision resolves to the parts of its parent county (a TERYT municipality code carries the county code as its first four characters), so its values are a superset - the upstream endpoint cannot narrow below a county. A voivodeship resolves to every county whose code starts with the voivodeship code. A country resolves to an empty list, which the page reads as the whole table.</para>
        /// </summary>
        /// <param name="code">The administrative code of the selected area.</param>
        /// <param name="administrativeArealType">The type of the selected area, bound as nullable so an omitted value is refused rather than read as <see cref="AdministrativeArealType.Country"/>.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by the caller to cancel the asynchronous operation.</param>
        /// <returns>A <see cref="Task{IActionResult}"/> containing the JSON array of county part identifiers (empty for a country), a 204 No Content response when the upstream service answers nothing, or a 400 Bad Request response when the code is blank or the type is missing.</returns>
        [HttpGet("countyids")]
        public async Task<IActionResult> GetCountyIdsAsync([FromQuery(Name = "code")] string code, [FromQuery(Name = "administrativearealtype")] AdministrativeArealType? administrativeArealType, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(code) || administrativeArealType is null || administrativeArealType.Value == AdministrativeArealType.Undefined)
            {
                return BadRequest();
            }

            if (administrativeArealType.Value == AdministrativeArealType.Country)
            {
                return Ok(Array.Empty<int>());
            }

            HttpClient httpClient = httpClientFactory.CreateClient();

            if (administrativeArealType.Value == AdministrativeArealType.Voivodeship)
            {
                UrlBuilder urlBuilder_References = new($"{Constants.Default.GISWebAPIUri}/gis/administrativeareal2D/administrativeareal2Dreferencesbyadministrativearealtype");
                urlBuilder_References = urlBuilder_References.AddParameter("administrativearealtype", (int)AdministrativeArealType.County);

                List<PostgreSQL.Classes.AdministrativeAreal2DReference>? administrativeAreal2DReferences = await httpClient.ItemsAsync<PostgreSQL.Classes.AdministrativeAreal2DReference>(urlBuilder_References.ToString(), cancellationToken);
                if (administrativeAreal2DReferences is null)
                {
                    return NoContent();
                }

                List<int> ids = administrativeAreal2DReferences
                    .Where(administrativeAreal2DReference => administrativeAreal2DReference.Id >= 0 && administrativeAreal2DReference.Code is not null && administrativeAreal2DReference.Code.StartsWith(code, StringComparison.Ordinal))
                    .Select(administrativeAreal2DReference => administrativeAreal2DReference.Id)
                    .Distinct()
                    .OrderBy(id => id)
                    .ToList();

                return ids.Count == 0 ? NoContent() : Ok(ids);
            }

            // A county code is the first four characters of a municipality or subdivision code.
            string code_County = administrativeArealType.Value == AdministrativeArealType.County ? code : (code.Length >= 4 ? code.Substring(0, 4) : code);

            UrlBuilder urlBuilder = new($"{Constants.Default.GISWebAPIUri}/gis/administrativeareal2D/idsbycode");
            urlBuilder = urlBuilder.AddParameter("code", code_County);
            urlBuilder = urlBuilder.AddParameter("administrativearealtype", (int)AdministrativeArealType.County);

            string? json = await httpClient.JsonAsync(urlBuilder.ToString(), cancellationToken);
            if (string.IsNullOrWhiteSpace(json))
            {
                return NoContent();
            }

            return Content(json, "application/json");
        }

        /// <summary>
        /// Relays the distinct values of one building-data column, for unique-value coloring in the Column Properties section.
        /// <para>The upstream <c>gis/BuildingData/uniquevalues</c> answers 404 for an empty result and takes several seconds per county (tens of seconds nationwide), so every non-success collapses to 204 No Content and the page shows its empty state rather than an error.</para>
        /// </summary>
        /// <param name="columnUniqueId">The unique identifier (slug) of the column, as listed by <see cref="GetColumnsAsync"/>.</param>
        /// <param name="countyId">The optional county part identifier that scopes the distinct values; <c>null</c> asks for the whole table.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by the caller to cancel the asynchronous operation.</param>
        /// <returns>A <see cref="Task{IActionResult}"/> containing the upstream JSON array of primitive values, a 204 No Content response when the upstream service answers nothing, or a 400 Bad Request response when the column identifier is blank.</returns>
        [HttpGet("uniquevalues")]
        public async Task<IActionResult> GetUniqueValuesAsync([FromQuery(Name = "columnuniqueid")] string columnUniqueId, [FromQuery(Name = "countyid")] int? countyId = null, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(columnUniqueId))
            {
                return BadRequest();
            }

            HttpClient httpClient = httpClientFactory.CreateClient();

            // The GIS Web API binds these names in lowercase ([FromQuery(Name = "columnuniqueid")]); query binding
            // is case-insensitive either way, so the names match the other proxies in this repository.
            UrlBuilder urlBuilder = new($"{Constants.Default.GISWebAPIUri}/gis/BuildingData/uniquevalues");
            urlBuilder = urlBuilder.AddParameter("columnuniqueid", columnUniqueId);
            if (countyId.HasValue)
            {
                urlBuilder = urlBuilder.AddParameter("countyid", countyId.Value);
            }

            string? json = await httpClient.JsonAsync(urlBuilder.ToString(), cancellationToken);
            if (string.IsNullOrWhiteSpace(json))
            {
                return NoContent();
            }

            return Content(json, "application/json");
        }

        /// <summary>
        /// Turns the page state of the Typology definition page into its document: the DiGi JSON of a <see cref="DiGi.Typology.Visual.Classes.VisualColumnTypologyFilter"/> chain, which is what the page downloads on Export and what <see cref="ValidateDefinitionAsync"/> reads back on Import.
        /// <para>The state is resolved against the live column catalog and checked by <see cref="Query.TypologyDefinitionErrors(Classes.TypologyDefinitionParameter, IEnumerable{DiGi.PostgreSQL.Table.Classes.Column})"/>; the document is composed here so that the browser never spells a <c>_type</c> or an appearance key. Unlike the read actions above, a rejected state answers 400 with the error list as a JSON string array - the page shows it as it is - because the visitor can act on it; an unreachable catalog is not the visitor's fault and answers 503.</para>
        /// </summary>
        /// <param name="typologyDefinitionParameter">The page state, as <c>typology.js</c> holds it.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by the caller to cancel the asynchronous operation.</param>
        /// <returns>A <see cref="Task{IActionResult}"/> containing the document as JSON, a 400 Bad Request response carrying the error list, or a 503 Service Unavailable response when the column catalog cannot be read.</returns>
        [HttpPost("definition/export")]
        public async Task<IActionResult> ExportDefinitionAsync([FromBody] Classes.TypologyDefinitionParameter? typologyDefinitionParameter, CancellationToken cancellationToken = default)
        {
            if (typologyDefinitionParameter == null)
            {
                return BadRequest(new List<string>() { "The request carries no definition." });
            }

            HttpClient httpClient = httpClientFactory.CreateClient();

            List<DiGi.PostgreSQL.Table.Classes.Column>? columns = await httpClient.BuildingDataColumnsAsync(cancellationToken);
            if (columns is null)
            {
                return StatusCode(StatusCodes.Status503ServiceUnavailable);
            }

            List<string> errors = Query.TypologyDefinitionErrors(typologyDefinitionParameter, columns);
            if (errors.Count != 0)
            {
                return BadRequest(errors);
            }

            DiGi.Typology.Visual.Classes.VisualColumnTypologyFilter? visualColumnTypologyFilter = Create.VisualColumnTypologyFilter(typologyDefinitionParameter, columns);
            string? json = Core.Convert.ToSystem_String(visualColumnTypologyFilter);
            if (string.IsNullOrWhiteSpace(json))
            {
                return BadRequest(new List<string>() { "The definition could not be serialized." });
            }

            return Content(json, "application/json");
        }

        /// <summary>
        /// Reads a Typology definition document chosen for Import, validates it against the live column catalog and answers the page state the page replaces its own with.
        /// <para>The body is bound as a JSON object so that a file that is not JSON is refused before anything is read from it; a body whose <c>_type</c> names no known class deserializes to nothing and is refused the same way - never turned into an emptied level. Every check of <see cref="Query.TypologyDefinitionErrors(DiGi.Typology.Visual.Classes.VisualColumnTypologyFilter{Core.IO.Table.Classes.Column}, IEnumerable{DiGi.PostgreSQL.Table.Classes.Column})"/> answers 400 with the error list; only a document the page can show in full answers 200, so the page state is replaced after this action succeeds and never before.</para>
        /// </summary>
        /// <param name="jsonObject">The document, the JSON <see cref="ExportDefinitionAsync"/> produced.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by the caller to cancel the asynchronous operation.</param>
        /// <returns>A <see cref="Task{IActionResult}"/> containing the page state, a 400 Bad Request response carrying the error list, or a 503 Service Unavailable response when the column catalog cannot be read.</returns>
        [HttpPost("definition/validate")]
        public async Task<IActionResult> ValidateDefinitionAsync([FromBody] JsonObject? jsonObject, CancellationToken cancellationToken = default)
        {
            if (jsonObject == null)
            {
                return BadRequest(new List<string>() { "The file is not a JSON object." });
            }

            DiGi.Typology.Visual.Classes.VisualColumnTypologyFilter? visualColumnTypologyFilter = Core.Convert.ToDiGi<DiGi.Typology.Visual.Classes.VisualColumnTypologyFilter>(jsonObject.ToJsonString())?.FirstOrDefault();
            if (visualColumnTypologyFilter == null)
            {
                return BadRequest(new List<string>() { "The file is not a Typology definition: its _type is not a VisualColumnTypologyFilter." });
            }

            HttpClient httpClient = httpClientFactory.CreateClient();

            List<DiGi.PostgreSQL.Table.Classes.Column>? columns = await httpClient.BuildingDataColumnsAsync(cancellationToken);
            if (columns is null)
            {
                return StatusCode(StatusCodes.Status503ServiceUnavailable);
            }

            List<string> errors = Query.TypologyDefinitionErrors(visualColumnTypologyFilter, columns);
            if (errors.Count != 0)
            {
                return BadRequest(errors);
            }

            Classes.TypologyDefinitionParameter? typologyDefinitionParameter = Create.TypologyDefinitionParameter(visualColumnTypologyFilter, columns);
            if (typologyDefinitionParameter == null)
            {
                return BadRequest(new List<string>() { "The definition could not be read." });
            }

            return Ok(typologyDefinitionParameter);
        }
    }
}
