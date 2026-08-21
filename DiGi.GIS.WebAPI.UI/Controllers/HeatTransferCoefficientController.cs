using DiGi.Analytical.Building.HVAC.Classes;
using DiGi.Analytical.Building.HVAC.Interfaces;
using DiGi.GIS.Classes;
using DiGi.GIS.Interfaces;
using DiGi.GIS.WebAPI.UI.ViewModels;
using DiGi.WebAPI.Classes;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace DiGi.GIS.WebAPI.UI.Controllers
{
    /// <summary>
    /// Controller responsible for handling requests related to heat transfer coefficients,
    /// providing data based on year or building reference.
    /// </summary>
    [Route("[controller]")]
    public class HeatTransferCoefficientController : Controller
    {
        private readonly IHttpClientFactory httpClientFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="HeatTransferCoefficientController"/> class.
        /// </summary>
        /// <param name="httpClientFactory">The HTTP client factory used to create clients for external API communication.</param>
        public HeatTransferCoefficientController(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory;
        }

        /// <summary>
        /// Retrieves regulated heat transfer coefficients for a building identified by its reference,
        /// determining the applicable year based on the building's construction data.
        /// </summary>
        /// <param name="reference">The unique reference string of the building.</param>
        /// <param name="countyId">The optional identifier for the county associated with the building.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by the caller to cancel the asynchronous operation.</param>
        /// <returns>An <see cref="IActionResult"/> containing a partial view with the coefficient data, or an error result if the request fails.</returns>
        [HttpGet("regulatedheattransfercoefficientsbyreference")]
        public async Task<IActionResult> GetRegulatedHeatTransferCoefficientsByReferenceAsync([FromQuery(Name = "reference")] string reference, [FromQuery(Name = "countyid")] int? countyId, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(reference))
            {
                return BadRequest();
            }

            HttpClient httpClient = httpClientFactory.CreateClient();

            UrlBuilder urlBuilder;

            #region YearBuiltDatas

            urlBuilder = new($"{Constants.Default.GISWebAPIUri}/gis/yearbuiltdata/itemsbyreference");
            urlBuilder = urlBuilder.AddParameter("reference", reference);
            if (countyId.HasValue)
            {
                urlBuilder = urlBuilder.AddParameter("countyid", countyId.Value);
            }

            List<IYearBuiltData>? yearBuiltDatas = await httpClient.ItemsAsync<IYearBuiltData>(urlBuilder.ToString(), cancellationToken);
            if (yearBuiltDatas is null)
            {
                return NoContent();
            }

            #endregion YearBuiltDatas

            #region Year

            YearBuiltData? yearBuiltData = yearBuiltDatas.Find(x => x is YearBuiltData) as YearBuiltData;
            if (yearBuiltData is null)
            {
                return NoContent();
            }

            IYearBuilt? yearBuilt = yearBuiltData.GetUserYearBuilt();
            yearBuilt ??= yearBuiltData.GetLatestPredictedYearBuilt();

            if (yearBuilt is null)
            {
                return NoContent();
            }

            short year = yearBuilt.Year;

            #endregion Year

            #region RegulatedHeatTransferCoefficients

            urlBuilder = new($"{Constants.Default.GISWebAPIUri}/gis/heattransfercoefficient/regulatedheattransfercoefficientsbyyear");
            urlBuilder = urlBuilder.AddParameter("year", year);

            IRegulatedHeatTransferCoefficients? regulatedHeatTransferCoefficients = await httpClient.ItemAsync<IRegulatedHeatTransferCoefficients>(urlBuilder.ToString(), cancellationToken);
            if (regulatedHeatTransferCoefficients is null)
            {
                return NoContent();
            }

            #endregion RegulatedHeatTransferCoefficients

            #region IsResidential

            // The 2002 regulation is the only one that sets different limits for a residential building,
            // so the footprint is only read when that is the regulation in force.
            bool? isResidential = null;

            if (regulatedHeatTransferCoefficients is RegulatedHeatTransferCoefficients_2002)
            {
                urlBuilder = new($"{Constants.Default.GISWebAPIUri}/gis/building2d/itembyreference");
                urlBuilder = urlBuilder.AddParameter("reference", reference);
                if (countyId.HasValue)
                {
                    urlBuilder = urlBuilder.AddParameter("countyid", countyId.Value);
                }

                Building2D? building2D = await httpClient.ItemAsync<Building2D>(urlBuilder.ToString(), cancellationToken);
                if (building2D is not null)
                {
                    isResidential = GIS.Query.IsResidential(building2D);
                }
            }

            #endregion IsResidential

            RegulatedHeatTransferCoefficientsViewModel regulatedHeatTransferCoefficientsViewModel = new(year, regulatedHeatTransferCoefficients, isResidential);

            return PartialView("_RegulatedHeatTransferCoefficientsView", regulatedHeatTransferCoefficientsViewModel);
        }
    }
}
