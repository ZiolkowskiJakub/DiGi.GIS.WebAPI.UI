using DiGi.Analytical.Building.HVAC.Classes;
using DiGi.Analytical.Building.HVAC.Interfaces;
using DiGi.GIS.Classes;
using DiGi.GIS.Interfaces;
using DiGi.WebAPI.Classes;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace DiGi.GIS.WebAPI.UI.Classes
{
    [Route("[controller]")]
    public class HeatTransferCoefficientController : Controller
    {
        private readonly IHttpClientFactory httpClientFactory;

        // Constructor injection for the PostgreSQL data source
        public HeatTransferCoefficientController(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory;
        }

        [HttpGet("regulatedheattransfercoefficientsbyyear")]
        public async Task<IActionResult> GetRegulatedHeatTransferCoefficientsByYearAsync([FromQuery(Name = "year")] short year, CancellationToken cancellationToken = default)
        {
            HttpClient httpClient = httpClientFactory.CreateClient();

            UrlBuilder urlBuilder;
            HttpResponseMessage httpResponseMessage;
            string json;

            #region RegulatedHeatTransferCoefficients

            urlBuilder = new("https://api.digiproject.uk/gis/heattransfercoefficient/regulatedheattransfercoefficientsbyyear");
            urlBuilder = urlBuilder.AddParameter("year", year);

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

            List<IRegulatedHeatTransferCoefficients>? regulatedHeatTransferCoefficients = Core.Convert.ToDiGi<IRegulatedHeatTransferCoefficients>(json);
            if (regulatedHeatTransferCoefficients is null)
            {
                return NoContent();
            }

            #endregion RegulatedHeatTransferCoefficients

            RegulatedHeatTransferCoefficientsView regulatedHeatTransferCoefficientsView = new(year, regulatedHeatTransferCoefficients?.FirstOrDefault(), null);

            // We pass the objects to a Partial View
            return PartialView("_RegulatedHeatTransferCoefficientsView", regulatedHeatTransferCoefficientsView);
        }

        [HttpGet("regulatedheattransfercoefficientsbyreference")]
        public async Task<IActionResult> GetRegulatedHeatTransferCoefficientsByReferenceAsync([FromQuery(Name = "reference")] string reference, [FromQuery(Name = "countyid")] int? countyId)
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

            List<IYearBuiltData>? yearBuiltDatas = Core.Convert.ToDiGi<IYearBuiltData>(json);
            if (yearBuiltDatas is null)
            {
                return NoContent();
            }

            #endregion YearBuiltDatas

            #region Year

            YearBuiltData? yearBuiltData = yearBuiltDatas.Find(x => x is YearBuiltData) as YearBuiltData;
            if(yearBuiltData is null)
            {
                return NoContent();
            }

            IYearBuilt? yearBuilt = yearBuiltData.GetUserYearBuilt();
            if (yearBuilt is null)
            {
                yearBuilt = yearBuiltData.GetLatestPredictedYearBuilt();
            }

            if (yearBuilt is null)
            {
                return NoContent();
            }

            short year = yearBuilt.Year;

            # endregion Year

            #region RegulatedHeatTransferCoefficients

            urlBuilder = new("https://api.digiproject.uk/gis/heattransfercoefficient/regulatedheattransfercoefficientsbyyear");
            urlBuilder = urlBuilder.AddParameter("year", year);

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

            List<IRegulatedHeatTransferCoefficients>? regulatedHeatTransferCoefficientsList = Core.Convert.ToDiGi<IRegulatedHeatTransferCoefficients>(json);
            if (regulatedHeatTransferCoefficientsList is null)
            {
                return NoContent();
            }

            #endregion RegulatedHeatTransferCoefficients

            bool? isResidential = null;

            IRegulatedHeatTransferCoefficients? regulatedHeatTransferCoefficients = regulatedHeatTransferCoefficientsList?.FirstOrDefault();
            if(regulatedHeatTransferCoefficients is RegulatedHeatTransferCoefficients_2002)
            {
                #region Building2D

                urlBuilder = new("https://api.digiproject.uk/gis/building2d/itembyreference");
                urlBuilder = urlBuilder.AddParameter("reference", reference);
                if (countyId is not null && countyId.HasValue)
                {
                    urlBuilder = urlBuilder.AddParameter("countyId", countyId.Value);
                }

                httpResponseMessage = await httpClient.GetAsync(urlBuilder.ToString());
                if (httpResponseMessage.IsSuccessStatusCode)
                {

                    json = await httpResponseMessage.Content.ReadAsStringAsync();
                    if (!string.IsNullOrWhiteSpace(json))
                    {
                        Building2D? building2D = Core.Convert.ToDiGi<Building2D>(json)?.FirstOrDefault();
                        if (building2D is not null)
                        {
                            isResidential = Query.IsResidential(building2D);
                        }
                    }
                }
             
                #endregion Building2D
            }

            RegulatedHeatTransferCoefficientsView regulatedHeatTransferCoefficientsView = new(year, regulatedHeatTransferCoefficients, isResidential);

            // We pass the objects to a Partial View
            return PartialView("_RegulatedHeatTransferCoefficientsView", regulatedHeatTransferCoefficientsView);
        }
    }
}