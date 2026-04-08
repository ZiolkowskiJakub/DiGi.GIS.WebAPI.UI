using DiGi.WebAPI.Classes;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace DiGi.GIS.WebAPI.UI.Classes
{
    [Route("[controller]")]
    public class Building2DController : Controller
    {
        private readonly IHttpClientFactory httpClientFactory;

        // Constructor injection for the PostgreSQL data source
        public Building2DController(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory;
        }

        [HttpGet("building2Dreferencesbyadministrativeareal2Did")]
        public async Task<IActionResult> GetBuilding2DReferencesByAdministrativeAreal2DIdAsync([FromQuery(Name = "administrativeareal2Did")] int administrativeAreal2DId)
        {
            HttpClient httpClient = httpClientFactory.CreateClient();

            UrlBuilder urlBuilder = new("https://api.digiproject.uk/gis/building2D/building2Dreferencesbyadministrativeareal2Did");
            urlBuilder = urlBuilder.AddParameter("administrativeareal2Did", administrativeAreal2DId);

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

            // Here we use your DLL to turn JSON back into real C# objects.
            // Note: Since AdministrativeAreal2D is abstract,
            // you might need a specific converter or a concrete type.
            List<PostgreSQL.Classes.Building2DReference>? building2DReferences = Core.Convert.ToDiGi<PostgreSQL.Classes.Building2DReference>(json);

            // We pass the objects to a Partial View
            return PartialView("_Building2DReferences", building2DReferences ?? []);
        }

        // This action will trigger for: gis.digiproject.uk/administrativeareal2D
        [HttpGet("")]
        public IActionResult Start()
        {
            return View();
        }
    }
}