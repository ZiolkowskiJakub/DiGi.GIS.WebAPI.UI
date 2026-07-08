using DiGi.Analytical.Building.Classes;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
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
        /// Asynchronously creates a <see cref="BuildingModel"/> for the building with the specified unique identifier (see <see cref="Create.BuildingModelAsync(HttpClient?, long, int?, double, double)"/>) and returns it as JSON.
        /// </summary>
        /// <param name="id">The unique identifier of the building.</param>
        /// <param name="countyId">The optional unique identifier of the county associated with the building.</param>
        /// <returns>A <see cref="Task{IActionResult}"/> holding the serialized <see cref="BuildingModel"/>.</returns>
        [HttpGet("itembyid")]
        public async Task<IActionResult> GetItemByIdAsync([FromQuery(Name = "id")] long id, [FromQuery(Name = "countyid")] int? countyId)
        {
            HttpClient httpClient = httpClientFactory.CreateClient();

            BuildingModel? buildingModel = await httpClient.BuildingModelAsync(id, countyId);
            if (buildingModel is null)
            {
                return NoContent();
            }

            string json = Core.Convert.ToSystem_String(buildingModel) ?? string.Empty;
            if (string.IsNullOrWhiteSpace(json))
            {
                return NotFound();
            }

            return Content(json, "application/json");
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
