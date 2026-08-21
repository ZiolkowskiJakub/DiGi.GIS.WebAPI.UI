using DiGi.GIS.Interfaces;
using DiGi.GIS.PostgreSQL.Classes;
using DiGi.GIS.WebAPI.UI.ViewModels;
using DiGi.WebAPI.Classes;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
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
        /// Retrieves the occupancy data and building reference for a specific 2D building item by its reference.
        /// </summary>
        /// <param name="reference">The unique reference string of the building.</param>
        /// <param name="countyId">The optional ID of the county associated with the building.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by the caller to cancel the asynchronous operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains an <see cref="IActionResult"/>, which returns a partial view containing the occupancy data if successful; otherwise, a bad request or no content response.</returns>
        [HttpGet("building2d/itembyreference")]
        public async Task<IActionResult> GetBuilding2DItemByReferenceAsync([FromQuery(Name = "reference")] string reference, [FromQuery(Name = "countyid")] int? countyId, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(reference))
            {
                return BadRequest();
            }

            HttpClient httpClient = httpClientFactory.CreateClient();

            UrlBuilder urlBuilder = new($"{Constants.Default.GISWebAPIUri}/gis/occupancydata/building2d/itemsbyreference");
            urlBuilder = urlBuilder.AddParameter("reference", reference);
            if (countyId.HasValue)
            {
                urlBuilder = urlBuilder.AddParameter("countyid", countyId.Value);
            }

            List<IOccupancyData>? occupancyDatas = await httpClient.ItemsAsync<IOccupancyData>(urlBuilder.ToString(), cancellationToken);
            if (occupancyDatas is null || occupancyDatas.Count == 0)
            {
                return NoContent();
            }

            // The reference record is context for the panel rather than its subject, so a missing one
            // renders the panel without it instead of failing the request.
            Building2DReference? building2DReference = await httpClient.Building2DReferenceAsync(reference, countyId, cancellationToken);

            Building2DOccupancyDataViewModel building2DOccupancyDataViewModel = new(building2DReference, occupancyDatas.FirstOrDefault());

            return PartialView("_Building2DOccupancyDataView", building2DOccupancyDataViewModel);
        }
    }
}
