using DiGi.GIS.PostgreSQL.Classes;
using DiGi.GIS.WebAPI.UI.ViewModels;
using DiGi.WebAPI.Classes;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace DiGi.GIS.WebAPI.UI.Controllers
{
    /// <summary>
    /// Controller responsible for handling requests related to occupancy data for 2D buildings.
    /// </summary>
    [Route("[controller]")]
    public class OccupancyDataController : Controller
    {
        private readonly IHttpClientFactory httpClientFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="OccupancyDataController"/> class.
        /// </summary>
        /// <param name="httpClientFactory">The HTTP client factory used to create clients for external API communication.</param>
        public OccupancyDataController(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory;
        }

        /// <summary>
        /// Retrieves the occupancy data and building reference for a specific 2D building item by its reference identifier.
        /// </summary>
        /// <param name="reference">The unique reference string of the building.</param>
        /// <param name="countyId">The optional ID of the county associated with the building.</param>
        /// <param name="isResidential">An optional flag indicating whether the building is residential.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains an <see cref="IActionResult"/>, which returns a partial view containing the occupancy data if successful; otherwise, a bad request or no content response.</returns>
        [HttpGet("building2d/itembyreference")]
        public async Task<IActionResult> GetBuilding2DItemByIdAsync([FromQuery(Name = "reference")] string reference, [FromQuery(Name = "countyid")] int? countyId, [FromQuery(Name = "isresidential")] bool? isResidential)
        {
            if (string.IsNullOrWhiteSpace(reference))
            {
                return BadRequest();
            }

            HttpClient httpClient = httpClientFactory.CreateClient();

            UrlBuilder urlBuilder;
            HttpResponseMessage httpResponseMessage;
            string json;

            #region OccupancyDatas

            urlBuilder = new("https://api.digiproject.uk/gis/occupancydata/building2d/itemsbyreference");
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

            List<Interfaces.IOccupancyData>? occupancyDatas = Core.Convert.ToDiGi<Interfaces.IOccupancyData>(json);
            if (occupancyDatas is null)
            {
                return NoContent();
            }

            #endregion OccupancyDatas

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

            Building2DOccupancyDataView building2DOccupancyDataView = new(building2DReference, occupancyDatas?.FirstOrDefault());

            // We pass the objects to a Partial View
            return PartialView("_Building2DOccupancyDataView", building2DOccupancyDataView);
        }
    }
}