using DiGi.GIS.WebAPI.UI.ViewModels;
using DiGi.WebAPI.Classes;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace DiGi.GIS.WebAPI.UI.Controllers
{
    /// <summary> Provides controller endpoints for accessing and managing building data, acting as an interface between the client and the underlying GIS building data services. </summary>
    [Route("[controller]")]
    public class BuildingDataController : Controller
    {
        private readonly IHttpClientFactory httpClientFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="BuildingDataController"/> class.
        /// </summary>
        /// <param name="httpClientFactory">The <see cref="IHttpClientFactory"/> used to create and manage <see cref="HttpClient"/> instances for making API requests.</param>
        public BuildingDataController(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory;
        }

        /// <summary>
        /// Asynchronously retrieves a building data table based on the specified reference and an optional county identifier.
        /// </summary>
        /// <param name="reference">The unique reference string used to look up the table.</param>
        /// <param name="countyId">The optional ID of the county associated with the request.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by the caller to cancel the asynchronous operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains an <see cref="IActionResult"/> representing the HTTP response.</returns>
        [HttpGet("tablebyreference")]
        public async Task<IActionResult> GetTableByReferenceAsync([FromQuery(Name = "reference")] string reference, [FromQuery(Name = "countyid")] int? countyId = null, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(reference))
            {
                return BadRequest();
            }

            HttpClient httpClient = httpClientFactory.CreateClient();

            UrlBuilder urlBuilder = new($"{Constants.Default.GISWebAPIUri}/gis/buildingdata/tablebyreference");
            urlBuilder = urlBuilder.AddParameter("reference", reference);
            if (countyId.HasValue)
            {
                urlBuilder = urlBuilder.AddParameter("countyid", countyId.Value);
            }

            string? json = await httpClient.JsonAsync(urlBuilder.ToString(), cancellationToken);
            if (string.IsNullOrWhiteSpace(json))
            {
                return NoContent();
            }

            // Table is a plain data carrier rather than a DiGi serializable object, so it is deserialized
            // directly instead of through Core.Convert.ToDiGi. The DiGi qualifier is required: from this
            // namespace a bare PostgreSQL binds to DiGi.GIS.PostgreSQL, which holds no Table namespace.
            DiGi.PostgreSQL.Table.Classes.Table? table = JsonSerializer.Deserialize<DiGi.PostgreSQL.Table.Classes.Table>(json);
            if (table is null)
            {
                return NoContent();
            }

            return View("TableView", new TableViewModel(table));
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
