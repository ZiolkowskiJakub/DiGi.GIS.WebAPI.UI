using System.Net.Http;
using Microsoft.AspNetCore.Mvc;

namespace DiGi.GIS.WebAPI.UI.Controllers
{
    /// <summary>
    /// Provides the Typology definition feature: the page where a building typology is defined as a chain of building-data columns with rule types, ranges and colors.
    /// <para>The column data and the solving are owned by the GIS Web API (ZiolkowskiJakub/DiGi.Gis#5); this controller only reads and renders, so the query and rule semantics stay owned by that service and cannot drift here.</para>
    /// </summary>
    [Route("[controller]")]
    public class TypologyController : Controller
    {
        private readonly IHttpClientFactory httpClientFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="TypologyController"/> class.
        /// </summary>
        /// <param name="httpClientFactory">The <see cref="IHttpClientFactory"/> the feature's data actions use to create <see cref="HttpClient"/> instances.</param>
        public TypologyController(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory;
        }

        // This action will trigger for: gis.digiproject.uk/typology
        /// <summary>
        /// Starts the Typology definition page.
        /// </summary>
        /// <returns>An <see cref="IActionResult"/> result that renders the start view.</returns>
        [HttpGet("")]
        public IActionResult Start()
        {
            return View();
        }
    }
}
