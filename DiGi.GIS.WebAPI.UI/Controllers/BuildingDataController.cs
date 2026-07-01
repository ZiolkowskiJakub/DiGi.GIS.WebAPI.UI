using DiGi.GIS.WebAPI.UI.ViewModels;
using DiGi.WebAPI.Classes;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text.Json;
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
        /// <returns>A task that represents the asynchronous operation. The task result contains an <see cref="IActionResult"/> representing the HTTP response.</returns>
        [HttpGet("tablebyreference")]
        public async Task<IActionResult> GetTableByReferenceAsync([FromQuery(Name = "reference")] string reference, [FromQuery(Name = "countyid")] int? countyId = null)
        {
            HttpClient httpClient = httpClientFactory.CreateClient();

            UrlBuilder urlBuilder = new("https://api.digiproject.uk/gis/buildingdata/tablebyreference");
            urlBuilder = urlBuilder.AddParameter("reference", reference);

            if (countyId is not null)
            {
                urlBuilder = urlBuilder.AddParameter("countyid", countyId.Value);
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
            DiGi.PostgreSQL.Table.Classes.Table? table = JsonSerializer.Deserialize<DiGi.PostgreSQL.Table.Classes.Table>(json);
            if (table is null)
            {
                return BadRequest();
            }

            // We pass the objects to a View
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