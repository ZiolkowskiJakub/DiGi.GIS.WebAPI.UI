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
    /// Provides API endpoints for retrieving and managing year built data.
    /// </summary>
    [Route("[controller]")]
    public class YearBuiltDataController : Controller
    {
        private readonly IHttpClientFactory httpClientFactory;

        // Constructor injection for the PostgreSQL data source
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
        /// <returns>A task that represents the asynchronous operation, containing an <see cref="Microsoft.AspNetCore.Mvc.IActionResult"/> result.</returns>
        [HttpGet("itembyreference")]
        public async Task<IActionResult> GetItemByReferenceAsync([FromQuery(Name = "reference")] string reference, [FromQuery(Name = "countyid")] int? countyId)
        {
            if (string.IsNullOrWhiteSpace(reference))
            {
                return BadRequest();
            }

            HttpClient httpClient = httpClientFactory.CreateClient();

            UrlBuilder urlBuilder;
            HttpResponseMessage httpResponseMessage;
            string json;

            #region YearBuiltDatas

            urlBuilder = new("https://api.digiproject.uk/gis/yearbuiltdata/itemsbyreference");
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

            List<Interfaces.IYearBuiltData>? yearBuiltDatas = Core.Convert.ToDiGi<Interfaces.IYearBuiltData>(json);
            if (yearBuiltDatas is null)
            {
                return NoContent();
            }

            #endregion YearBuiltDatas

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

            YearBuiltDataViewModel yearBuiltDataViewModel = new(building2DReference, yearBuiltDatas?.FirstOrDefault());

            // We pass the objects to a Partial View
            return PartialView("_YearBuiltDataView", yearBuiltDataViewModel);
        }
    }
}