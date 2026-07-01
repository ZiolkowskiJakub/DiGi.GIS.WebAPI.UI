using DiGi.GIS.PostgreSQL.Classes;
using DiGi.GIS.WebAPI.UI.ViewModels;
using DiGi.WebAPI.Classes;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
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
        /// <returns>A task that represents the asynchronous operation. The task result contains an <see cref="IActionResult"/> representing the partial view or error response.</returns>
        [HttpGet("itembyreference")]
        public async Task<IActionResult> GetItemByIdAsync([FromQuery(Name = "reference")] string reference, [FromQuery(Name = "countyid")] int? countyId)
        {
            if (string.IsNullOrWhiteSpace(reference))
            {
                return BadRequest();
            }

            HttpClient httpClient = httpClientFactory.CreateClient();

            UrlBuilder urlBuilder;
            HttpResponseMessage httpResponseMessage;
            string json;

            #region OrtoDatas

            urlBuilder = new("https://api.digiproject.uk/gis/ortodatas/itembyreference");
            urlBuilder = urlBuilder.AddParameter("reference", reference);
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

            GIS.Classes.OrtoDatas? ortoDatas = Core.Convert.ToDiGi<GIS.Classes.OrtoDatas>(json)?.FirstOrDefault();
            if (ortoDatas is null)
            {
                return NoContent();
            }

            #endregion OrtoDatas

            #region Building2DReference

            urlBuilder = new("https://api.digiproject.uk/gis/building2D/building2Dreferencebyreference");
            urlBuilder = urlBuilder.AddParameter("reference", reference);
            if (countyId is not null && countyId.HasValue)
            {
                urlBuilder = urlBuilder.AddParameter("countyId", countyId.Value);
            }

            Building2DReference? building2DReference = null;

            httpResponseMessage = await httpClient.GetAsync(urlBuilder.ToString());
            if (httpResponseMessage.IsSuccessStatusCode)
            {
                json = await httpResponseMessage.Content.ReadAsStringAsync();
                if (!string.IsNullOrWhiteSpace(json))
                {
                    building2DReference = Core.Convert.ToDiGi<Building2DReference>(json)?.FirstOrDefault();
                }
            }

            #endregion Building2DReference

            OrtoDatasViewModel ortoDatasViewModel = new(building2DReference, ortoDatas);

            // We pass the objects to a Partial View
            return PartialView("_OrtoDatasView", ortoDatasViewModel);
        }

        /// <summary>
        /// Retrieves the estimated coverage factor for a specific administrative area L2D identifier.
        /// </summary>
        /// <param name="administrativeAreal2DId">The unique identifier of the administrative area L2D.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains an <see cref="IActionResult"/> with the coverage factor or error status.</returns>
        [HttpGet("estimatedcoveragefactor")]
        public async Task<IActionResult> GetEstimatedCoverageFactorAsync([FromQuery(Name = "administrativeareal2Did")] int administrativeAreal2DId)
        {
            // Using HttpClient to call the external API from the server side
            HttpClient httpClient = httpClientFactory.CreateClient();
            try
            {
                string url = $"https://api.digiproject.uk/gis/ortodatas/estimatedcoveragefactor?administrativeareal2Did={administrativeAreal2DId}";
                HttpResponseMessage response = await httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    return Ok(content);
                }
                return BadRequest();
            }
            catch
            {
                // Log error and return N/A equivalent
                return StatusCode(500, "Error");
            }
        }

        /// <summary>
        /// Retrieves estimated coverage factors for a collection of administrative area L2D identifiers.
        /// </summary>
        /// <param name="administrativeAreal2DIds">An enumerable collection of administrative area L2D identifiers.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains an <see cref="IActionResult"/> with the list of coverage factor values or error status.</returns>
        [HttpPost("estimatedcoveragefactors")]
        public async Task<IActionResult> GetEstimatedCoverageFactorsAsync([FromBody] IEnumerable<int> administrativeAreal2DIds)
        {
            if (administrativeAreal2DIds == null || !administrativeAreal2DIds.Any())
            {
                return BadRequest("The list of IDs cannot be empty.");
            }

            // Use IHttpClientFactory to prevent socket exhaustion
            HttpClient httpClient = httpClientFactory.CreateClient();

            try
            {
                // Note: If calling the method you provided, it expects a Body.
                // We change this to Post or send the list as a query string.
                string url = "https://api.digiproject.uk/gis/ortodatas/estimatedcoveragefactors";

                // Sending the collection in the body via POST (more reliable than GET with body)
                HttpResponseMessage httpResponseMessage = await httpClient.PostAsJsonAsync(url, administrativeAreal2DIds);

                if (httpResponseMessage.IsSuccessStatusCode)
                {
                    // Directly deserialize the response from the DLL-based logic
                    List<double>? values = await httpResponseMessage.Content.ReadFromJsonAsync<List<double>>();
                    if (values == null)
                    {
                        return NoContent();
                    }

                    return Ok(values);
                }

                return StatusCode((int)httpResponseMessage.StatusCode, "External API returned an error.");
            }
            catch (Exception exception)
            {
                // In a real scenario, log 'ex' using Serilog
                return StatusCode(500, $"Internal server error: {exception.Message}");
            }
        }
    }
}