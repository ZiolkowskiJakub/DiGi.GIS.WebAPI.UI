using DiGi.GIS.PostgreSQL.Classes;
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

            List<Analytical.Building.HVAC.Interfaces.IRegulatedHeatTransferCoefficients>? regulatedHeatTransferCoefficients = Core.Convert.ToDiGi<Analytical.Building.HVAC.Interfaces.IRegulatedHeatTransferCoefficients>(json);
            if (regulatedHeatTransferCoefficients is null)
            {
                return NoContent();
            }

            #endregion RegulatedHeatTransferCoefficients

            RegulatedHeatTransferCoefficientsView regulatedHeatTransferCoefficientsView = new(year, regulatedHeatTransferCoefficients?.FirstOrDefault());

            // We pass the objects to a Partial View
            return PartialView("_RegulatedHeatTransferCoefficientsView", regulatedHeatTransferCoefficientsView);
        }
    }
}