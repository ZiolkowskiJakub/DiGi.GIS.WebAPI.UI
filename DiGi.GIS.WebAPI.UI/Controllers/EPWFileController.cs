using DiGi.EPW.Classes;
using DiGi.GIS.WebAPI.UI.ViewModels;
using DiGi.WebAPI.Classes;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text.Json;
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
        /// <param name="x">The X coordinate (longitude).</param>
        /// <param name="y">The Y coordinate (latitude).</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains an <see cref="IActionResult"/> representing the HTTP response.</returns>
        [HttpGet("item")]
        public async Task<IActionResult> GetEPWFileAsync([FromQuery(Name = "x")] double x, [FromQuery(Name = "y")] double y)
        {
            HttpClient httpClient = httpClientFactory.CreateClient();

            UrlBuilder urlBuilder = new("https://api.digiproject.uk/gis/epwfile/item");
            urlBuilder = urlBuilder.AddParameter("x", x);
            urlBuilder = urlBuilder.AddParameter("y", y);

            HttpResponseMessage httpResponseMessage = await httpClient.GetAsync(urlBuilder.ToString());
            if (!httpResponseMessage.IsSuccessStatusCode)
            {
                return BadRequest();
            }

            string json = await httpResponseMessage.Content.ReadAsStringAsync();
            if (string.IsNullOrWhiteSpace(json))
            {
                return NoContent();
            }

            EPWFile? epwFile = JsonSerializer.Deserialize<EPWFile>(json);
            if (epwFile is null)
            {
                return BadRequest();
            }

            return View("EPWFileView", new EPWFileViewModel(epwFile));
        }

        /// <summary>
        /// Returns the starting view.
        /// </summary>
        /// <returns>An <see cref="IActionResult"/> representing the start view.</returns>
        [HttpGet("")]
        public IActionResult Start()
        {
            return View();
        }
    }
}