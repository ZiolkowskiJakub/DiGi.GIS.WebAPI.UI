using Microsoft.AspNetCore.Mvc;

namespace DiGi.GIS.WebAPI.UI.Classes
{
    /// <summary>
    /// Provides the controller logic for handling requests to the home page of the GIS WebAPI user interface.
    /// </summary>
    [Route("")]
    public class HomeController : Controller
    {
        // Constructor injection for the PostgreSQL data source
        /// <summary> Handles the request for the start page and returns the corresponding view. </summary>
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
