using Microsoft.AspNetCore.Mvc;

namespace DiGi.GIS.WebAPI.UI.Controllers
{
    /// <summary>
    /// Provides the controller logic for handling requests to the home page of the GIS WebAPI user interface.
    /// </summary>
    [Route("")]
    public class HomeController : Controller
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HomeController"/> class.
        /// <para>The landing pages carry no data of their own, so nothing is injected here.</para>
        /// </summary>
        public HomeController()
        {
        }

        // This action will trigger for: gis.digiproject.uk/
        /// <summary>
        /// Returns the view for the Start page.
        /// </summary>
        /// <returns>An <see cref="Microsoft.AspNetCore.Mvc.IActionResult"/> result.</returns>
        [HttpGet("")]
        public IActionResult Start()
        {
            // This will look for Views/Home/Start.cshtml
            return View();
        }

        // This action will trigger for: gis.digiproject.uk/about
        /// <summary>
        /// Returns the view for the About page.
        /// </summary>
        /// <returns>An <see cref="Microsoft.AspNetCore.Mvc.IActionResult"/> result.</returns>
        [HttpGet("about")]
        public IActionResult About()
        {
            return View();
        }
    }
}