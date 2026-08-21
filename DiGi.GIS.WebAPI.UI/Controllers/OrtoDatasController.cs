using DiGi.GIS.PostgreSQL.Classes;
using DiGi.GIS.WebAPI.UI.ViewModels;
using DiGi.WebAPI.Classes;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace DiGi.GIS.WebAPI.UI.Controllers
{
    /// <summary>
    /// Controller providing endpoints for managing and retrieving orthodata and coverage factor information.
    /// </summary>
    [Route("[controller]")]
    public class OrtoDatasController : Controller
    {
        private readonly IHttpClientFactory httpClientFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="OrtoDatasController"/> class.
        /// </summary>
        /// <param name="httpClientFactory">The HTTP client factory used to create <see cref="HttpClient"/> instances.</param>
        public OrtoDatasController(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory;
        }

        /// <summary>
        /// Retrieves orthodata and building 2D reference information based on a provided reference and optional county ID.
        /// </summary>
        /// <param name="reference">The unique reference string for the item.</param>
        /// <param name="countyId">The optional identifier for the county.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by the caller to cancel the asynchronous operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains an <see cref="IActionResult"/> representing the partial view or error response.</returns>
        [HttpGet("itembyreference")]
        public async Task<IActionResult> GetItemByReferenceAsync([FromQuery(Name = "reference")] string reference, [FromQuery(Name = "countyid")] int? countyId, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(reference))
            {
                return BadRequest();
            }

            HttpClient httpClient = httpClientFactory.CreateClient();

            UrlBuilder urlBuilder = new($"{Constants.Default.GISWebAPIUri}/gis/ortodatas/itembyreference");
            urlBuilder = urlBuilder.AddParameter("reference", reference);
            if (countyId.HasValue)
            {
                urlBuilder = urlBuilder.AddParameter("countyid", countyId.Value);
            }

            GIS.Classes.OrtoDatas? ortoDatas = await httpClient.ItemAsync<GIS.Classes.OrtoDatas>(urlBuilder.ToString(), cancellationToken);
            if (ortoDatas is null)
            {
                return NoContent();
            }

            // The reference record is context for the panel rather than its subject, so a missing one
            // renders the panel without it instead of failing the request.
            Building2DReference? building2DReference = await httpClient.Building2DReferenceAsync(reference, countyId, cancellationToken);

            OrtoDatasViewModel ortoDatasViewModel = new(building2DReference, ortoDatas);

            return PartialView("_OrtoDatasView", ortoDatasViewModel);
        }

        /// <summary>
        /// Retrieves the estimated orthophoto coverage factor of a single administrative area.
        /// </summary>
        /// <param name="administrativeAreal2DId">The unique identifier of the administrative area.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by the caller to cancel the asynchronous operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains an <see cref="IActionResult"/> with the coverage factor or error status.</returns>
        [HttpGet("estimatedcoveragefactor")]
        public async Task<IActionResult> GetEstimatedCoverageFactorAsync([FromQuery(Name = "administrativeareal2Did")] int administrativeAreal2DId, CancellationToken cancellationToken = default)
        {
            if (administrativeAreal2DId <= 0)
            {
                return BadRequest();
            }

            HttpClient httpClient = httpClientFactory.CreateClient();

            UrlBuilder urlBuilder = new($"{Constants.Default.GISWebAPIUri}/gis/ortodatas/estimatedcoveragefactor");
            urlBuilder = urlBuilder.AddParameter("administrativeareal2Did", administrativeAreal2DId);

            string? json = await httpClient.JsonAsync(urlBuilder.ToString(), cancellationToken);
            if (string.IsNullOrWhiteSpace(json))
            {
                return NoContent();
            }

            return Content(json, "application/json");
        }

        /// <summary>
        /// Retrieves the estimated orthophoto coverage factors of several administrative areas at once.
        /// <para>The values come back in the order the identifiers were given, which is what lets the page update one progress bar per row without matching anything up.</para>
        /// </summary>
        /// <param name="administrativeAreal2DIds">The unique identifiers of the administrative areas.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by the caller to cancel the asynchronous operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains an <see cref="IActionResult"/> with the list of coverage factor values or error status.</returns>
        [HttpPost("estimatedcoveragefactors")]
        public async Task<IActionResult> GetEstimatedCoverageFactorsAsync([FromBody] IEnumerable<int> administrativeAreal2DIds, CancellationToken cancellationToken = default)
        {
            if (administrativeAreal2DIds is null || !administrativeAreal2DIds.Any())
            {
                return BadRequest();
            }

            HttpClient httpClient = httpClientFactory.CreateClient();

            string? json = await httpClient.PostJsonAsync($"{Constants.Default.GISWebAPIUri}/gis/ortodatas/estimatedcoveragefactors", administrativeAreal2DIds, cancellationToken);
            if (string.IsNullOrWhiteSpace(json))
            {
                return NoContent();
            }

            List<double>? values = JsonSerializer.Deserialize<List<double>>(json);
            if (values is null)
            {
                return NoContent();
            }

            return Ok(values);
        }
    }
}
