using Microsoft.AspNetCore.Mvc;

namespace DiGi.GIS.WebAPI.UI.Classes
{
    [Route("")]
    public class HomeController : Controller
    {
        // Constructor injection for the PostgreSQL data source
        public HomeController()
        {
        }

        // This action will trigger for: gis.digiproject.uk/
        [HttpGet("")]
        public IActionResult Start()
        {
            // This will look for Views/Home/Start.cshtml
            return View();
        }

        // This action will trigger for: gis.digiproject.uk/about
        [HttpGet("about")]
        public IActionResult About()
        {
            return View();
        }
    }
}