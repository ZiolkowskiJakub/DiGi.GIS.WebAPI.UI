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
    /// Provides API endpoints for retrieving and managing year built data.
    /// </summary>
    [Route("[controller]")]
    public class YearBuiltDataController : Controller
    {
        private readonly IHttpClientFactory httpClientFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="YearBuiltDataController"/> class.
        /// </summary>
        /// <param name="httpClientFactory">The <see cref="IHttpClientFactory"/> used to create HTTP clients.</param>
        public YearBuiltDataController(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory;
        }

        /// <summary>
        /// Retrieves a year built data item based on the specified reference and optional county identifier.
        /// </summary>
        /// <param name="reference">The unique reference of the item to retrieve.</param>
        /// <param name="countyId">The optional identifier of the county associated with the item.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by the caller to cancel the asynchronous operation.</param>
        /// <returns>A task that represents the asynchronous operation, containing an <see cref="Microsoft.AspNetCore.Mvc.IActionResult"/> result.</returns>
        [HttpGet("itembyreference")]
        public async Task<IActionResult> GetItemByReferenceAsync([FromQuery(Name = "reference")] string reference, [FromQuery(Name = "countyid")] int? countyId, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(reference))
            {
                return BadRequest();
            }

            HttpClient httpClient = httpClientFactory.CreateClient();

            UrlBuilder urlBuilder = new($"{Constants.Default.GISWebAPIUri}/gis/yearbuiltdata/itemsbyreference");
            urlBuilder = urlBuilder.AddParameter("reference", reference);
            if (countyId.HasValue)
            {
                urlBuilder = urlBuilder.AddParameter("countyid", countyId.Value);
            }

            List<IYearBuiltData>? yearBuiltDatas = await httpClient.ItemsAsync<IYearBuiltData>(urlBuilder.ToString(), cancellationToken);
            if (yearBuiltDatas is null || yearBuiltDatas.Count == 0)
            {
                return NoContent();
            }

            // The reference record is context for the panel rather than its subject, so a missing one
            // renders the panel without it instead of failing the request.
            Building2DReference? building2DReference = await httpClient.Building2DReferenceAsync(reference, countyId, cancellationToken);

            YearBuiltDataViewModel yearBuiltDataViewModel = new(building2DReference, yearBuiltDatas.FirstOrDefault());

            return PartialView("_YearBuiltDataView", yearBuiltDataViewModel);
        }
    }
}
