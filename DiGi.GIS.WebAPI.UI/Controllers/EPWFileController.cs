using DiGi.EPW.Classes;
using DiGi.GIS.WebAPI.UI.ViewModels;
using DiGi.WebAPI.Classes;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace DiGi.GIS.WebAPI.UI.Controllers
{
    /// <summary>
    /// Provides controller endpoints for accessing and managing EPW file data.
    /// </summary>
    [Route("[controller]")]
    public class EPWFileController : Controller
    {
        private readonly IHttpClientFactory httpClientFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="EPWFileController"/> class.
        /// </summary>
        /// <param name="httpClientFactory">The HTTP client factory.</param>
        public EPWFileController(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory;
        }

        /// <summary>
        /// Asynchronously retrieves an EPW file based on the specified coordinates.
        /// </summary>
        /// <param name="x">The X coordinate, in PL-1992 (EPSG:2180) metres.</param>
        /// <param name="y">The Y coordinate, in PL-1992 (EPSG:2180) metres.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by the caller to cancel the asynchronous operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains an <see cref="IActionResult"/> representing the HTTP response.</returns>
        [HttpGet("item")]
        public async Task<IActionResult> GetEPWFileAsync([FromQuery(Name = "x")] double x, [FromQuery(Name = "y")] double y, CancellationToken cancellationToken = default)
        {
            if (!double.IsFinite(x) || !double.IsFinite(y))
            {
                return BadRequest();
            }

            HttpClient httpClient = httpClientFactory.CreateClient();

            UrlBuilder urlBuilder = new($"{Constants.Default.GISWebAPIUri}/gis/epwfile/item");
            urlBuilder = urlBuilder.AddParameter("x", x);
            urlBuilder = urlBuilder.AddParameter("y", y);

            EPWFile? epwFile = await httpClient.ItemAsync<EPWFile>(urlBuilder.ToString(), cancellationToken);
            if (epwFile is null)
            {
                return NoContent();
            }

            return View("EPWFileView", new EPWFileViewModel(epwFile));
        }
    }
}
